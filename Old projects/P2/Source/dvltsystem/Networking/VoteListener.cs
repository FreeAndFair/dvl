using System;
using DVLTerminal.Local;
using DVLTerminal.Utilities;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Listens for incoming votes on the network registering them in the database and answering VoteOk back to sender.
    /// If a vote is received from an unknown peer, the response will be a PeerDead packet with the recipient as data,
    /// to inform the computer that it shouldn't continue working.
    /// </summary>
    class VoteListener : INetworkPacketHandler
    {
        /// <summary>
        /// The LocalVoteListener containing a list of all connected computers
        /// </summary>
        private readonly LocalVoteListener localVoteListener;

        /// <summary>
        /// Create a new VoteListener using this LocalVoteListener as a source of connected computers
        /// </summary>
        /// <param name="lvl">The LocalVoteListener to use as source of connected computers</param>
        public VoteListener(LocalVoteListener lvl)
        {
            localVoteListener = lvl;
        }

        /// <summary>
        /// Recieves a voting number and reports to it's own database
        /// that the person with that specific voting number has voted.
        /// </summary>
        /// <param name="message">The Message-object containing the voting number
        /// of the person who has been given a voting card</param>
        /// <returns></returns>
        /// Author: Michael Oliver Urhøj Mortensen
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.Vote)
                return PacketResponse.Decline;
            
            try
            {
                var voterNumber = Convert.ToUInt64(message.DataAsString);
                if (!localVoteListener.Peers.ContainsKey(message.Sender)) //Error in sender - kill'im
                {
                    var response = new Message(MessageType.PeerDead, message.Sender) { DataAsString = message.Sender.ToString() };
                    Outbox.SendMessage(response);
                }
                else
                {
                    var response = new Message(MessageType.VoteOk, message.Sender) { Data = message.Data };
                    Outbox.SendMessage(response);
                    Database.GetInstance.SetVoted(voterNumber, true);
                }
            }
            catch(Exception)
            {
                message.DeemInvalid();
                return PacketResponse.Decline;
            }
            return PacketResponse.Accept;
        }

        /// <summary>
        /// Shouldn't time out
        /// </summary>
        public void TimedOut()
        {
            System.Diagnostics.Debug.Assert(false);
        }
    }
}