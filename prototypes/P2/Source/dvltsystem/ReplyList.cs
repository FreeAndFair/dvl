namespace dvltsystem
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using System.Net;

    /// <summary>
    /// A map of peers to booleans indicating who replied
    /// </summary>
    public class ReplyList
    {
        private Dictionary<IPAddress, bool> peerMap = new Dictionary<IPAddress, bool>();

        /// <remarks>Author: Emil Blædel Nygaard</remarks>
        /// <summary>
        /// Check if a specific computer has replied
        /// </summary>
        /// <param name="ipadress">The IP Adress of the computer you want to check</param>
        /// <returns>A boolean</returns>
        /// Author: Emil Blædel Nygaard
        public bool GetReply(IPAddress ipadress)
        {
            return this.peerMap[ipadress];
        }

        /// <summary>
        /// Set that a specific computer has replied
        /// </summary>
        /// <param name="ipadress">The IP Adress of the computer you want to set</param>
        /// Author: Emil Blædel Nygaard
        public void SetReply(IPAddress ipadress)
        {
            this.peerMap[ipadress] = true;
        }


        public bool ForAll(Func<bool, bool> f)
        {
            
        }
    }
}