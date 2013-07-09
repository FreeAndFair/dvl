namespace DVLTerminal.Networking
{
    /// <summary>
    /// Defines the different responses that can be received as answer to a ping request
    /// </summary>
    public enum PingRequestResponse
    {
        /// <summary>
        /// The other computer answered that the suspected peer is alive
        /// </summary>
        Yes,
        /// <summary>
        /// The other computer answered that the suspected peer is dead
        /// </summary>
        No, 
        /// <summary>
        /// The computer has not yet answered
        /// </summary>
        NotYet
    }
}
