namespace SmallTuba.Network.UDP {
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Serialization.Formatters.Binary;

    /// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// Class used for multicasting data
    /// </summary>
    public class UdpMulticast {
        /// <summary>
        /// The Udpclient that wraps all the socket programming nicely
        /// </summary>
        private readonly UdpClient client;

        /// <summary>
        /// Where we send the packages to
        /// </summary>
        private readonly IPEndPoint sendPoint;

        /// <summary>
        /// Where the packages come from
        /// </summary>
        private IPEndPoint receivePoint;

        /// <summary>
        /// May i have a new multicast client with this choice of server or client setting?
        /// The client listens to ip 224.5.6.7 port 5000
	    /// The client sends to ip 224.5.6.7 port 5001
	    /// The server listens to ip 224.5.6.7 port 5001
	    /// The server sends to ip 224.5.6.7 port 5000
        /// </summary>
        /// <param name="server">If the client should be server - 0 or client - 1</param>
        public UdpMulticast(int server) {
            Contract.Requires(server == 0 || server == 1);
            
            // Creates a new client initialized for port 5000/5001
            this.client = new UdpClient(5000 + 1 - server);
            
            // The multicast adress
            IPAddress ip = IPAddress.Parse("224.5.6.7"); 
            
            // Where we send the packages to
            this.sendPoint = new IPEndPoint(ip, 5000 + server);
            
            // Where we recieve them from
            this.receivePoint = null;
            
            // How far to multicast
            this.client.Ttl = 2;

            // Join the multicast group
            this.client.JoinMulticastGroup(ip);
        }

        /// <summary>
        /// Send this object
        /// </summary>
        /// <param name="o">The object to send</param>
        public void Send(object o) {
            Contract.Requires(o != null);
            var bf = new BinaryFormatter();
            var ms = new MemoryStream();
            bf.Serialize(ms, o);
            var data = ms.ToArray();
            this.client.Send(data.ToArray(), data.Length, this.sendPoint);
        }

        /// <summary>
        /// May I have an object if one is in queue or arrives within this timeframe?
        /// </summary>
        /// <param name="timeOut">The time to wait in milliseconds</param>
        /// <returns>The received object, null if it times out</returns>
        public object Receive(long timeOut) {
            Contract.Requires(timeOut >= 0);

            // The initial time
            var preTime = DateTime.Now.ToFileTime();
            
            // Listen forever if timeout was 0, or until the time has expired
            while (timeOut == 0 || DateTime.Now.ToFileTime() < preTime + (timeOut * 10000)) {
                // If packages are available
                if (this.client.Available > 0) {
                    // Receive the next as a byte array
                    byte[] data = this.client.Receive(ref this.receivePoint);
                    var ms = new MemoryStream();
                    var bf = new BinaryFormatter();
                    ms.Write(data, 0, data.Length);
                    ms.Seek(0, SeekOrigin.Begin);
                    
                    // Try to deserialize the data
                    try {
                        return bf.Deserialize(ms);
                    }
                    catch (System.Runtime.Serialization.SerializationException) {
                        // The received input was not an object
                        // Maybe Another application is using this port
                    }
                }
                else {
                    // Sleeps for 10 miliseconds
                    System.Threading.Thread.Sleep(10);
                }
            }

            // The time has run out
            return null;
        }
    }
}
