using System.Collections.Generic;
using System.Net;
using DVLTerminal.Utilities;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// The connector class establishes connections to other peers also using the connector.
    /// Also allows for keyexchange
    /// </summary>
    public class Connector : INetworkPacketHandler
    {
        /// <summary>
        /// A Dictionary of connected peers and their computer names.
        /// </summary>
        public Dictionary<IPAddress, string> ConnectedPeers { get; private set; }
        
        /// <summary>
        /// A value indicating if the connection setup process is done
        /// </summary>
        public bool ConnectionDone { get; private set; }

        /// <summary>
        /// Create a new connector and put all found peers in the given set
        /// </summary>
        /// <param name="peers">The set to store peers in</param>
        public Connector(Dictionary<IPAddress, string> peers)
        {
            ConnectedPeers = peers;
            Inbox.GetInstance().UseEncryption = false;
            ConnectionDone = false;
        }

        /// <summary>
        /// Broadcasts a heartbeat to the entire network
        /// </summary>
        public void BroadcastHeartbeatRequest()
        {
            var msg = new Message(MessageType.HeartbeatRequest) {DataAsString = Dns.GetHostName()};
            Outbox.SendUnencryptedMessage(msg);
        }
        
        /// <summary>
        /// Handle incoming packets related to connection setup
        /// </summary>
        /// <param name="message">The message to handle</param>
        /// <returns>A PacketResponse indicating if the message was consumed</returns>
        public PacketResponse HandlePacket(Message message)
        {
            switch (message.MessageType)
            {
                case MessageType.HeartbeatRequest:
                    var msg = new Message(MessageType.Heartbeat, message.Sender) {DataAsString = Dns.GetHostName()};
                    Outbox.SendUnencryptedMessage(msg);
                    if (!ConnectedPeers.ContainsKey(message.Sender))
                        ConnectedPeers.Add(message.Sender, message.DataAsString);
                    return PacketResponse.Accept;

                case MessageType.Heartbeat:
                    if (!ConnectedPeers.ContainsKey(message.Sender))
                        ConnectedPeers.Add(message.Sender, message.DataAsString);
                    return PacketResponse.Accept; 

                case MessageType.EncryptionPacket:
                    SecurityManager.GetInstance().SetKeysFromPacket(message.Data);
                    Inbox.GetInstance().UseEncryption = true;
                    ConnectionDone = true;
                    
                    return PacketResponse.AcceptAndFinish;

                default:
                    return PacketResponse.Decline;
            }
        }

        /// <summary>
        /// Send the generated key and IV directly to the discovered peers
        /// </summary>
        public void SendKey()
        {
            if (ConnectedPeers.Count < Program.MinimumPeerCount)
            {
                throw new NotEnoughPeersException(ConnectedPeers.Count);
            }

            foreach (var ipaddress in ConnectedPeers)
            {
                var keymsg = new Message(MessageType.EncryptionPacket, ipaddress.Key);
                keymsg.Data = SecurityManager.GetInstance().GetKeysAsPacket();
                Outbox.SendUnencryptedMessage(keymsg);
            }
            Inbox.GetInstance().UseEncryption = true;
            ConnectionDone = true;
        }

        /// <summary>
        /// Shouldn't time out
        /// </summary>
        public void TimedOut()
        {
            System.Diagnostics.Debug.Assert(false, "Connector timed out");
        }
    }
}

