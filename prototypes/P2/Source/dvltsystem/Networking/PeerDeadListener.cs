using System.Net;
using DVLTerminal.Local;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Handles incoming PeerDead messages
    /// </summary>
    class PeerDeadListener : INetworkPacketHandler
    {
        /// <summary>
        /// The current local vote listener that holds the list of connected peers
        /// </summary>
        private readonly LocalVoteListener localVoteListener;

        /// <summary>
        /// Create a new PeerDeadListener with a given LocalVoteListener
        /// </summary>
        /// <param name="lvl">The local vote listener that holds the list of connected peers</param>
        public PeerDeadListener(LocalVoteListener lvl)
        {
            localVoteListener = lvl;
        }

        /// <summary>
        /// Check if the message is of the type PeerDead and return a PacketResponse
        /// </summary>
        /// <param name="message">The Message that should be checkeds</param>
        /// <returns>Was the type PeerDead? If Accept - it has been handled</returns>
        public PacketResponse HandlePacket(Message message)
        {
            if (message.MessageType != MessageType.PeerDead)
                return PacketResponse.Decline;

            IPAddress deadPeer;
            bool valid = IPAddress.TryParse(message.DataAsString, out deadPeer);
            if (!valid)
            {
                message.DeemInvalid();
                return PacketResponse.Decline;
            }

            if (deadPeer.Equals(message.Recipient))
            {
                throw new NotEnoughPeersException(0);
            }

            localVoteListener.PeerIsDead(deadPeer);
            return PacketResponse.Accept;
        }

        /// <summary>
        /// This is not used!
        /// </summary>
        public void TimedOut()
        {
            System.Diagnostics.Debug.Assert(false);
        }
    }
}
