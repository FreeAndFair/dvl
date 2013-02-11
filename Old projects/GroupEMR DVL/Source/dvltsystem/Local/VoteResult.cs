namespace DVLTerminal.Local
{
    /// <summary>
    /// Defines the different outcomes of a proposed vote
    /// </summary>
    public enum VoteResult
    {
        /// <summary>
        /// The vote was accepted. Hand out ballot
        /// </summary>
        VoteOk, 
        /// <summary>
        /// The voter was validated against the local database. Needs response from network
        /// </summary>
        VoterValidated,
        /// <summary>
        /// The voter has already got a ballot
        /// </summary>
        HasVoted,
        /// <summary>
        /// The last four digits of the CPR number was not correct
        /// </summary>
        CPRCheckFailed,
        /// <summary>
        /// This response occours if the voter was not in the database
        /// This means that the votercard is either invalid, or this is the wrong voting place
        /// </summary>
        NotInDatabase,
        /// <summary>
        /// The vote has not yet been evaluated
        /// </summary>
        NotYetEvaluated
    }
}
