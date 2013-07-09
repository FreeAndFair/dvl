using System.Net;
using System.Net.Sockets;
using DVLTerminal.Utilities;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Encrypts and broadcasts messages on the network
    /// </summary>
    public static class Outbox
    {
        /// <summary>
        /// Encrypts and broadcasts a message on the network
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void SendMessage(Message message)
        {
            Send(message, true);
        }

        /// <summary>
        /// Broadcasts the message unencrypted to the network - should only be used during startup of the system!
        /// </summary>
        /// <param name="message">The message to send</param>
        public static void SendUnencryptedMessage(Message message)
        {
            Send(message, false);
        }

        /// <summary>
        /// Sends messages and encrypts them if the bool is true
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="encrypt">Should the message be encrypted?</param>
        private static void Send(Message message, bool encrypt)
        {
            var destinationEP = new IPEndPoint(message.Recipient, 5000);
            var bytes = message.ToPacket();
            
            if (encrypt)
            {
                bytes = SecurityManager.GetInstance().Encrypt(bytes);
            }

            var sender = new UdpClient();
            sender.Send(bytes, bytes.Length, destinationEP);
            sender.Close();
        }
    }
}
