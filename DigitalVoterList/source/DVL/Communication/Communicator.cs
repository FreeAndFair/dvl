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
  public class Communicator : ICommunicator {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Communicator"/> class. 
    /// May I have a new Communicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public Communicator(Station parent) {
      Contract.Requires(parent != null);
      this.Parent = parent;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent { get; private set; }

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
      var res = new List<IPEndPoint>();
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
          byte[] buffer = new byte[16];
          IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);
          EndPoint serverRemote = (EndPoint) server;
          udpSocket.ReceiveTimeout = 8000;
          udpSocket.SendTo(new byte[] { 0 }, new IPEndPoint(IPAddress.Parse(myipprefix + "255"), 62000));
          while (!done) {
            try {
              udpSocket.ReceiveFrom(buffer, ref serverRemote);
              IPEndPoint responder = new IPEndPoint(IPAddress.Parse(System.Text.Encoding.ASCII.GetString(buffer)), 62000);
              potentials.Add(responder);
              Console.WriteLine("got response from " + responder.Address);
            } catch (Exception e) {
              done = true;
            }
          }
        } catch (Exception e) {
          // something went wrong so fall back to the old way of doing things
          for (int i = 1; i < 255; i++) {
            potentials.Add(new IPEndPoint(IPAddress.Parse(myipprefix + i), 62000));
          }
        }
      }

      potentials.Remove(new IPEndPoint(IPAddress.Parse(myip), 62000));
      Console.WriteLine(potentials.Count + " possible stations found");

      /*
      using (cdEvent) {
        int i = 0;
        foreach (IPEndPoint endpoint in potentials) {
          cdEvent.AddCount();
          ThreadPool.QueueUserWorkItem(
            element => {
              var elem = (Tuple<int, CountdownEvent>)element;
              if (this.IsListening(endpoint)) {
                Console.WriteLine("Found a station at " + endpoint.Address);
                res.Add(endpoint);
              }
              elem.Item2.Signal();
            }, 
            new Tuple<int, CountdownEvent>(i, cdEvent));
          i++;
        }

        cdEvent.Signal();
        cdEvent.Wait();
      }

      cdEvent.Dispose();
      */

      foreach (IPEndPoint endpoint in potentials) {
        Console.WriteLine("Checking for station at " + endpoint.Address);
        if (IsListening(endpoint)) {
          Console.WriteLine("Found station at " + endpoint.Address);
          res.Add(endpoint);
        } else {
          Console.WriteLine("No response at " + endpoint.Address);
        }
      }

      Console.WriteLine("Found a total of " + res.Count() + " stations.");
      return res;
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
      try {
        this.Send(new IsAliveCommand(this.Parent.Address), address);
      } catch (SocketException) {
        return false;
      }

      return true;
    }

    public void HandlePing() {
      IPAddress myip = Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip =>
                    ip.AddressFamily == AddressFamily.InterNetwork);
      using (var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
        byte[] buffer = new byte[16];
        byte[] myipbytes = System.Text.Encoding.ASCII.GetBytes(myip.ToString());
        EndPoint server = new IPEndPoint(IPAddress.Any, 0);

        udpSocket.Bind(new IPEndPoint(myip, 62000));
        try {
          udpSocket.ReceiveFrom(buffer, ref server);
          IPEndPoint ips = (IPEndPoint)server;
          if (!ips.Address.Equals(myip)) {
            udpSocket.SendTo(myipbytes, server);
            Console.WriteLine("pinged by " + ips.Address);
          }
        } catch (Exception e) {
          // we timed out, so no more packets
        }
      }
    }

    /// <summary>
    /// The receive and handle.
    /// </summary>
    /// TODO: review for problems with complexity
    public void ReceiveAndHandle() {
      var listener = new TcpListener(this.Parent.Address);
      listener.Start();
      try {
        using (TcpClient client = listener.AcceptTcpClient())
        using (NetworkStream stream = client.GetStream()) {
          byte[] bytes = Bytes.FromNetworkStream(stream);
          if (bytes.Length == 0) return;
          var cmd = bytes.To<ICommand>();
          if (cmd is PublicKeyExchangeCommand ||
              cmd is CryptoCommand ||
              cmd is IsAliveCommand) {
            if (this.Parent.Logger != null &&
                !(cmd is IsAliveCommand)) this.Parent.Logger.Log("Received " + cmd.GetType() + " from " + cmd.Sender, Level.Info);
            cmd.Execute(this.Parent);
          } else {
            if (this.Parent.Logger != null) {
              this.Parent.Logger.Log(
                "Received a command that wasn't PublicKeyExchangeCommand, CryptoCommand or IsAliveCommand from " +
                client.Client.RemoteEndPoint + ". Shutting down.", 
                Level.Fatal);
            }

            this.Parent.ShutDownElection();
          }
        }
      } finally {
        listener.Stop();
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
      if (this.Parent.Logger != null &&
          !(command is IsAliveCommand)) this.Parent.Logger.Log("Attempting to send " + command.GetType() + " to " + target, Level.Info);
      bool isBallotReceived = command is BallotReceivedCommand;

      if (!(command is PublicKeyExchangeCommand || command is IsAliveCommand || command is CryptoCommand)) command = new CryptoCommand(this.Parent, target, command);
      try {
        using (var client = new TcpClient()) {
          IAsyncResult ar = client.BeginConnect(target.Address, target.Port, null, null);
          WaitHandle wh = ar.AsyncWaitHandle;
          try {
            if (!ar.AsyncWaitHandle.WaitOne(TimeSpan.FromMilliseconds(1000), false)) throw new SocketException();
            client.EndConnect(ar);
          } finally {
            wh.Close();
          }

          byte[] bytes = Bytes.From(command);
          const int packetSize = 2048;
          var packetAmount = (int)Math.Ceiling((double)bytes.Length / packetSize);
          using (NetworkStream stream = client.GetStream()) {
            for (int i = 0; i < packetAmount; i++) {
              int offset = i * packetSize;
              int size = i == packetAmount - 1 ? bytes.Length % packetSize : packetSize;
              stream.Write(bytes, offset, size);
            }
          }
        }
      } catch (SocketException) {
        if (command is IsAliveCommand || isBallotReceived) throw;
        if (this.Parent.Logger != null) this.Parent.Logger.Log("SocketException thrown while sending, attempting to handle.", Level.Error);
        if (this.Parent.IsManager &&
            this.Parent.Peers.ContainsKey(target)) this.Parent.AnnounceRemovePeer(target);
        else {
          if (target.Equals(this.Parent.Manager)) {
            this.Parent.StartNewManagerElection();
            this.Send(command, target);
          } else
            if (this.Parent.Peers.ContainsKey(target)) this.Parent.RemovePeer(target);
        }
      } catch (IOException) {
        if (this.Parent.Logger != null) this.Parent.Logger.Log("Problem sending due to IOException, retrying send.", Level.Error);
        this.Send(command, target);
      }
    }

    #endregion
  }
}
