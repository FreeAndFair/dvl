using System.Collections.Generic;
using System.Linq;
using System.Net;
using DVLTerminal.Local;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// A PingRequestResponseListener or PRRL is a networklistener that listens for responses to PingRequest
    /// sent out as a result of a missing response from one or more peers.
    /// Can subsequently create other instances of itself if a peer doesn't answer to a PingRequest
    /// </summary>
    class PingRequestResponseListener : INetworkPacketHandler
    {
        private readonly NetworkListener networkListener;
        private readonly Vote vote;
        public const int Timeout = 500; //How many ms should the PRRL wait before trying to ping again
        private const int networkStabilityTreshold = 10; //How deep a level of 'recursion' is allowed for the PRRL.
        private readonly int retries;

        /// <summary>
        /// Dictionary of non responding peers that maps to dictionaries of responding Peers' responses. 
        /// </summary>
        private readonly Dictionary<IPAddress, Dictionary<IPAddress, PingRequestResponse>> nonRespondingPeers = new Dictionary<IPAddress, Dictionary<IPAddress, PingRequestResponse>>();

        /// <summary>
        /// Sets all the fields and gets lists of all alive- and dead peers
        /// Then sets the state of the suspected peers to MaybeDead on the vote
        /// </summary>
        /// <param name="networkListener">The NetworkListener to register new PingRequestResponseListeners in if this should time out</param>
        /// <param name="vote">The current vote that are being processed</param>
        /// <param name="retries">Counter for recursion level as PRRLs can create new PRRLs</param>
        private PingRequestResponseListener(NetworkListener networkListener, Vote vote, int retries)
        {
            this.networkListener = networkListener;
            this.vote = vote;
            this.retries = retries;

            var alivePeers = (from peer in vote.VoteReplyList where peer.Value select peer.Key).ToList();
            var deadPeers = (from peer in vote.VoteReplyList where !peer.Value select peer.Key).ToList();
            
            foreach (var deadPeer in deadPeers)
            {
                vote.SetPeerState(deadPeer,PeerState.MaybeDead);
                var dict = alivePeers.ToDictionary(peer => peer, peer => PingRequestResponse.NotYet);
                nonRespondingPeers.Add(deadPeer,dict);
            }           
        }

        /// <summary>
        /// Create a new PingRequestResponseListener that will listen for responses to Ping requests
        /// </summary>
        /// <param name="networkListener">The current networklistener</param>
        /// <param name="vote">The vote being worked on</param>
        public PingRequestResponseListener(NetworkListener networkListener, Vote vote) : this(networkListener,vote, 0)
        {
            //This constructor gives public access to this class without exposing the recursion counter
        }

        /// <summary>
        /// Handle incoming ping request responses and saves them in the current working data dictionary
        /// If a PingOK packet is received, the suspected peer is considered alive.
        /// If a PingFailed packet is received and that's the only kind of messages that
        /// have been received concerning this peer, the peer is concidered dead.
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>AcceptAndFinish if all suspected peers is concluded dead or alive. 
        /// Accept if the packet is of type PingOK or PingFailed. Decline otherwise</returns>
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.PingOk && message.MessageType != MessageType.PingFailed)
                return PacketResponse.Decline;

            IPAddress pingedIp;
            var validIp = IPAddress.TryParse(message.DataAsString, out pingedIp);
            if (!validIp)
            {
                message.DeemInvalid();
                return PacketResponse.Decline;
            }

            if (nonRespondingPeers.ContainsKey(pingedIp))
            {
                UpdateResponses(pingedIp, message.Sender, message.MessageType);
            }
            
            if (nonRespondingPeers.Count == 0)
            {
                var vrl = new VoteReponseListener(networkListener, vote);
                networkListener.RegisterHandler(vrl, VoteReponseListener.Timeout);
                vote.SendToAll();
                return PacketResponse.AcceptAndFinish;
            }

            return PacketResponse.Accept;
        }

        /// <summary>
        /// Update the list of responses from an incoming packet
        /// </summary>
        /// <param name="nonRespondingPeer">The unresponsive peer that has new data</param>
        /// <param name="sender">The sender of the new data</param>
        /// <param name="type">The message type that was received</param>
        private void UpdateResponses(IPAddress nonRespondingPeer, IPAddress sender, MessageType type)
        {
            if (type == MessageType.PingOk)
            {
                nonRespondingPeers.Remove(nonRespondingPeer);
                vote.SetPeerState(nonRespondingPeer, PeerState.Alive);
            }
            else
            {
                nonRespondingPeers[nonRespondingPeer][sender] = PingRequestResponse.No;

                //If all living peers answered No, broadcast that the peer must be dead.
                if (nonRespondingPeers[nonRespondingPeer].All(responder => responder.Value == PingRequestResponse.No))
                {
                    nonRespondingPeers.Remove(nonRespondingPeer);
                    vote.SetPeerState(nonRespondingPeer, PeerState.Dead);
                    var msg = new Message(MessageType.PeerDead) {DataAsString = nonRespondingPeer.ToString()};
                    Outbox.SendMessage(msg);
                }
            }
        }

        /// <summary>
        /// The PRRL timed out.
        /// All peers that did not respond are registered as non-responding, and a new PRRL is created from the peer list
        /// </summary>
        public void TimedOut()
        {
            foreach (var nonRespondingPeer in nonRespondingPeers)
            {
                vote.SetPeerState(nonRespondingPeer.Key,PeerState.MaybeDead);
            }
            if (retries+1 >= networkStabilityTreshold)
                throw new NotEnoughPeersException(0);

            var prrl = new PingRequestResponseListener(networkListener, vote, retries + 1);
            networkListener.RegisterHandler(prrl, Timeout);
            
            foreach (var peer in nonRespondingPeers)
            {
                var pingRequest = new Message(MessageType.PingRequest) { DataAsString = peer.Key.ToString() };
                Outbox.SendMessage(pingRequest);
            }
        }
    }
}
