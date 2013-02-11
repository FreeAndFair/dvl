using System;
using System.Net;
using System.Linq;

namespace DVLTerminal.Networking
{
    /// <summary>
    /// Defines a message that was received or can be sent over the network.
    /// Contains data which is stored as a byte array, but can be accesed as a string.
    /// </summary>
    public class Message
    {
        /// <summary>
        /// The sender of the message. This is unused on outgoing messages
        /// </summary>
        public IPAddress Sender { get; set; }
        
        /// <summary>
        /// The recipient of the message. Would be local ip on incoming messages
        /// </summary>
        public IPAddress Recipient { get; set; }

        /// <summary>
        /// The type of message
        /// </summary>
        public MessageType MessageType {get; private set; }

        /// <summary>
        /// The data array that the message contains
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Get or set the data as a string. 
        /// Note that some characters can't be translated from byte to string, which could result in a bad packet.
        /// </summary>
        public string DataAsString
        {
            get
            {
                return Data == null ? "" : new String((from b in Data select (char)b).ToArray());
            }
            set
            {
                Data = (from c in value select (byte)c).ToArray();
            }
        }

        /// <summary>
        /// Deem the message invalid. Could be done as a result of irrelevant packet, wrong decryption key or ill formed from sender
        /// </summary>
        public void DeemInvalid()
        {
            MessageType = MessageType.Unknown;
        }

        /// <summary>
        /// Create a new message to be broadcasted
        /// </summary>
        /// <param name="type">The type of message to create</param>
        public Message(MessageType type) : this(type, IPAddress.Broadcast) {}

        /// <summary>
        /// Create a new message.
        /// </summary>
        /// <param name="type">The type of message</param>
        /// <param name="recipient">The recipient of the message</param>
        public Message(MessageType type, IPAddress recipient)
        {
            MessageType = type;
            Recipient = recipient;
        }

        /// <summary>
        /// Create a new message from a string representation
        /// </summary>
        /// <param name="packetData">The datagram representation. Could be created by a call to ToPacket() on a message</param>
        /// <param name="sender">The (intended) sender of the message</param>
        /// <param name="recipient">The (intended) recipient of the message</param>
        /// <returns>The created message</returns>
        public static Message FromPacket(byte[] packetData, IPAddress sender, IPAddress recipient)
        {
            try
            {
                var dataPart = new byte[packetData.Length-1];
                Array.Copy(packetData, 1, dataPart, 0, dataPart.Length);

                return new Message((MessageType)packetData[0], recipient) { Data = dataPart, Sender = sender};
            }
            catch (Exception)
            {
                return new Message(MessageType.Unknown, recipient) {Sender = sender};
            }
        }

        /// <summary>
        /// Convert the message to a bytearray packet that can be send over the network
        /// </summary>
        /// <returns>A bytearray containing the message</returns> 
        public byte[] ToPacket()
        {
            if (Data == null || Data.Length == 0) return new byte[]{(byte)MessageType};
            var output = new byte[Data.Length+1];
            output[0] = (byte) MessageType;
            Array.Copy(Data,0,output,1,Data.Length);
            return output;
        }
    }
}
