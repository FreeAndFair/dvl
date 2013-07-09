namespace SmallTuba.Network.RPC {
    using System;
	using System.Diagnostics.Contracts;

	/// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// A message describing a keyword and a value
    /// </summary>
    [Serializable]
    internal struct Message {
        /// <summary>
        /// The keyword
        /// </summary>
        private readonly Keyword keyword;

        /// <summary>
        /// The sender of the message
        /// </summary>
        private readonly string sender;

        /// <summary>
        /// The value
        /// </summary>
        private readonly object value;

        /// <summary>
        /// May I have a new message with this keyword and this value?
        /// </summary>
        /// <param name="keyword">The keyword</param>
        /// <param name="sender">The sender</param>
        /// <param name="value">The value</param>
        public Message(Keyword keyword, string sender, object value) {
            this.keyword = keyword;
            this.sender = sender;
            this.value = value;
        }

        /// <summary>
        /// What is the keyword of this message?
        /// </summary>
        [Pure]
        public Keyword GetKeyword {
            get {
                return this.keyword;
            }
        }

        /// <summary>
        /// Who is the sender of this message
        /// </summary>
        public string GetSender {
            get {
                return this.sender;
            }
        }
        
        /// <summary>
        /// What is the value of this message?
        /// </summary>
        [Pure]
        public object GetValue {
            get {
                return this.value;
            }
        }
    }
}
