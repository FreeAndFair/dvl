namespace SmallTuba.Network.RequestReply {
	using System;
	using System.Diagnostics.Contracts;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// A network packet containing sender, receiver and request id and a message
	/// </summary>
	[Serializable]
	internal struct Packet {
		/// <summary>
		/// The receiver ID
		/// </summary>
		private readonly string receiverId;

		/// <summary>
		/// The sender ID
		/// </summary>
		private readonly string senderId;

		/// <summary>
		/// The request ID
		/// </summary>
		private readonly string requestId;

		/// <summary>
		/// The content of this packet
		/// </summary>
		private readonly object message;

		/// <summary>
		/// May I have a new packet with this receiver, this sender, this request ID and this message?
		/// </summary>
		/// <param name="receiverId">The receiver ID</param>
		/// <param name="senderId">The sender ID</param>
		/// <param name="requestId">The request ID</param>
		/// <param name="message">The content of this packet</param>
		public Packet(string receiverId, string senderId, string requestId, object message) {
			Contract.Requires(receiverId != null);
			Contract.Requires(senderId != null);
			Contract.Requires(requestId != null);
			Contract.Requires(message != null);
			this.receiverId = receiverId;
			this.senderId = senderId;
			this.requestId = requestId;
			this.message = message;
		}

		/// <summary>
		/// Who is the receiver of this message?
		/// </summary>
		[Pure]
		public string GetReceiverId {
			get {
				return this.receiverId;
			}
		}

		/// <summary>
		/// Who is the sender of this message?
		/// </summary>
		[Pure]
		public string GetSenderId {
			get {
				return this.senderId;
			}
		}

		/// <summary>
		/// What is the request ID of this message?
		/// </summary>
		[Pure]
		public string GetRequestId {
			get {
				return this.requestId;
			}
		}

		/// <summary>
		/// What is the content of this packet?
		/// </summary>
		[Pure]
		public object GetMessage {
			get {
				return this.message;
			}
		}
	}
}