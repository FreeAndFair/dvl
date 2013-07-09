using System.Net;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Listens for Ping Reqeust packets from the network.
    /// When a Ping Request is received it initiates a Pong listener which will respond, and sends a ping to the given peer
    /// </summary>
    class PingRequestListener : INetworkPacketHandler
    {
        /// <summary>
        /// The current NetworkListener in which new handlers can be registered
        /// </summary>
        private readonly NetworkListener networkListener;

        /// <summary>
        /// Create a new PingRequest listener to listen for PingRequests
        /// </summary>
        /// <param name="networkListener">The NetworkListener to register the created PongListeners in</param>
        public PingRequestListener(NetworkListener networkListener)
        {
            this.networkListener = networkListener;
        }

        /// <summary>
        /// Handles a PingRequest packet by creating a <see cref="PongListener">PongListener</see> and sending a ping to the suspect
        /// </summary>
        /// <param name="pingRequest">The received PingRequest object</param>
        /// <returns>Accept or Decline depending on what type of packet was received</returns>
        public PacketResponse HandlePacket(Message pingRequest)
        {
            if (pingRequest.MessageType != MessageType.PingRequest)
                return PacketResponse.Decline;

            IPAddress peerToPing;
            var valid = IPAddress.TryParse(pingRequest.DataAsString, out peerToPing);
            if (!valid)
            {
                pingRequest.DeemInvalid();
                return PacketResponse.Decline;
            }

            var pongListener = new PongListener(pingRequest.Sender, peerToPing);
            networkListener.RegisterHandler(pongListener, PongListener.Timeout);

            var ping = new Message(MessageType.Ping, peerToPing);
            Outbox.SendMessage(ping);

            return PacketResponse.Accept;
        }

        /// <summary>
        /// Shouldn't time out
        /// </summary>
        public void TimedOut()
        {
            System.Diagnostics.Debug.Assert(false);
            //Nothing to do as the PingRequestListener must never timeout!
        }
    }
}
