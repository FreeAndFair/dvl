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
  public class LocalhostCommunicator : IPCommunicator {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ICommunicator"/> class. 
    /// May I have a new Communicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public LocalhostCommunicator(Station parent) : base(parent) {
      Contract.Requires(parent != null);
      InitializeStationNames();
    }

    #endregion

    #region Public Properties

    #endregion

    #region Private Methods and Operators

    private void InitializeStationNames() {
      Console.WriteLine("At InitializeStatementNames, Parent is Null = " + (Parent == null));
      char c = 'A';
      for (int i = 50000; i < 65536; i += 1000) {
        StationNames.Add(i, c.ToString());
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
    [Pure] public override IEnumerable<IPEndPoint> DiscoverPeers() {
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
                  remotePort != LocalPort) {
                potentials.Add(new IPEndPoint(IPAddress.Loopback, remotePort));
                Console.WriteLine("got response from port " + remotePort);
              } else {
                Console.WriteLine("got response from unresolvable port");
              }
            } catch (SocketException) {
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
      Console.WriteLine("Found a total of " + potentials.Count + " possible stations.");
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
    public override IPEndPoint GetLocalEndPoint(int port)
    {
      int tcpPort = 0, udpPort = 0;

      if (TcpListener != null || UdpSocket != null) {
        if (TcpListener != null) {
          tcpPort = ((IPEndPoint) TcpListener.LocalEndpoint).Port;
        }
        if (UdpSocket != null) {
          udpPort = ((IPEndPoint) UdpSocket.LocalEndPoint).Port;
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
            TcpListener = new TcpListener(ipep);
            TcpListener.Start();
          }
          catch (SocketException)
          {
            // on to the next number
            TcpListener = null;
            ipep = null;
            continue;
          }

          try
          {
            UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            UdpSocket.Bind(ipep);
            // ignore connection resets on the socket
            UdpSocket.IOControl((int)SIO_UDP_CONNRESET, new byte[] { Convert.ToByte(false) }, null);
          }
          catch (SocketException)
          {
            // on to the next number
            TcpListener.Stop();
            TcpListener = null;
            UdpSocket = null;
            ipep = null;
            continue;
          }

          // if we get here, we created both sockets successfully!
          LocalPort = i;
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
        Parent.Logger.Log("Started listeners on port " + LocalPort, Level.Info);
      } else {
        Console.WriteLine("Started listeners on port " + LocalPort);
      }

      return ipep;
    }

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify this DVL.
    /// </summary>
    /// <returns>
    /// The port number of this DVL, or 0 if no listeners are running.
    /// </returns>
    public override string GetIdentifyingString() {
      return StationNames[LocalPort];
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
    public override string GetIdentifyingStringForStation(IPEndPoint address) {
      return StationNames[address.Port];
    }

    #endregion
  }
}
