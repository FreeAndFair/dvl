using System.Net;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Listens for Pong messages and sends a PingOk back to initiator of the PingRequest that created this PongListener.
    /// On time out it sends a PingFailed back to initiator.
    /// </summary>
    /// Author: Michael Oliver Urhøj Mortensen
    class PongListener : INetworkPacketHandler
    {
        /// <summary>
        /// The IPAddress of the peer that initiated the ping request leading to this PongListener
        /// </summary>
        private readonly IPAddress initiator;

        /// <summary>
        /// The IPAddress of the peer that is suspected to be dead
        /// </summary>
        private readonly IPAddress suspect;

        /// <summary>
        /// For how long to wait for a pong response before timing out
        /// </summary>
        public const int Timeout = 250;

        /// <summary>
        /// Constructs a new PongListener, saving the initiating client's IP
        /// (the client that initiated the "is X alive?" broadcast).
        /// </summary>
        /// <param name="initiator">The IPAddress of the initiating client</param>
        /// <param name="suspect">The IPAddress of the peer that is suspected to be dead</param>
        public PongListener(IPAddress initiator, IPAddress suspect)
        {
            this.initiator = initiator;
            this.suspect = suspect;
        }

        /// <summary>
        /// Handle an incomping Pong packet
        /// </summary>
        /// <param name="message">The ping message to process</param>
        /// <returns>Accept if it's a ping message, Decline otherwise</returns>
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.Pong || !message.Sender.Equals(suspect))
                return PacketResponse.Decline;

            var pingOK = new Message(MessageType.PingOk, initiator) {DataAsString = suspect.ToString()};
            Outbox.SendMessage(pingOK);

            return PacketResponse.AcceptAndFinish;
        }

        /// <summary>
        /// The pong listener times out when it doesn't receive a pong. This will be reported to the initiator
        /// </summary>
        public void TimedOut()
        {
            Message pingFailed = new Message(MessageType.PingFailed, initiator) {DataAsString = suspect.ToString()};
            Outbox.SendMessage(pingFailed);
        }
    }
}
