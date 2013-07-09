using System.Collections.Generic;
using System.Linq;
using System.Net;
using DVLTerminal.Networking;

namespace DVLTerminal.Local
{
    /// <summary>
    /// A Vote object contains information about a vote from the local computer, and
    /// provides methods for distributing the vote to the connected peers
    /// </summary>
    public class Vote
    {
        /// <summary>
        /// Edits the current VoteResult.
        /// </summary>
        public VoteResult VoteResult { get; set; }
        
        /// <summary>
        /// The current VoteState.
        /// Must never be able to leave the VoteState "Done", hence the assert in
        /// the property bellow.
        /// </summary>
        private VoteState voteState;
        
        /// <summary>
        /// Gets and sets the VoteState.
        /// </summary>
        public VoteState VoteState
        {
            get { return voteState; }
            set
            {
                System.Diagnostics.Debug.Assert(VoteState != VoteState.Done, "Votestate leaved done");
                voteState = value;
            }
        }

        /// <summary>
        /// Get the reply list for this vote
        /// </summary>
        public Dictionary<IPAddress, bool> VoteReplyList
        {
            get { return voteReplyList; }
        }
        
        /// <summary>
        /// The voting number of the person.
        /// </summary>
        public ulong VotingNumber { get; private set; }
        
        /// <summary>
        /// The last four digits of the CPR-number of the person.
        /// </summary>
        public ushort CPRCheckNumber { get; private set; }

        /// <summary>
        /// A dictionary of the current peers and their state.
        /// </summary>
        private readonly Dictionary<IPAddress, PeerState> peers;

        /// <summary>
        /// A dictionary of the peers and whether or not their have send a 
        /// "VoteOK" reply.
        /// </summary>
        private readonly Dictionary<IPAddress, bool> voteReplyList;

        /// <summary>
        /// Create a new vote.
        /// </summary>
        /// <param name="votingNumber">The voting number of the voter</param>
        /// <param name="cprCheckNumber">The cpr check number of the voter</param>
        /// <param name="peers">A dictionary of connected peers. Will be updated if a peer is discovered to be disconnected</param>
        public Vote(ulong votingNumber, ushort cprCheckNumber, Dictionary<IPAddress, PeerState> peers)
        {
            VotingNumber = votingNumber;
            CPRCheckNumber = cprCheckNumber;
            this.peers = peers;
            voteReplyList = peers.ToDictionary(peer => peer.Key, peer => false);
            VoteState = VoteState.Initializing;
        }
        
        /// <summary>
        /// Set the state of a peer.
        /// Setting the state in the vote updates the state for the peer in the entire system
        /// </summary>
        /// <param name="peer">The peer to update</param>
        /// <param name="state">The new state. Notice that setting this to Dead will remove the peer!</param>
        public void SetPeerState(IPAddress peer, PeerState state)
        {
            if (state == PeerState.Dead)
            {
                peers.Remove(peer);
                voteReplyList.Remove(peer);
                if (peers.Count < Program.MinimumPeerCount) 
                {
                    throw new NotEnoughPeersException(peers.Count);
                }
            }
            else
            {
                peers[peer] = state;
            }
        }

        /// <summary>
        /// Not that a peer answered VoteOk
        /// </summary>
        /// <param name="peer">The peer to register</param>
        public void Replied(IPAddress peer)
        {
            voteReplyList[peer] = true;
        }

        /// <summary>
        /// Broadcast this vote
        /// </summary>
        public void SendToAll()
        {
            var msg = new Message(MessageType.Vote, IPAddress.Broadcast)
                          {
                              DataAsString = VotingNumber.ToString()
                          };
            Outbox.SendMessage(msg);
        }
        
        /// <summary>
        /// Have all the peers responded to this vote?
        /// </summary>
        /// <returns>True, if all peers have responded to the vote.</returns>
        public bool IsAcknowledged()
        {
            return voteReplyList.Aggregate(true, (current, b) => current && b.Value);
        }
    }
}
