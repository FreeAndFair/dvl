namespace ServerApplication {
	using System;
	using System.Collections;
	using System.Diagnostics.Contracts;

	using SmallTuba.Database;
	using SmallTuba.Entities;
	using SmallTuba.Network.RPC;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The Server class handles request from the ClientApplication,
	/// by utilizing the VoterNetworkServer and entity classes.
	/// 
	/// The Server will listen forever (or until external process
	/// kill) for requests, and upon request it will utilize a
	/// callback assigned to the underlying VoterNetworkServer
	/// which requests data from the external data source by
	/// utilizing the entity and QueryBuilder classes.
	/// </summary>
	class Server {
		private readonly VoterServer voterServer;
		private readonly ServerData serverData;

		/// <summary>
		/// Construct a new server and instantiate and instance
		/// of the VoterNetworkServer identified as "primary".
		/// </summary>
		public Server() {
			this.voterServer = new VoterServer(System.Net.Dns.GetHostName());
			this.serverData = new ServerData();
		}

		/// <summary>
		/// Calls the Setup() method to give the user possibility
		/// to setup the servers data. If the setup is not canceled,
		/// the proper callback functions will be attached to the 
		/// listeners of the VoterNetworkServer object and the server
		/// will start to listen for requests.
		/// </summary>
		public void Start() {
			Console.WriteLine(@"Digital Voter Registration System");
			Console.WriteLine(@"Server v1.8");
			Console.WriteLine("");
			Console.WriteLine(@"Setup server:");
			Console.WriteLine("");

			if (this.Setup()) {
				Console.WriteLine(@"Server is starting");

				this.voterServer.CprToPersonRequest = CprToPersonRequestHandler;
				this.voterServer.VoterIdToPersonRequest = VoterIdToPersonRequestHandler;

				this.voterServer.RegisterVoteRequest = RegisterVoteRequestHandler;
				this.voterServer.UnregisterVoteRequest = UnregisterVoteRequestHandler;

				this.voterServer.ValidTableRequest = ValidTableRequestHandler;

				Console.WriteLine(@"Server is running");

				this.voterServer.ListenForCalls(0);
			}

			Console.WriteLine(@"Cancelling boot");
		}

		/// <summary>
		/// Handle a request for a PersonState object by CPR.
		/// 
		/// Takes a CPR number as parameter and constructs the
		/// proper Person entity. If the entity does not exists
		/// a PersonState object will still be passed back,
		/// but the Exist property of the object will return false.
		/// </summary>
		/// <param name="cpr">The CPR number to search for a person by.</param>
		/// <param name="clientName">The id of the client.</param>
		/// <returns>A PersonState object filled with information from the Person entity.</returns>
		public Person CprToPersonRequestHandler(string clientName, string cpr) {
			Contract.Requires(cpr != null);

			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "cpr", cpr } });

			return personEntity.ToObject();
		}

		/// <summary>
		/// Handle a request for a PersonState object by barcode.
		/// 
		/// Takes a barcode number as parameter and constructs the
		/// proper Person entity. If the entity does not exists
		/// a PersonState object will still be passed back,
		/// but the Exist property of the object will return false.
		/// </summary>
		/// <param name="voterId">The barcode number to search for a person by.</param>
		/// <param name="clientName">The id of the client.</param>
		/// <returns>A PersonState object filled with information from the Person entity.</returns>
		public Person VoterIdToPersonRequestHandler(string clientName, int voterId) {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "voter_id", voterId } });

			return personEntity.ToObject();
		}

		/// <summary>
		/// Handle a request to register a person.
		/// 
		/// The method takes a PersonState object sent from
		/// a client application, which contains information
		/// about the person who should be registered.
		/// 
		/// If the person does not exists or if the person
		/// already voted, the method will return false.
		/// Otherwise the method will return true meaning
		/// that the registration successed and that a log
		/// entity in the database was created.
		/// </summary>
		/// <param name="person">The person to be registered.</param>
		/// <returns>A boolean value determining if the request failed or successed.</returns>
		/// <param name="clientName">The id of the client.</param>
		public bool RegisterVoteRequestHandler(string clientName, Person person) {
			Contract.Requires(person != null);
			Contract.Ensures(!Contract.OldValue(VoterIdToPersonRequestHandler("", person.VoterId).Exists) || Contract.OldValue(VoterIdToPersonRequestHandler("", person.VoterId).Voted) ? Contract.Result<bool>() == false : Contract.Result<bool>() == true);

			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", person.DbId } });

			if (personEntity.Exists() && !personEntity.Voted) {
				var log = new LogEntity {
					PersonDbId = personEntity.DbId,
					Action = "register",
					Client = clientName,
					PollingTable = clientName,
					Timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
				};

				log.Save();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Handle a request to unregister a person.
		/// 
		/// The method takes a PersonState object sent from
		/// a client application, which contains information
		/// about the person who should be registered.
		/// 
		/// If the person does not exists or if the person
		/// already voted, the method will return false.
		/// Otherwise the method will return true meaning
		/// that the unregistration successed and that a log
		/// entity in the database was created.
		/// </summary>
		/// <param name="person">The person to be unregistrated.</param>
		/// <param name="clientName">The id of the client.</param>
		/// <returns>A boolean value determining if the request failed or successed.</returns>
		public bool UnregisterVoteRequestHandler(string clientName, Person person) {
			Contract.Requires(person != null);
			Contract.Ensures(!Contract.OldValue(VoterIdToPersonRequestHandler("", person.VoterId).Exists) || !Contract.OldValue(VoterIdToPersonRequestHandler("", person.VoterId).Voted) ? Contract.Result<bool>() == false : Contract.Result<bool>() == true);

			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", person.DbId } });

			if (personEntity.Exists() && personEntity.Voted) {
				var log = new LogEntity {
					PersonDbId = personEntity.DbId,
					Action = "unregister",
					Client = clientName,
					PollingTable = clientName,
					Timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
				};

				log.Save();

				return true;
			}

			return false;
		}

		/// <summary>
		/// Returns the available tables at a polling venue
		/// for a client application to choose between.
		/// </summary>
		/// <param name="clientName">The id of the client.</param>
		/// <returns>The available tables for the client to choose as a string array.</returns>
		public string[] ValidTableRequestHandler(string clientName) {
			var queryBuilder = new QueryBuilder();
			queryBuilder.SetType("select");
			queryBuilder.SetTable("Person");
			queryBuilder.SetColumns(new [] { "polling_table" } );
			queryBuilder.SetGroupBy("polling_table");

			var results = queryBuilder.ExecuteQuery();
			var tables = new string [results.Count];

			var i = 0;
			foreach (var result in results) {
				tables[i++] = (string) ((Hashtable) result)["polling_table"];
			}

			return tables;
		}

		/// <summary>
		/// Acts as a commandline interface for setting up the server
		/// given the user the possibility to import data to the
		/// database.
		/// </summary>
		/// <returns>Returns false if the user decides to cancel the setup.</returns>
		private bool Setup() {
			Console.WriteLine(@"Commands: import <<filetoimport.csv>>, clear <<(person|log)>>, start, cancel");

			var input = Console.ReadLine();
			if (input != null) {
				var args = input.Split(' ');
				var command = args.Length > 0 ? args[0] : "";

				switch (command) {
					case "import":
						Console.WriteLine(
							serverData.Import(args)
								? @"importing to database succeeded"
								: @"importing to database failed - command: import <<filetoimport.csv>> - please make sure that the file to import exists and is correctly formatted"
						);

						return this.Setup();
					case "clear":
						Console.WriteLine(
							serverData.Clear(args)
								? @"clearing of database succeeded"
								: @"clearing of database failed - command: clear <<(person|log)>> - please try again"
						);

						return this.Setup();
					case "start":
						break;
					case "cancel":
						return false;
					default:
						return this.Setup();
				}
			} else {
				return this.Setup();
			}

			return true;
		}

		/// <summary>
		/// The main method starting the thread for the
		/// server to run at and instantiating and starting
		/// an instance of the Server class.
		/// </summary>
		static void Main() {
			var server = new Server();

			server.Start();
		}
	}
}
