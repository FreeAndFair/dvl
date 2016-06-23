using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Aegis_DVL.Commands;
using Aegis_DVL.Data_Types;
using Aegis_DVL.Util;

namespace Aegis_DVL.Communication
{
  public abstract class IPCommunicator : ICommunicator
  {
    private static readonly uint IOC_IN = 0x80000000;
    private static readonly uint IOC_VENDOR = 0x18000000;
    protected static readonly uint SIO_UDP_CONNRESET = IOC_IN | IOC_VENDOR | 12;

    private CancellationTokenSource _cancel = new CancellationTokenSource();
    private BlockingCollection<ICommand> _incomingQueue;
    private BlockingCollection<Tuple<IPEndPoint, ICommand, int>> _outgoingQueue;
    private int _maxTries = 5;
    private Thread _receiveThread;
    private Thread _sendThread;
    private Thread _dequeueThread;
    private Thread _pingThread;
    private readonly Dictionary<IPEndPoint, DateTime> _responseTimes = new Dictionary<IPEndPoint, DateTime>();
    private int _lengthOfCount = 4;
    private int _tcpTimeout = 10000;

    protected TcpListener TcpListener;
    protected Socket UdpSocket;
    protected int LocalPort; 
    protected readonly Dictionary<int, string> StationNames = new Dictionary<int, string>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ICommunicator"/> class. 
    /// May I have a new Communicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public IPCommunicator(Station parent) {
      Contract.Requires(parent != null);
      Parent = parent;
      _incomingQueue = new BlockingCollection<ICommand>(new ConcurrentQueue<ICommand>());
      _outgoingQueue = new BlockingCollection<Tuple<IPEndPoint, ICommand, int>>(new ConcurrentQueue<Tuple<IPEndPoint, ICommand, int>>());
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent { get; private set; }

    public bool ThreadsStarted { get; private set; }

    /// <summary>
    /// Destroys the local end point for this instance of the pollbook.
    /// </summary>
    /// <returns>
    /// The IPEndPoint representing the local end point.
    /// </returns>
    private void DestroyLocalEndPoint() {
      Contract.Ensures(TcpListener == null);
      Contract.Ensures(UdpSocket == null);
      if (TcpListener != null) {
        try {
          TcpListener.Stop();
          if (Parent.Logger != null) {
            Parent.Logger.Log("Stopped TCP listener on port " + LocalPort, Level.Info);
          } else {
            Console.WriteLine("Stopped TCP listener on port " + LocalPort);
          }
        } catch (Exception) {
          // ignored
        }
        TcpListener = null;
      }

      if (UdpSocket != null) {
        try {
          UdpSocket.Close();
          if (Parent.Logger != null) {
            Parent.Logger.Log("Stopped UDP listener on port " + LocalPort, Level.Info);
          } else {
            Console.WriteLine("Stopped UDP listener on port " + LocalPort);
          }
        } catch (Exception) {
          // ignored
        }
        UdpSocket = null;
      }

      LocalPort = 0;
    }

    public abstract string GetIdentifyingStringForStation(IPEndPoint ipEndPoint);
    public abstract string GetIdentifyingString();
    public abstract IPEndPoint GetLocalEndPoint(int port);
    public abstract IEnumerable<IPEndPoint> DiscoverPeers();

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

      if (_responseTimes.ContainsKey(address) &&
          DateTime.Now.Subtract(_responseTimes[address]).TotalSeconds < 30) {
            // we've heard from this station within the last 30 seconds
          } else {
            // we haven't heard from this station in 30 seconds, or ever, let's ping it
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
      if (UdpSocket == null) {
        throw new Exception("Local endpoint has not yet been constructed.");
      }

      byte[] buffer = new byte[32];
      byte[] portBytes = System.Text.Encoding.ASCII.GetBytes(Parent.Address.Port.ToString());
      EndPoint server = new IPEndPoint(IPAddress.Any, 0);
      while (!_cancel.IsCancellationRequested) {
        int remotePort = 0;
        try {
          int length = UdpSocket.ReceiveFrom(buffer, ref server);
          remotePort = 0;
          if (!int.TryParse(System.Text.Encoding.ASCII.GetString(buffer, 0, length), out remotePort)) {
            if (Parent.Logger != null) {
              Parent.Logger.Log("pinged by unresolvable port.", Level.Info);
            } else {
              Console.WriteLine("pinged by unresolvable port.");
            }
            continue;
          } else {
            Console.WriteLine("pinged by " + ((IPEndPoint) server).Address.ToString() + ":" + remotePort);
          }
        } catch (SocketException e) {
          if (e.SocketErrorCode == SocketError.TimedOut) {
            break;
          } else {
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
        }
        try {
          UdpSocket.SendTo(portBytes, new IPEndPoint(((IPEndPoint) server).Address, remotePort));
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
      if (TcpListener == null) {
        throw new Exception("Local endpoint has not yet been constructed.");
      }

      while (!_cancel.IsCancellationRequested) {
        try {
          TcpClient client = TcpListener.AcceptTcpClient();
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
      client.ReceiveTimeout = _tcpTimeout;
      IPEndPoint clientEndpoint = null;
      try {
        using (NetworkStream stream = client.GetStream()) {
          clientEndpoint = (IPEndPoint)client.Client.RemoteEndPoint;
          if (Parent.Logger != null) {
            Parent.Logger.Log("Handling connection from " + clientEndpoint, Level.Info);
          } else {
            Console.WriteLine("Handling connection from " + clientEndpoint);
          }
          client.Client.ReceiveTimeout = _tcpTimeout;
          byte[] lengthBytes = Bytes.FromNetworkStreamWithSize(stream, _lengthOfCount);
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
            stream.Write(new byte[_lengthOfCount], 0, _lengthOfCount);
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
                _incomingQueue.Add(cmd);
                _responseTimes[cmd.Sender] = DateTime.Now;
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
      Send(command, target, _maxTries);
    }

    private void Send(ICommand command, IPEndPoint target, int tries) {
      _outgoingQueue.Add(Tuple.Create<IPEndPoint, ICommand, int>(target, command, tries));
    }

    public void ReceivedMessageDequeueThread() {
      bool running = true;
      while (running) {
        ICommand command = null;
        try {
          command = _incomingQueue.Take(_cancel.Token);
        } catch (Exception e) {
          // the only way this can happen is if something has gone wrong or we were cancelled
          if (Parent.Logger != null)
          {
            Parent.Logger.Log("Dequeue thread died with exception " + e, Level.Info);
          }
          else
          {
            Console.WriteLine("Dequeue thread died with exception " + e);
          } 
          running = false;
        }
        if (command != null) {
          command.Execute(Parent);
        }
      }
    }

    public void MessageSendThread() {
      while (!_cancel.IsCancellationRequested || _outgoingQueue.Count > 0) {
        Tuple<IPEndPoint, ICommand, int> message;
        try {
          message = _outgoingQueue.Take(_cancel.Token);
        } catch (Exception e) {
          // we still send messages until we are done
          if (_outgoingQueue.Count > 0) {
            message = _outgoingQueue.Take();
          } else {
            break;
          }
        }
        IPEndPoint target = message.Item1;
        ICommand command = message.Item2;
        int tries = message.Item3;
        int attempt = _maxTries - tries + 1;
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
          client.ReceiveTimeout = _tcpTimeout;
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
          } else {
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
        _cancel.Cancel();

        // don't abort the send thread until the sending queue is empty
        _outgoingQueue.CompleteAdding();
        while (_outgoingQueue.Count > 0) {
          // this busy wait is bad, but fixing the entire system would likely be much harder
          Thread.Sleep(1000);
          if (Parent.Logger != null) {
            Parent.Logger.Log("Waiting for " + _outgoingQueue.Count + " messages to be sent.", Level.Info);
          } else {
            Console.WriteLine("Waiting for " + _outgoingQueue.Count + " messages to be sent.");
          }          
        }

        DestroyLocalEndPoint();
        _receiveThread.Join();
        _receiveThread = null;
        _dequeueThread.Join();
        _dequeueThread = null;
        _sendThread.Join();
        _sendThread = null;
        _pingThread.Join();
        _pingThread = null;
        _cancel = new CancellationTokenSource();
      }

      ThreadsStarted = false;
    }

    public void StartThreads() {
      if (!ThreadsStarted) {
        _receiveThread = new Thread(AcceptConnectionThread);
        _receiveThread.SetApartmentState(ApartmentState.STA);
        _receiveThread.Name = "Receive";
        _dequeueThread = new Thread(ReceivedMessageDequeueThread);
        _dequeueThread.SetApartmentState(ApartmentState.STA);
        _dequeueThread.Name = "Dequeue";
        _sendThread = new Thread(MessageSendThread);
        _sendThread.SetApartmentState(ApartmentState.STA);
        _sendThread.Name = "Send";
        _pingThread = new Thread(PingThread);
        _pingThread.SetApartmentState(ApartmentState.STA);
        _pingThread.Name = "PingResponder";

        _receiveThread.Start();
        _dequeueThread.Start();
        _sendThread.Start();
        _pingThread.Start();
      }

      ThreadsStarted = true;
    }
  }
}