namespace SmallTuba.Network.RPC {
    using System;
    using System.Diagnostics.Contracts;
	using SmallTuba.Entities;
	using SmallTuba.Network.RequestReply;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The server side of the network communication in our voting system.
	///	This is the top most level and communication is based on procedure calls.
	/// </summary>
	public class VoterServer {
		/// <summary>
		/// The name of the server
		/// </summary>
		private readonly string name;

		/// <summary>
		/// The request reply network class
		/// </summary>
		private readonly ServerFrontEnd serverFrontEnd;

		/// <summary>
		/// May I have a new server for the voter network with this name?
		/// </summary>
		/// <param name="name">The name of the server</param>
		public VoterServer(string name) {
			Contract.Requires(name != null);
			this.name = "Server: " + name;
			this.serverFrontEnd = new ServerFrontEnd();
			this.serverFrontEnd.RequestHandler = this.RequestHandler;
		}

		/// <summary>
		/// A type of a function to invoke when a request for a person is made
		/// </summary>
		/// <param name="clientName">The name of the client making the call</param>
		/// <param name="cpr">The cpr. nr.</param>
		/// <returns>The person</returns>
		public delegate Person CprToPersonRequestDelegate(string clientName, string cpr);

		/// <summary>
		/// A type of a function to invoke when a request for a person is made
		/// </summary>
		/// <param name="clientName">The name of the client making the call</param>
		/// <param name="id">The id</param>
		/// <returns>The person</returns>
		public delegate Person VoterIdToPersonRequestDelegate(string clientName, int id);

		/// <summary>
		/// A type of a function to invoke when a request for registering a voter is made
		/// </summary>
		/// <param name="clientName">The name of the client making the call</param>
		/// <param name="person">The person to register</param>
		/// <returns>If the voter was registered</returns>
		public delegate bool RegisterVoteRequestDelegate(string clientName, Person person);

		/// <summary>
		/// A type of a function to invoke when a request for unregistering a voter is made
		/// </summary>
		/// <param name="clientName">The name of the client making the call</param>
		/// <param name="person">The person to unregister</param>
		/// <returns>If the voter was unregistered</returns>
		public delegate bool UnregisterVoteRequestDelegate(string clientName, Person person);

		/// <summary>
		/// A type of a function to invoke when a request for valid tables is made
		/// </summary>
		/// <param name="clientName">The name of the client making the call</param>
		/// <returns>The valid tables</returns>
		public delegate string[] ValidTableRequestDelegate(string clientName);

		/// <summary>
		/// Invoke this function that returns a person when asked about a person from a cpr.nr.
		/// </summary>
		public CprToPersonRequestDelegate CprToPersonRequest { get; set; }

		/// <summary>
		/// Invoke this function that returns a person when asked about a person from a barcode ID
		/// </summary>
		public VoterIdToPersonRequestDelegate VoterIdToPersonRequest { get; set; }

		/// <summary>
		/// Invoke this function when asked about registering a user
		/// </summary>
		public RegisterVoteRequestDelegate RegisterVoteRequest { get; set; }

		/// <summary>
		/// Invoke this function when asked about unregistering a user
		/// </summary>
		public UnregisterVoteRequestDelegate UnregisterVoteRequest { get; set; }

		/// <summary>
		/// Invoke this function when asked about valid tables
		/// </summary>
		public ValidTableRequestDelegate ValidTableRequest { get; set; }

		/// <summary>
		/// Listen for calls for this amount of time
		/// </summary>
		/// <param name="timeOut">The amount of time in milliseconds, if zero it waits forever</param>
		public void ListenForCalls(int timeOut) {
			Contract.Requires(timeOut >= 0);
			Contract.Requires(this.CprToPersonRequest != null);
			Contract.Requires(this.VoterIdToPersonRequest != null);
			Contract.Requires(this.RegisterVoteRequest != null);
			Contract.Requires(this.UnregisterVoteRequest != null);
			Contract.Requires(this.ValidTableRequest != null);

			this.serverFrontEnd.ListenForCalls(timeOut);
		}

		/// <summary>
		/// This function is called when a request from a client is received
		/// </summary>
		/// <param name="o">The request from the client</param>
		/// <returns>The reply for the client</returns>
		private object RequestHandler(object o) {
            // This should not be possible
            // Only packets from a SmallTuba.Network.RequestReply.ClientFrontEnd will be received
            if (!(o is Message)) {
                Contract.Assert(false);
                return null;
            }
            
            Message request = (Message) o;
			Keyword keyword = request.GetKeyword;

			// Test which procedure is to be invoked
			switch (keyword) {
				case Keyword.GetPersonFromCpr:
					Person personFromCpr = this.CprToPersonRequest.Invoke(request.GetSender, request.GetValue.ToString());
					return new Message(keyword, this.name, personFromCpr);

				case Keyword.GetPersonFromId:
					Person personFromId = this.VoterIdToPersonRequest.Invoke(request.GetSender, (int)request.GetValue);
					return new Message(keyword, this.name, personFromId);

				case Keyword.RegisterVoter:
					bool registered = this.RegisterVoteRequest.Invoke(request.GetSender, (Person)request.GetValue);
					return new Message(keyword, this.name, registered);
				
				case Keyword.UnregisterVoter:
					bool unregistered = this.UnregisterVoteRequest.Invoke(request.GetSender, (Person)request.GetValue);
					return new Message(keyword, this.name, unregistered);
				
				case Keyword.ValidTables:
					string[] arr = this.ValidTableRequest.Invoke(request.GetSender);
					return new Message(keyword, this.name, arr);

				case Keyword.Ping:
					// An empty message indicating that the server is listening
					return new Message(keyword, this.name, null);
				default:
					throw new NotImplementedException();
			}
		}
	}
}
