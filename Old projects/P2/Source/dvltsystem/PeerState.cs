namespace DVLTerminal
{
    /// <summary>
    /// Defines the different states that this computer can know a peer to be in
    /// </summary>
    public enum PeerState
    {
        /// <summary>
        /// The peer is assumed to be alive
        /// </summary>
        Alive,
        /// <summary>
        /// The peer might be dead, an invesigation is started
        /// </summary>
        MaybeDead, 
        /// <summary>
        /// The peer is dead, remove it from the lists
        /// </summary>
        Dead
    }
}
