namespace DVLTerminal.Networking
{
    /// <summary>
    /// Implementing classes are able to receive packets from network, or timeout after a given time.
    /// </summary>
    public interface INetworkPacketHandler
    {
        /// <summary>
        /// Handle an incoming message
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>Whether or not the message could be handled, and whether to continue listening for messages</returns>
        PacketResponse HandlePacket(Message message);

        /// <summary>
        /// Handle that the object has timed out
        /// This method will only be called once, as each object only can time out once.
        /// </summary>
        void TimedOut();
    }
}
