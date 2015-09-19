#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="InternetCommunicator.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Communication {
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics.Contracts;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Threading;
  using System.Threading.Tasks;

  using Aegis_DVL.Commands;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  /// <summary>
  /// The communicator.
  /// </summary>
  public class InternetCommunicator : ICommunicator {
    public static int DEFAULT_PORT = 62000;

    private TcpListener tcpListener;
    private Socket udpSocket;
    private BlockingCollection<ICommand> incomingQueue;
    private BlockingCollection<Tuple<IPEndPoint, ICommand, int>> outgoingQueue;

    private int maxTries = 5;

    private Thread receiveThread;
    private Thread sendThread;
    private Thread dequeueThread;
    private Thread pingThread;

    private Dictionary<IPEndPoint, DateTime> responseTimes = new Dictionary<IPEndPoint, DateTime>();

    private int lengthOfCount = 4;
    private int TCPTimeout = 10000;

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InternetCommunicator"/> class. 
    /// May I have a new InternetCommunicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public InternetCommunicator(Station parent) {
      Contract.Requires(parent != null);
      Parent = parent;
      incomingQueue = new BlockingCollection<ICommand>(new ConcurrentQueue<ICommand>());
      outgoingQueue = new BlockingCollection<Tuple<IPEndPoint, ICommand, int>>(new ConcurrentQueue<Tuple<IPEndPoint, ICommand, int>>());
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent { get; private set; }

    public bool ThreadsStarted { get; private set; }

    #endregion

    // TODO: review for problems with complexity
    #region Public Methods and Operators

    /// <summary>
    /// The discover network machines.
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    [Pure] public IEnumerable<IPEndPoint> DiscoverPeers() {
      var cdEvent = new CountdownEvent(1);
      string myip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip =>
                    ip.AddressFamily == AddressFamily.InterNetwork).ToString();
      string myipprefix = myip.Substring(0, myip.LastIndexOf('.') + 1);
      Console.WriteLine("my ip address is " + myip);

      // narrow down possible targets
      var potentials = new List<IPEndPoint>();

      using (var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
        try {
          bool done = false;
          byte[] buffer = new byte[32];
          IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);
          EndPoint serverRemote = (EndPoint) server;
          udpSocket.ReceiveTimeout = 2000;
          udpSocket.SendTo(new byte[] { 0 }, new IPEndPoint(IPAddress.Parse(myipprefix + "255"), 62000));
          while (!done) {
            try {
              int length = udpSocket.ReceiveFrom(buffer, ref serverRemote);
              IPEndPoint responder = new IPEndPoint(IPAddress.Parse(System.Text.Encoding.ASCII.GetString(buffer, 0, length)), 62000);
              potentials.Add(responder);
              Console.WriteLine("got response from " + responder.Address);
            } catch (Exception) {
              done = true;
            }
          }
        } catch (Exception) {
          // something went wrong so fall back to the old way of doing things
          for (int i = 1; i < 255; i++) {
            potentials.Add(new IPEndPoint(IPAddress.Parse(myipprefix + i), 62000));
          }
        }
      }

      potentials.Remove(new IPEndPoint(IPAddress.Parse(myip), 62000));
      Console.WriteLine("Found a total of " + potentials.Count() + " possible stations.");
      return potentials;
    }

    /// <summary>
    /// Gets the local end point for this instance of the pollbook. If 
    /// one does not exist, it is created. Note that this starts the network
    /// listeners if they are not already running.
    /// </summary>
    /// <returns>
    /// The IPEndPoint representing the local end point.
    /// </returns>
    public IPEndPoint GetLocalEndPoint(int port) {
      IPEndPoint ipep = new IPEndPoint(
          Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip =>
            ip.AddressFamily == AddressFamily.InterNetwork),
          port);
      try {
        if (tcpListener == null) {
          tcpListener = new TcpListener(ipep);
          tcpListener.Start();
        }

        if (udpSocket == null) {
          udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
          udpSocket.Bind(ipep);
        }
      }
      catch (Exception e) {
        if (Parent.Logger != null)
        {
          Parent.Logger.Log("Problem starting network operations with exception " + e, Level.Info);
        }
        else
        {
          Console.WriteLine("Problem starting network operations with exception " + e);
        }
      }
      return ipep;
    }

    /// <summary>
    /// Destroys the local end point for this instance of the pollbook.
    /// </summary>
    /// <returns>
    /// The IPEndPoint representing the local end point.
    /// </returns>
    void DestroyLocalEndPoint() {
      if (tcpListener != null) {
        try {
          tcpListener.Stop();
        } catch (Exception) {
          // ignored
        }
      }

      if (udpSocket != null) {
        try {
          udpSocket.Close();
        } catch (Exception) {
          // ignored
        }
      }
    }

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify this DVL.
    /// </summary>
    /// <returns>
    /// The IP address of this DVL.
    /// </returns>
    public string GetIdentifyingString() {
      string result = "0.0.0.0";

      if (tcpListener != null) {
        result = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip =>
            ip.AddressFamily == AddressFamily.InterNetwork).ToString();
      }

      return result;
    }

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify the DVL
    /// at the specified address.
    /// </summary>
    /// <param name="address">
    /// The IPEndPoint representing the address of the DVL
    /// </param>
    /// <returns>
    /// A string identifying the DVL represented by the specified address
    /// </returns>
    public string GetIdentifyingStringForStation(IPEndPoint address) {
      return address.Address.ToString();
    }

    /// <summary>
    /// The is listening.
    /// </summary>
    /// <param name="address">
    /// The address.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    [Pure] public bool IsListening(IPEndPoint address) {
      bool result = true;

      if (responseTimes.ContainsKey(address) &&
          DateTime.Now.Subtract(responseTimes[address]).TotalSeconds < 30) {
        // we've heard from this station within the last minute
        result = true;
      } else {
        // we haven't heard from this station in a minute, or ever, let's ping it
        TcpClient client = new TcpClient();
        WaitHandle wh = null;

        try {
          IAsyncResult ar = client.BeginConnect(address.Address, address.Port, null, null);
          wh = ar.AsyncWaitHandle;

          if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(2000), false)) throw new SocketException();
          client.EndConnect(ar);
        } catch (SocketException e) {
          if (Parent.Logger != null) {
            Parent.Logger.Log("Could not connect to " + address + ", assuming down. " + e, Level.Error);
          } else {
            Console.WriteLine("Could not connect to " + address + ", assuming down. " + e);
          }
          result = false;
        } finally {
          if (wh != null) {
            wh.Close();
          }
        }

        if (result) {
          byte[] bytes = Bytes.From(new IsAliveCommand(Parent.Address));
          byte[] lengthBytes = BitConverter.GetBytes(bytes.Count());
          using (NetworkStream stream = client.GetStream()) {
            try {
              stream.Write(lengthBytes, 0, lengthBytes.Count());
              stream.Write(bytes, 0, bytes.Count());
            } catch (Exception e) {
              if (Parent.Logger != null) {
                Parent.Logger.Log("Could not send IsAlive command to " + address + ", assuming down." + e, Level.Error);
              } else {
                Console.WriteLine("Could not send IsAlive command to " + address + ", assuming down." + e);
              }
              result = false;
            }
          }
        }
      }
      return result;
    }

    public void PingThread() {
      if (udpSocket == null) {
        udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        udpSocket.Bind(Parent.Address);
      }
      byte[] buffer = new byte[32];
      byte[] myipbytes = System.Text.Encoding.ASCII.GetBytes(Parent.Address.Address.ToString());
      EndPoint server = new IPEndPoint(IPAddress.Any, 0);
      while (true) {
        try {
          udpSocket.ReceiveFrom(buffer, ref server);
        } catch (SocketException e) {
          // the socket was closed, or some other problem happened that we can't survive
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Ping listener died with exception " + e, Level.Info);
          }
          else
          {
            Console.WriteLine("Ping listener died with exception " + e);
          } 
          return;
        }
        IPEndPoint ips = (IPEndPoint)server;
        try {
          if (!ips.Address.Equals(Parent.Address.Address)) {
            udpSocket.SendTo(myipbytes, server);
            Console.WriteLine("pinged by " + ips.Address);
          }
        } catch (SocketException e) {
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Ping listener encountered recoverable exception " + e, Level.Error);
          }
          else
          {
            Console.WriteLine("Ping listener encountered recoverable exception " + e);
          }
        }
      }
    }

    public void AcceptConnectionThread() {
      if (tcpListener == null) {
        tcpListener = new TcpListener(Parent.Address);
        tcpListener.Start();
      }

      while (true) {
        try {
          TcpClient client = tcpListener.AcceptTcpClient();
          if (Parent.Logger != null) {
            Parent.Logger.Log("Accepted connection from " + client.Client.RemoteEndPoint, Level.Info);
          } else {
            Console.WriteLine("Accepted connection from " + client.Client.RemoteEndPoint);
          }

          ProcessConnection(client);
        } catch (Exception e) {
          // the socket was closed, or some other problem happened that we can't survive
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Listener died with exception " + e, Level.Info);
          }
          else
          {
            Console.WriteLine("Listener died with exception " + e);
          }
        }
      }
    }

    /// <summary>
    /// The receive and handle.
    /// </summary>
    /// TODO: review for problems with complexity
    private void ProcessConnection(TcpClient client) {
      client.ReceiveTimeout = TCPTimeout;
      IPEndPoint clientEndpoint = null;
      try {
        using (NetworkStream stream = client.GetStream()) {
          clientEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
          if (Parent.Logger != null) {
            Parent.Logger.Log("Handling connection from " + clientEndpoint, Level.Info);
          } else {
            Console.WriteLine("Handling connection from " + clientEndpoint);
          }
          client.Client.ReceiveTimeout = TCPTimeout;
          byte[] lengthBytes = Bytes.FromNetworkStreamWithSize(stream, lengthOfCount);
          var length = BitConverter.ToInt32(lengthBytes, 0);
          if (length == 0) {
            if (Parent.Logger != null) {
              Parent.Logger.Log("Received empty message from " + clientEndpoint, Level.Info);
            } else {
              Console.WriteLine("Received empty message from " + clientEndpoint);
            }
            return;
          }
          byte[] cmdBytes = null;
          try {
            cmdBytes = Bytes.FromNetworkStreamWithSize(stream, length);
          } catch (Exception e) {
            if (Parent.Logger != null) {
              Parent.Logger.Log("Failed to receive message from " + clientEndpoint + ": " + e, Level.Info);
            } else {
              Console.WriteLine("Failed to receive message from " + clientEndpoint + ": " + e);
            }
            stream.Write(new byte[lengthOfCount], 0, lengthOfCount);
            return;
          }
          var cmd = cmdBytes.To<ICommand>();
          Type cmdType = cmd.GetType();
          if (cmd is CryptoCommand) {
            cmdType = ((CryptoCommand)cmd).GetEncapsulatedType();
          }
          if (cmd is PublicKeyExchangeCommand ||
              cmd is CryptoCommand ||
              cmd is IsAliveCommand ||
              cmd is DisconnectStationCommand) {
            if (Parent.Logger != null && !(cmd is IsAliveCommand)) {
              Parent.Logger.Log("Received " + cmdType + " from " + cmd.Sender, Level.Info);
            } else {
              Console.WriteLine("Received " + cmdType + " from " + cmd.Sender);
            }
            // enqueue the command
            incomingQueue.Add(cmd);
            responseTimes[cmd.Sender] = DateTime.Now;
          } else {
            if (Parent.Logger != null) {
              Parent.Logger.Log(
                "Received " + cmdType + " from " + clientEndpoint + ". Shutting down.",
                Level.Fatal);
            } else {
              Console.WriteLine(
                "Received " + cmdType + " from " + clientEndpoint + ". Shutting down.");
            }

            Parent.ShutDownElection();
          }
        }
      } catch (Exception e) {
        // there was a problem reading a message, but the socket should be OK
        // so we live to try another day
        Console.WriteLine("Exception while reading from " + clientEndpoint + ": " + e);
      }
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    /// <exception cref="SocketException">
    /// </exception>
    public void Send(ICommand command, IPEndPoint target) {
      Send(command, target, maxTries);
    }

    private void Send(ICommand command, IPEndPoint target, int tries) {
      outgoingQueue.Add(Tuple.Create<IPEndPoint, ICommand, int>(target, command, tries));
    }

    public void ReceivedMessageDequeueThread() {
      bool running = true;
      while (running) {
        ICommand command = null;
        try {
          command = incomingQueue.Take();
        } catch (Exception e) {
          // the only way this can happen is if something has gone wrong
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Dequeue thread died with exception " + e, Level.Info);
          }
          else
          {
            Console.WriteLine("Dequeue thread died with exception " + e);
          } running = false;
        }
        if (command != null) {
          command.Execute(Parent);
        }
      }
    }

    public void MessageSendThread() {
      bool running = true;
      while (running) {
        Tuple<IPEndPoint, ICommand, int> message = outgoingQueue.Take();
        IPEndPoint target = message.Item1;
        ICommand command = message.Item2;
        int tries = message.Item3;
        int attempt = maxTries - tries + 1;
        bool retry = false;

        Type commandType = command.GetType();
        if (command is CryptoCommand) {
          CryptoCommand c = (CryptoCommand)command;
          commandType = c.GetEncapsulatedType();
        }

        if (tries > 0) {
          if (Parent.Logger != null && !(command is IsAliveCommand)) {
            Parent.Logger.Log("Attempt #" + attempt + " to send " + commandType + " to " + target, Level.Info);
          }
          if (!(command is PublicKeyExchangeCommand || command is IsAliveCommand ||
                command is CryptoCommand || command is DisconnectStationCommand)) {
            if (Parent.Peers.ContainsKey(target)) {
              command = new CryptoCommand(Parent, target, command);
            } else {
              if (Parent.Logger != null) Parent.Logger.Log("Attempt to send a message to non-peer " + target, Level.Error);
              return;
            }
          }

          TcpClient client = new TcpClient();
          client.ReceiveTimeout = TCPTimeout;
          WaitHandle wh = null;

          try {
            IAsyncResult ar = client.BeginConnect(target.Address, target.Port, null, null);
            wh = ar.AsyncWaitHandle;

            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(2000), false)) throw new SocketException();
            client.EndConnect(ar);
          } catch (SocketException e) {
            if (Parent.Logger != null) {
              Parent.Logger.Log("Problem sending due to SocketException " + e + ", retrying.", Level.Error);
            } else {
              Console.WriteLine("Problem sending due to SocketException " + e + ", retrying.");
            }
            retry = true;
          } finally {
            if (wh != null) {
              wh.Close();
            }
          }

          if (!retry) {
            byte[] commandBytes = Bytes.From(command);
            byte[] length = BitConverter.GetBytes(commandBytes.Count());
            using (NetworkStream stream = client.GetStream()) {
              try {
                stream.Write(length, 0, length.Count());
                stream.Write(commandBytes, 0, commandBytes.Count());

                if (Parent.Logger != null) {
                  Parent.Logger.Log(commandType + " successfully sent to " + target + " on attempt #" + attempt, Level.Info);
                } else {
                  Console.WriteLine(commandType + " successfully sent to " + target + " on attempt #" + attempt);
                }
              } catch (Exception e) {
                if (Parent.Logger != null) {
                  Parent.Logger.Log(commandType + " failed to " + target + " on attempt #" + attempt + ": " + e, Level.Info);
                } else {
                  Console.WriteLine(commandType + " failed to " + target + " on attempt #" + attempt + ": " + e);
                }
                if (!(command is DisconnectStationCommand)) {
                  retry = true;
                }
              }
            }
          }
        } else {
          if (Parent.Logger != null) {
            Parent.Logger.Log("Maximum number of retries reached, recording absent host " + target, Level.Error);
          } else {
            Console.WriteLine("Maximum number of retries reached, recording absent host " + target);
          }
          if (Parent.IsManager && Parent.Peers.ContainsKey(target)) {
            Console.WriteLine("I am manager, alerting other peers of absent host.");
            Parent.AnnounceRemovePeer(target);
            Parent.RemovePeer(target, false);
          } else if (target.Equals(Parent.Manager)) {
            Console.WriteLine("Absent host was manager, attempting to elect new manager.");
            Parent.StartNewManagerElection();
          } else if (Parent.Peers.ContainsKey(target)) {
            Parent.PeerStatuses[target].ConnectionState = "Not Connected";
            Parent.RemovePeer(target, false);
          }
        }

        if (retry) {
          Send(command, target, tries - 1);
        }
      }
    }

    public void StopThreads() {
      if (ThreadsStarted) {
        receiveThread.Abort();
        dequeueThread.Abort();
        sendThread.Abort();
        pingThread.Abort();

        DestroyLocalEndPoint();
        receiveThread = null;
        dequeueThread = null;
        sendThread = null;
        pingThread = null;
      }

      ThreadsStarted = false;
    }

    public void StartThreads() {
      if (!ThreadsStarted) {
        receiveThread = new Thread(AcceptConnectionThread);
        receiveThread.SetApartmentState(ApartmentState.STA);
        receiveThread.Name = "Receive";
        dequeueThread = new Thread(ReceivedMessageDequeueThread);
        dequeueThread.SetApartmentState(ApartmentState.STA);
        dequeueThread.Name = "Dequeue";
        sendThread = new Thread(MessageSendThread);
        sendThread.SetApartmentState(ApartmentState.STA);
        sendThread.Name = "Send";
        pingThread = new Thread(PingThread);
        pingThread.SetApartmentState(ApartmentState.STA);
        pingThread.Name = "PingResponder";

        receiveThread.Start();
        dequeueThread.Start();
        sendThread.Start();
        pingThread.Start();
      }

      ThreadsStarted = true;
    }

    #endregion
  }
}
