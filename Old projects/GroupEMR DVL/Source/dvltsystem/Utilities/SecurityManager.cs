using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System;

namespace DVLTerminal.Utilities
{
    /// <summary>
    /// The securitymanager gives easy access to encryption of messages
    /// </summary>
    /// Author: Rasmus Greve & Emil Blædel Nygaard
    public class SecurityManager
    {
        private static SecurityManager singleton;

        private SecurityManager()
        {
            GenerateKeys();
        }

        /// <summary>
        /// Generate and store new values for Key and IV
        /// </summary>
        public void GenerateKeys()
        {
            using (var aesManaged = new AesManaged())
            {
                Key = aesManaged.Key;
                IV = aesManaged.IV;
            }
        }

        /// <summary>
        /// The key to use in AES transformation
        /// </summary>
        private byte[] Key;

        /// <summary>
        /// The initialization vector to use in AES transformation
        /// </summary>
        private byte[] IV;

        /// <summary>
        /// Get the instance of the SecurityManager
        /// </summary>
        /// <returns>The only instance of SecurityManager</returns>
        public static SecurityManager GetInstance()
        {
            return singleton ?? (singleton = new SecurityManager());
        }

        /// <summary>
        /// Get the Key and IV as a byte array that can be sent over the network
        /// </summary>
        /// <returns>A byte array containing key and IV</returns>
        public byte[] GetKeysAsPacket()
        {
            var data = new byte[Key.Length + IV.Length + 1];
            data[0] = (byte)Key.Length; //Will never have a length over 255! //TODO: Is this right?
            Array.Copy(Key, 0, data, 1, Key.Length);
            Array.Copy(IV, 0, data, Key.Length + 1, IV.Length);
            return data;
        }
        
        /// <summary>
        /// Set Key and IV from a packet byte array received from the network
        /// </summary>
        /// <param name="packet">The received packetdata containing key and iv</param>
        public void SetKeysFromPacket(byte[] packet)
        {
            byte keyLength = packet[0];
            byte[] key = new byte[keyLength];
            byte[] iv = new byte[(packet.Length - keyLength) - 1];
            Array.Copy(packet, 1, key, 0, key.Length);
            Array.Copy(packet, keyLength + 1, iv, 0, iv.Length);
            Key = key;
            IV = iv;
        }

        /// <summary>
        /// Encrypt a given string using AES
        /// </summary>
        /// <param name="input">The string to encrypt</param>
        /// <returns>The encrypted data as a byte array</returns>
        public byte[] Encrypt(byte[] input)
        {
            var aesManaged = new AesManaged { Key = Key, IV = IV };
            var encryptor = aesManaged.CreateEncryptor(Key, IV);

            byte[] output;
            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(input,0,input.Length);
                }
                output = memoryStream.ToArray();
            }

            aesManaged.Dispose();
            return output;
        }

        /// <summary>
        /// Decrypts a given data byte array using AES
        /// </summary>
        /// <param name="input">The byte array data to decrypt</param>
        /// <returns>The decrypted array of data</returns>
        public byte[] Decrypt(byte[] input)
        {
            var aesManaged = new AesManaged { Key = Key, IV = IV };
            var decryptor = aesManaged.CreateDecryptor(Key, IV);

            byte[] output;
            using (var memoryStream = new MemoryStream(input))
            {
                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    var bytes = new List<byte>();
                    var read = cryptoStream.ReadByte();
                    while (read != -1)
                    {
                        bytes.Add((byte)read); //Safe cast as read will be a byte or -1, and -1 is handled
                        read = cryptoStream.ReadByte();
                    }
                    output = bytes.ToArray();
                }
            }
            aesManaged.Dispose();
            return output;
        }
    }
}
