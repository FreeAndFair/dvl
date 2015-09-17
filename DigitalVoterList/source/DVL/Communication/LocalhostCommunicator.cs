#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Communicator.cs" company="DemTech">
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
  public class LocalhostCommunicator : ICommunicator {

    private static uint IOC_IN = 0x80000000;
    private static uint IOC_VENDOR = 0x18000000;
    private static uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

    private TcpListener tcpListener;
    private Socket udpSocket;
    private BlockingCollection<ICommand> incomingQueue;
    private BlockingCollection<Tuple<IPEndPoint, ICommand, int>> outgoingQueue;
    private int localPort;

    private int maxTries = 5;

    private Thread receiveThread;
    private Thread sendThread;
    private Thread dequeueThread;
    private Thread pingThread;

    private Dictionary<IPEndPoint, DateTime> responseTimes = new Dictionary<IPEndPoint, DateTime>();

    private int lengthOfCount = 4;
    private int TCPTimeout = 10000;

    private Dictionary<int, string> stationNames = new Dictionary<int, string>();

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Communicator"/> class. 
    /// May I have a new Communicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public LocalhostCommunicator(Station parent) {
      Contract.Requires(parent != null);
      this.Parent = parent;
      incomingQueue = new BlockingCollection<ICommand>(new ConcurrentQueue<ICommand>());
      outgoingQueue = new BlockingCollection<Tuple<IPEndPoint, ICommand, int>>(new ConcurrentQueue<Tuple<IPEndPoint, ICommand, int>>());
      initializeStationNames();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent { get; private set; }

    public bool ThreadsStarted { get; private set; }

    #endregion

    #region Private Methods and Operators

    private void initializeStationNames() {
      char c = 'A';
      for (int i = 50000; i < 65536; i += 1000) {
        stationNames.Add(i, c.ToString());
        c++;
      }
    }

    #endregion

    // TODO: review for problems with complexity
    #region Public Methods and Operators

    /// <summary>
    /// The discover network machines.
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    [Pure] public IEnumerable<IPEndPoint> DiscoverNetworkMachines() {
      var cdEvent = new CountdownEvent(1);

      // narrow down possible targets
      var potentials = new List<IPEndPoint>();

      using (var localSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
        try {
          bool done = false;
          byte[] buffer = new byte[32];
          localSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
          byte[] port_bytes = System.Text.Encoding.ASCII.GetBytes(((IPEndPoint)localSocket.LocalEndPoint).Port.ToString());
          EndPoint serverRemote = new IPEndPoint(IPAddress.Any, 0);
          localSocket.ReceiveTimeout = 4000;
          // ignore connection resets on the socket
          localSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
          for (int i = 50000; i < 65536; i += 1000) {
            localSocket.SendTo(port_bytes, new IPEndPoint(IPAddress.Loopback, i));
          }
          while (!done) {
            try {
              int length = localSocket.ReceiveFrom(buffer, ref serverRemote);
              int remotePort = 0;
              if (int.TryParse(System.Text.Encoding.ASCII.GetString(buffer, 0, length), out remotePort) &&
                  remotePort != localPort) {
                potentials.Add(new IPEndPoint(IPAddress.Loopback, remotePort));
                Console.WriteLine("got response from port " + remotePort);
              } else {
                Console.WriteLine("got response from unresolvable port");
              }
            } catch (SocketException e) {
              // the only way this happens is a timeout... but regardless, we're done
              done = true;
            }
          }
        } catch (Exception e) {
          if (Parent.Logger != null) {
            Parent.Logger.Log("Problem scanning local machine with exception " + e, Level.Info);
          } else {
            Console.WriteLine("Problem scanning local machine with exception " + e);
          }
        }
      }
      potentials.Remove(Parent.Address);
      Console.WriteLine("Found a total of " + potentials.Count() + " possible stations.");
      foreach (IPEndPoint p in potentials) {
        Console.WriteLine(p);
      }
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
    public IPEndPoint GetLocalEndPoint(int port)
    {
      int tcpPort = 0, udpPort = 0;

      if (tcpListener != null || udpSocket != null) {
        if (tcpListener != null) {
          tcpPort = ((IPEndPoint) tcpListener.LocalEndpoint).Port;
        }
        if (udpSocket != null) {
          udpPort = ((IPEndPoint) udpSocket.LocalEndPoint).Port;
        }
        if (udpPort != tcpPort) {
          throw new Exception("Listener Port Mismatch! TCP = " + tcpPort + ", UDP = " + udpPort);
        }
      }

      // at this point, either both listeners are running on the same port, or
      // neither is running and we need a new port number; also, tcpPort = udpPort.

      IPEndPoint ipep = null;

      if (tcpPort > 0)
      {
        ipep = new IPEndPoint(IPAddress.Loopback, tcpPort);
      }
      else
      {
        // start TCP on some port >= 50000, multiple of 1000; if it works, start
        // UDP on the same port. repeat until successful or we run out of ports to try

        for (int i = 50000; i < 65536; i += 1000)
        {
          ipep = new IPEndPoint(IPAddress.Loopback, i);

          try
          {
            tcpListener = new TcpListener(ipep);
            tcpListener.Start();
          }
          catch (SocketException)
          {
            // on to the next number
            tcpListener = null;
            ipep = null;
            continue;
          }

          try
          {
            udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            udpSocket.Bind(ipep);
            // ignore connection resets on the socket
            udpSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
          }
          catch (SocketException)
          {
            // on to the next number
            tcpListener.Stop();
            tcpListener = null;
            udpSocket = null;
            ipep = null;
            continue;
          }

          // if we get here, we created both sockets successfully!
          localPort = i;
          break;
        }

        if (ipep == null)
        {
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Unable to start network operations (no available ports).", Level.Fatal);
          }
          else
          {
            Console.WriteLine("Unable to start network operations (no available ports).");
          }

          throw new Exception("Fatal networking exception: no available ports.");
        }
      }

      if (Parent.Logger != null) {
        Parent.Logger.Log("Started listeners on port " + localPort, Level.Info);
      } else {
        Console.WriteLine("Started listeners on port " + localPort);
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
    /// The port number of this DVL, or 0 if no listeners are running.
    /// </returns>
    public string GetIdentifyingString() {
      return stationNames[localPort];
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
      return stationNames[address.Port];
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
          if (this.Parent.Logger != null) {
            this.Parent.Logger.Log("Could not connect to " + address + ", assuming down. " + e, Level.Error);
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
              if (this.Parent.Logger != null) {
                this.Parent.Logger.Log("Could not send IsAlive command to " + address + ", assuming down." + e, Level.Error);
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
        throw new Exception("Local endpoint has not yet been constructed.");
      }

      byte[] buffer = new byte[32];
      byte[] port_bytes = System.Text.Encoding.ASCII.GetBytes(Parent.Address.Port.ToString());
      EndPoint server = new IPEndPoint(IPAddress.Any, 0);
      int remotePort = 0;
      while (true) {
        try {
          int length = udpSocket.ReceiveFrom(buffer, ref server);
          remotePort = 0;
          if (!int.TryParse(System.Text.Encoding.ASCII.GetString(buffer, 0, length), out remotePort)) {
            if (Parent.Logger != null) {
              Parent.Logger.Log("pinged by unresolvable port.", Level.Info);
            } else {
              Console.WriteLine("pinged by unresolvable port.");
            }
            continue;
          } else {
            Console.WriteLine("pinged by port " + remotePort);
          }
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
        if (remotePort == 0) {
        }
        try {
          udpSocket.SendTo(port_bytes, new IPEndPoint(IPAddress.Loopback, remotePort));
        } catch (SocketException e) {
          if (this.Parent.Logger != null)
          {
            this.Parent.Logger.Log("Ping listener encountered recoverable exception " + e, Level.Error);
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
        throw new Exception("Local endpoint has not yet been constructed.");
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
          IPEndPoint origEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
          clientEndpoint = new IPEndPoint(IPAddress.Loopback, origEndpoint.Port);
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
            if (this.Parent.Logger != null && !(cmd is IsAliveCommand)) {
              this.Parent.Logger.Log("Received " + cmdType + " from " + cmd.Sender, Level.Info);
            } else {
              Console.WriteLine("Received " + cmdType + " from " + cmd.Sender);
            }
            // enqueue the command
            incomingQueue.Add(cmd);
            responseTimes[cmd.Sender] = DateTime.Now;
          } else {
            if (this.Parent.Logger != null) {
              this.Parent.Logger.Log(
                "Received " + cmdType + " from " + clientEndpoint + ". Shutting down.",
                Level.Fatal);
            } else {
              Console.WriteLine(
                "Received " + cmdType + " from " + clientEndpoint + ". Shutting down.");
            }

            this.Parent.ShutDownElection();
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
          if (this.Parent.Logger != null && !(command is IsAliveCommand)) {
            this.Parent.Logger.Log("Attempt #" + attempt + " to send " + commandType + " to " + target, Level.Info);
          }
          if (!(command is PublicKeyExchangeCommand || command is IsAliveCommand ||
                command is CryptoCommand || command is DisconnectStationCommand)) {
            if (Parent.Peers.ContainsKey(target)) {
              command = new CryptoCommand(this.Parent, target, command);
            } else {
              if (this.Parent.Logger != null) this.Parent.Logger.Log("Attempt to send a message to non-peer " + target, Level.Error);
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
            if (this.Parent.Logger != null) {
              this.Parent.Logger.Log("Problem sending due to SocketException " + e + ", retrying.", Level.Error);
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
          } else if (target.Equals(Parent.Manager)) {
            Console.WriteLine("Absent host was manager, attempting to elect new manager.");
            Parent.StartNewManagerElection();
          } else if (Parent.Peers.ContainsKey(target)) {
            Parent.RemovePeer(target);
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
