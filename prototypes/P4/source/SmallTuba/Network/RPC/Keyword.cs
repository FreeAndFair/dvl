namespace SmallTuba.Network.RPC {

	/// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// Describing the action to be performed by the server
    /// </summary>
    internal enum Keyword {
        /// <summary>
        /// Are you a request about getting a person from a cpr number?
        /// </summary>
        GetPersonFromCpr,
        
        /// <summary>
        /// Are you a request about getting a person from an ID?
        /// </summary>
        GetPersonFromId,
        
        /// <summary>
        /// Are you a request about registering a voter?
        /// </summary>
        RegisterVoter,

        /// <summary>
        /// Are you a request about unregistering a voter?
        /// </summary>
        UnregisterVoter,

        /// <summary>
        /// Are you a request about valid tables?
        /// </summary>
        ValidTables,

        /// <summary>
        /// Are you connected to a server?
        /// </summary>
        Ping
    }
}
