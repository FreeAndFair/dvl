namespace DVLTerminal.Networking
{
    /// <summary>
    /// Handles an incomping Ping packet
    /// </summary>
    class PingListener : INetworkPacketHandler
    {
        /// <summary>
        /// Handle an incomping Ping packet
        /// </summary>
        /// <param name="message">The ping message to process</param>
        /// <returns>Accept if it's a ping message, Decline otherwise</returns>
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.Ping)
                return PacketResponse.Decline;

            var pong = new Message(MessageType.Pong, message.Sender);
            Outbox.SendMessage(pong);

            return PacketResponse.Accept;
        }


        public void TimedOut()
        {
            System.Diagnostics.Debug.Assert(false);
            //Nothing to do as the PingListener must never timeout!
        }
    }
}
