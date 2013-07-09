namespace DVLTerminal.Networking
{
    /// <summary>
    /// Defines the different kind of messages that can exist
    /// </summary>
    public enum MessageType : byte
    {
        /// <summary>
        /// A message of unknown type. Could be a result of a bad package, or a wrong key used for decryption.
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// A message saying that i'm here, and i want to know who's there
        /// MessageContent: Computer name
        /// </summary>
        HeartbeatRequest = 1,
        /// <summary>
        /// A message saying that i'm here
        /// MessageContent: Computer name
        /// </summary>
        Heartbeat = 2,
        /// <summary>
        /// A message saying that a given person has voted
        /// MessageContent: VoterNumber
        /// </summary>
        Vote = 3, 
        /// <summary>
        /// A response message as a reply to a Vote message.
        /// MessageContent: VoterNumber
        /// </summary>
        VoteOk = 4, 
        /// <summary>
        /// A request that this computer should ping that computer
        /// MessageContent: IP to ping
        /// </summary>
        PingRequest = 5, 
        /// <summary>
        /// A message asking if the recipient is alive
        /// MessageContent: nothing
        /// </summary>
        Ping = 6, 
        /// <summary>
        /// A message saying that the ping was successfull. Could be the reply to a PingRequest
        /// MessageContent: IP that was pinged
        /// </summary>
        PingOk = 7,
        /// <summary>
        /// A message saying that the ping was unsuccessfull. Could be the reply to a PingRequest
        /// MessageContent: IP that was pinged
        /// </summary>
        PingFailed = 8,
        /// <summary>
        /// A message saying i'm alive
        /// MessageContent: nothing
        /// </summary>
        Pong = 9, 
        /// <summary>
        /// A message saying that a given peer is dead
        /// MessageContent: Peer IP
        /// </summary>
        PeerDead = 10,
        /// <summary>
        /// A message containing the key and IV used for encryption
        /// MessageContent: Key Length, Key and IV
        /// </summary>
        EncryptionPacket = 11
    }
}
