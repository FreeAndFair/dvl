using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using DVLTerminal.Utilities;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Receives and decrypts messages from the network, and saves them as Message in a LinkedList
    /// </summary>
    public class Inbox
    {
        /// <summary>
        /// The only instance of inbox
        /// </summary>
        private static readonly Inbox singleton = new Inbox();

        /// <summary>
        /// The udp client listening for incoming packets
        /// </summary>
        private readonly UdpClient client = new UdpClient(5000, AddressFamily.InterNetwork);

        /// <summary>
        /// A list of all the current received messages - stored as Message
        /// </summary>
        private readonly LinkedList<Message> receivedList = new LinkedList<Message>();

        /// <summary>
        /// An inidicator of whether the inbox should try to decrypt incoming messages
        /// </summary>
        public bool UseEncryption { get; set; }
        
        private Inbox()
        {
        }

        /// <summary>
        /// The IP address of this computer
        /// </summary>
        public IPAddress MyIP { get; private set; }

        /// <summary>
        /// Starts listening for UDP messages on the network, and sets the local IP adress in the variable 'myip'
        /// </summary>
        public void StartListening()
        {
            client.BeginReceive(ReceiveCallback, client);

            foreach (var possibleMatch in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (possibleMatch.AddressFamily == AddressFamily.InterNetwork)
                {
                    MyIP = possibleMatch;
                }
            }
        }

        /// <summary>
        /// Decrypts and stores a message in the 'receivedList'
        /// </summary>
        private void ReceiveCallback(IAsyncResult ar)
        {
            var senderip = new IPEndPoint(IPAddress.Broadcast, 0);
            var receiveBytes = client.EndReceive(ar, ref senderip);
            client.BeginReceive(ReceiveCallback, client);
            if (senderip.Address.Equals(MyIP)) return; //Don't receive packets from self
            if (UseEncryption)
            {
                try
                {
                    receiveBytes = SecurityManager.GetInstance().Decrypt(receiveBytes);
                }
                catch (CryptographicException)
                {
                    receiveBytes[0] = (byte) MessageType.Unknown;
                }
                
            }
            var msg = Message.FromPacket(receiveBytes, senderip.Address, MyIP);
            receivedList.AddFirst(msg);
        }

        /// <summary>
        /// Receive an instance of the Inbox class
        /// </summary>
        /// <returns>Instance of Inbox</returns>
        public static Inbox GetInstance()
        {
            return singleton;
        }

        /// <summary>
        /// Check to see if there are any messages in the inbox
        /// </summary>
        /// <returns>True if there is at lesat one new message</returns>
        public bool HasMessages()
        {
            return receivedList.Count > 0;
        }

        /// <summary>
        /// Get and remove the next message in the inbox
        /// </summary>
        /// <returns>The first received message</returns>
        public Message GetNextMessage()
        {
            var msg = receivedList.Last.Value;
            receivedList.RemoveLast();
            return msg;
        }
    }
}
