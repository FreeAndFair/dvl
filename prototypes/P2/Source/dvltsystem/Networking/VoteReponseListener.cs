using DVLTerminal.Local;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// A vote response listener waits for VoteOk message from peers.
    /// When a message is received it is noted in the vote.
    /// It should time out as a result of not all peers not answering
    /// </summary>
    class VoteReponseListener : INetworkPacketHandler
    {
        /// <summary>
        /// The NetworkListener neede to listen for messages
        /// </summary>
        private readonly NetworkListener networkListener;

        /// <summary>
        /// The current vote in process
        /// </summary>
        private readonly Vote vote;

        /// <summary>
        /// The timeout in ms
        /// </summary>
        public const int Timeout = 250;

        /// <summary>
        /// Initialises the VoteResponseListener and stores the provided NetworkListener
        /// and Vote to the instance fields.
        /// </summary>
        /// <param name="networkListener">The NetworkListener that VoteResponseListener needs</param>
        /// <param name="vote">The Vote that VoteResponseListener needs</param>
        public VoteReponseListener(NetworkListener networkListener, Vote vote)
        {
            this.networkListener = networkListener;
            this.vote = vote;
        }

        /// <summary>
        /// Handles the Message:
        /// If the MessageType is of any other type than "VoteOK" it ignores the Message.
        /// If it is of the type "VoteOK" the following scenarios may happen:
        /// -   If the sender is not in the VoteReplyList, then that sender is considered dead
        ///     and a reply is send out to the sender to notify it about this. The sender dies.
        /// -   Notifies the vote object that a peer has replied "VoteOK".
        /// </summary>
        /// <param name="message">The Message that is broadcasted.</param>
        /// <returns>
        /// PacketResponse.AcceptAndFinish if all the peers have replied "VoteOK", 
        /// PacketResponse.Accept if not.
        /// </returns>
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.VoteOk)
                return PacketResponse.Decline;

            if (!vote.VoteReplyList.ContainsKey(message.Sender)) //Error in sender - kill'im
            {
                var response = new Message(MessageType.PeerDead, message.Sender) { DataAsString = message.Sender.ToString() };
                Outbox.SendMessage(response);
            }
            else
            {
                vote.Replied(message.Sender);
            }            
            return vote.IsAcknowledged() ? PacketResponse.AcceptAndFinish : PacketResponse.Accept;
        }

        /// <summary>
        /// Creates a new PingRequestResponseListener and sends out a new PingRequest
        /// message to each of the peers that has not yet replied in the VoteReplyList.
        /// </summary>
        public void TimedOut()
        {
            var prrl = new PingRequestResponseListener(networkListener,vote);
            networkListener.RegisterHandler(prrl, PingRequestResponseListener.Timeout);

            foreach (var peer in vote.VoteReplyList)
            {
                if (!peer.Value)
                {
                    var pingRequest = new Message(MessageType.PingRequest) {DataAsString = peer.Key.ToString()};
                    Outbox.SendMessage(pingRequest);
                }
            }
        }
    }
}
