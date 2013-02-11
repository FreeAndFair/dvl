namespace DVLTerminal.Networking
{
    /// <summary>
    /// A type enum to indicate whether a handler is able to handle a given package
    /// </summary>
    public enum PacketResponse
    {
        /// <summary>
        /// I would like to recieve this package
        /// </summary>
        Accept,
        /// <summary>
        /// I would not like to recieve this package
        /// </summary>
        Decline,
        /// <summary>
        /// I would like to recieve this package, and I am no longer needed - remove me
        /// </summary>
        AcceptAndFinish
    }
}