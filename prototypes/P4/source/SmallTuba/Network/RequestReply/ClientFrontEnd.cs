namespace SmallTuba.Network.RequestReply {
    using System;
	using System.Diagnostics.Contracts;
    using SmallTuba.Network.UDP;
    using SmallTuba.Utility;

	/// <author>Christian Olsson (chro@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
	/// This class keeps resending packets for the server if its unresponsive
	/// This class only sends to and receives packages from the server not other clients
	/// </summary>
	public class ClientFrontEnd {
		/// <summary>
		/// The name of the client
		/// </summary>
		private readonly string name;
		
		/// <summary>
		/// The lower level upd communication
		/// </summary>
		private readonly UdpMulticast udpMulticast;

		/// <summary>
		/// A request ID that is increased by one each time a unique packet is send
		/// </summary>
		private long requestId;

		/// <summary>
		/// May I have a new client front end with this name?
		/// </summary>
		/// <param name="name">The name of the client</param>
		public ClientFrontEnd(string name) {
			Contract.Requires(name != null && name != "server");
			this.name = name;

			// Specifies that this is a client
			this.udpMulticast = new UdpMulticast(1);
		}

		/// <summary>
		/// What is the result of the this request?
		/// The method will keep waiting/requesting an answer till the answer is received or the timeout is reached
		/// </summary>
		/// <param name="message">The request</param>
		/// <param name="timeOut">The time to wait in milliseconds, 0 means forever</param>
		/// <returns>The result, null in the case an answer isn't received</returns>
		public object SendRequest(object message, long timeOut) {
			Contract.Requires(message != null);
			Contract.Requires(timeOut >= 0);

			// Send a request
			this.requestId++;
			Packet packet = new Packet("server", this.name, this.requestId.ToString(), message);
			this.udpMulticast.Send(packet);
			
			// Start listening for a reply
			long preTime = DateTime.Now.ToFileTime();

			// Test if the timeout is 0 or the timeout value has run out
			while (timeOut == 0 || DateTime.Now.ToFileTime() < preTime + (timeOut * 10000)) {
				// Wait for reply
				object result = this.udpMulticast.Receive(500);
				if (result == null) {
					// No reply - resend...
					Debug.WriteLine("Client repeats");
					this.udpMulticast.Send(packet);
				}
				else if (result is Packet) {
					// A packet was received
					Packet recPacket = (Packet)result;
					
					// Test if the packet is for us
					if (recPacket.GetReceiverId.Equals(this.name) && recPacket.GetRequestId.Equals(this.requestId.ToString())) {
						return recPacket.GetMessage;
					}
				}
			}

			// The timeout value has expired
			return null;
		}
	}
}
