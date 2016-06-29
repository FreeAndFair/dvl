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
  using System.Net.NetworkInformation;
  using System.Net.Sockets;
  using System.Threading;
  using System.Threading.Tasks;

  using Aegis_DVL.Commands;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  /// <summary>
  /// The communicator.
  /// </summary>
  public class InternetCommunicator : IPCommunicator {
    public static readonly int DefaultPort = 62000;

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="InternetCommunicator"/> class. 
    /// May I have a new InternetCommunicator?
    /// </summary>
    /// <param name="parent">
    /// The parent station of the communicator.
    /// </param>
    public InternetCommunicator(Station parent) : base(parent) {
      Contract.Requires(parent != null);
      InitializeStationNames();
    }

    #endregion

    #region Private Methods and Operators

    private void InitializeStationNames() {
      char c = '0';
      char d = '0';
      for (int i = 0; i < 256; i++) {
        StationNames.Add(i, "0x" + c.ToString() + d.ToString());
        d++;
        if (d > '9' && !(d >= 'A')) {
          d = 'A';
        } else if (d > 'F') {
          d = '0';
          c++;
        }
      }
    }

    private string[] GetAllLocalIPv4() {
      List<string> ipAddrList = new List<string>();
      foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces()) {
        if (item.OperationalStatus == OperationalStatus.Up) {
          foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses) {
            if (ip.Address.AddressFamily == AddressFamily.InterNetwork) {
              string ipaddr = ip.Address.ToString();
              if (!ipaddr.Equals("127.0.0.1")) {
                ipAddrList.Add(ip.Address.ToString());
              }
            }
          }
        }
      }
      return ipAddrList.ToArray();
    }

    /// <summary>
    /// Gets the local IP address. This method prefers IP addresses on unroutable networks to
    /// IP addresses on routable networks, and will use the first such address it finds in 
    /// lexicographic order; if it finds no unroutable address, it will use the first
    /// routable address it finds in lexicographic order.
    /// </summary>
    /// <returns>The local IP address.</returns>
    private string GetMyLocalAddress() {
      string[] myips = GetAllLocalIPv4();
      Console.WriteLine("my ip addresses: " + string.Join(" ", myips));
      string myip = null;
      string chosen = null;
      Array.Sort(myips);
      foreach (string m in myips) {
        var bytes = IPAddress.Parse(m).GetAddressBytes();
        if ((bytes[0] == 10) ||
            ((bytes[0] == 192) && bytes[1] == 168) ||
            ((bytes[0] == 172) && ((bytes[1] & 0xf0) == 16))) {
          chosen = m;
          break;
        }
      }
      if (chosen == null) {
        chosen = myips[0];
      }
      Console.WriteLine("using address " + chosen);
      return chosen;
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
      string myip = GetMyLocalAddress();

      // narrow down possible targets
      var potentials = new List<IPEndPoint>();
      
      string myipprefix = myip.Substring(0, myip.LastIndexOf('.') + 1);
      using (var localSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)) {
        try {
          bool done = false;
          byte[] buffer = new byte[32];
          localSocket.Bind(new IPEndPoint(IPAddress.Any, 0));
          IPEndPoint server = new IPEndPoint(IPAddress.Any, 0);
          EndPoint serverRemote = (EndPoint) server;
          localSocket.ReceiveTimeout = 2000;
//            Console.WriteLine("sending broadcast packet to " + myipprefix + "255:62000");
//            localSocket.SendTo(new byte[] { 0 }, new IPEndPoint(IPAddress.Parse(myipprefix + "255"), 62000));
          byte[] port_bytes = System.Text.Encoding.ASCII.GetBytes(((IPEndPoint) localSocket.LocalEndPoint).Port.ToString());
          Console.WriteLine("Sending individual packets to all IPs in " + myipprefix + "0/24");
          for (int i = 1; i < 255; i++) {
            localSocket.SendTo(port_bytes, new IPEndPoint(IPAddress.Parse(myipprefix + i), 62000));
          }
          while (!done) {
            try {
              int length = localSocket.ReceiveFrom(buffer, ref serverRemote);
              IPEndPoint responder = new IPEndPoint(((IPEndPoint) serverRemote).Address, 62000);
              potentials.Add(responder);
              Console.WriteLine("got response from " + responder.Address);
            } catch (Exception) {
              Console.WriteLine("no more responses");
              done = true;
            }
          }
        } catch (Exception e) {
          Console.WriteLine("Exception when seaching for peers:" + e);
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
    public override IPEndPoint GetLocalEndPoint(int port) {
      LocalPort = port;
      string myip = GetMyLocalAddress();
      if (Parent.Logger != null) {
        Parent.Logger.Log("Creating network endpoint at " + myip + ":" + LocalPort, Level.Info);
      } else {
        Console.WriteLine("Creating network endpoint at " + myip + ":" + LocalPort);
      }
      IPEndPoint ipep = new IPEndPoint(
          IPAddress.Parse(myip),
          port);
      try {
        if (TcpListener == null) {
          TcpListener = new TcpListener(ipep);
          TcpListener.Start();
        }

        if (UdpSocket == null) {
          UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
          UdpSocket.Bind(ipep);
        }
      } catch (Exception e) {
        if (Parent.Logger != null) {
          Parent.Logger.Log("Problem starting network operations with exception " + e, Level.Info);
        } else {
          Console.WriteLine("Problem starting network operations with exception " + e);
        }
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
      string result = "UNKNOWN";

      try {
        result = GetIdentifyingStringForStation(Parent.Address);
      } catch (Exception) {
        // return UNKNOWN
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
    public override string GetIdentifyingStringForStation(IPEndPoint address) {
      String s = address.Address.ToString();
      int lastQuad = s.LastIndexOf('.') + 1;
      return StationNames[Int32.Parse(s.Substring(lastQuad, s.Length - lastQuad))];
    }

    #endregion
  }
}
