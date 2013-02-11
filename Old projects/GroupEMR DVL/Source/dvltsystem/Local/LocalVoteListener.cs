using System.Collections.Generic;
using System.Linq;
using System.Net;
using DVLTerminal.Networking;
using DVLTerminal.Utilities;

namespace DVLTerminal.Local
{
    /// <summary>
    /// The LocalVoteListener is used to maintain a list of connected peers,
    /// and the current vote registered from this machine.
    /// </summary>
    class LocalVoteListener : EventSystemExecutable
    {
        /// <summary>
        /// The running networklistener to register new handlers in
        /// </summary>
        private readonly NetworkListener networkListener;

        /// <summary>
        /// All connected peers and their state. This is passed to a Vote when created
        /// </summary>
        public Dictionary<IPAddress, PeerState> Peers { get; private set; }

        /// <summary>
        /// This is the current working Vote.
        /// When a new Vote is written here the LocalVoteListener will gather information and try to process the vote
        /// When the vote is done the result will be put in the vote, and the state of the vote will be 'Done'
        /// </summary>
        public Vote Vote { get; set; }

        /// <summary>
        /// Create a new local vote listener from a list of peers
        /// </summary>
        /// <param name="timeout">The timeout for use in the event system</param>
        /// <param name="networkListener">A networklistener to register created objects in</param>
        /// <param name="peers">A collection of connected peers. Will be updated if a peer is found to be dead</param>
        public LocalVoteListener(int timeout, NetworkListener networkListener, IEnumerable<IPAddress> peers) : base(timeout)
        {
            this.networkListener = networkListener;
            Peers = peers.ToDictionary(ip => ip, ip => PeerState.Alive);
        }

        /// <summary>
        /// Register that a peer is dead
        /// </summary>
        /// <param name="peer">The peer to save as dead</param>
        public void PeerIsDead(IPAddress peer)
        {
            if (Vote != null) //Need to be checked as a peer might die before a person votes on this computer
                Vote.SetPeerState(peer, PeerState.Dead);
            else
                Peers.Remove(peer);
        }

        /// <summary>
        /// Run the local vote listener making it look for new votes submitted on this machine, or finalize the current vote.
        /// </summary>
        protected override void Run()
        {
            if (Vote == null || Vote.VoteState == VoteState.Done) return; //Nothing to do, waiting for new vote!

            if (Vote.VoteState == VoteState.Initializing)
            {
                InitializeVote();
            }

            if (Vote.VoteState == VoteState.MessageSent && Vote.IsAcknowledged())
            {
                Vote.VoteResult = VoteResult.VoteOk;
                Vote.VoteState = VoteState.Done;
            }
        }

        /// <summary>
        /// Validate the submitted voter info
        /// </summary>
        /// <param name="voterNumber">The voter number of the person to validate</param>
        /// <param name="cprCheckNumber">The cpr check number (last 4 digits) of the person to validate</param>
        /// <returns>A VoteResult that describes the state of the information / voter</returns>
        private VoteResult ValidateVoteInfo(ulong voterNumber, ushort cprCheckNumber)
        {
            var db = Database.GetInstance;
            if (!db.ValidVoterNumber(voterNumber))
                return VoteResult.NotInDatabase;
            if (db.HasVoted(voterNumber))
                return VoteResult.HasVoted;
            if (!db.ValidCprCheckNumber(voterNumber, cprCheckNumber))
                return VoteResult.CPRCheckFailed;

            return VoteResult.VoterValidated;
        }

        /// <summary>
        /// Update the votestate and voteresult according to the local database and connected computers
        /// </summary>
        private void InitializeVote()
        {
            Vote.VoteResult = ValidateVoteInfo(Vote.VotingNumber, Vote.CPRCheckNumber);
            if (Vote.VoteResult != VoteResult.VoterValidated)
            {
                Vote.VoteState = VoteState.Done;
                return;
            }

            Database.GetInstance.SetVoted(Vote.VotingNumber, true);

            networkListener.RegisterHandler(new VoteReponseListener(networkListener, Vote), VoteReponseListener.Timeout);
            Vote.SendToAll();
            Vote.VoteState = VoteState.MessageSent;
        }
    }
}
