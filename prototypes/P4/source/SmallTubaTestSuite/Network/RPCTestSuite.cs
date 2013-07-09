namespace SmallTubaTestSuite.Network {
	using System;
	using System.Threading;
	using NUnit.Framework;
	using SmallTuba.Entities;
	using SmallTuba.Network.RPC;
	using SmallTuba.Utility;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	[TestFixture]
	class RPCTestSuite {
		private Person person1;
		private Person person2;
		private Person emptyPerson;
		private int unixTime;
		private string[] tables;

		private void Init() {
			unixTime = (int)TimeConverter.ConvertToUnixTimestamp(DateTime.Now.ToUniversalTime());
			person1 = new Person() {
				Cpr = "1",
				FirstName = "TestCprFirst",
				DbId = 11,
				LastName = "TestCprLast",
				VotedPollingTable = "Table 1",
				VotedTime = unixTime,
				Voted = false,
				Exists = true
			};
			person2 = new Person() {
				Cpr = "2",
				FirstName = "TestIdFirst",
				DbId = 22,
				LastName = "TestIdFirst",
				VotedPollingTable = "Table 2",
				VotedTime = unixTime,
				Voted = true,
				Exists = true
			};
			emptyPerson = new Person();
			tables = new string[] { "Table 1", "Table 2", "Table 3" };
		}

		[Test]
		public void TestRpc() {
			// Setup test objects
			Init();

			// Start a server that will be active for 10 seconds
			Thread serverThread = new Thread(new ThreadStart(SetupServer));
			serverThread.Start();

			// Wait to make sure its active
			Thread.Sleep(5000);

			// Start testing
			VoterClient voterClient = new VoterClient("Client1");

			// Test the name
			Assert.That(voterClient.Name.Equals("Client1"));
			voterClient.Name = "Client1.1";
			Assert.That(voterClient.Name.Equals("Client1.1"));

			// Connected
			Assert.True(voterClient.Connected());

			//Testing person from cpr
			Person person = voterClient.GetPersonFromCpr(0);
			Assert.That(person.Equals(emptyPerson));
			person = voterClient.GetPersonFromCpr(1);
			Assert.That(person.Equals(person1));

			//Testing person from id
			person = voterClient.GetPersonFromId(0);
			Assert.That(person.Equals(emptyPerson));
			person = voterClient.GetPersonFromId(2);
			Assert.That(person.Equals(person2));

			//Register voter
			bool b = voterClient.RegisterVoter(person1);
			Assert.True(b);

			//unregister voter
			b = voterClient.UnregisterVoter(person2);
			Assert.False(b);

			//Valid tables
			string[] arr = voterClient.ValidTables();
			Assert.That(arr.Length == 3);
			Assert.That(arr[0] == tables[0]);
			Assert.That(arr[1] == tables[1]);
			Assert.That(arr[2] == tables[2]);


			// Await that the server runs out of time
			Thread.Sleep(10000);

			// Abort the server thread
			serverThread.Abort();

			// Make sure that the thraed is not allive
			Thread.Sleep(5000);

			// Test that it perfomrs well when not connected to a server
			//Connected
			Assert.False(voterClient.Connected());

			//Testing person from cpr
			person = voterClient.GetPersonFromCpr(1);
			Assert.That(person == null);

			//Testing person from id
			person = voterClient.GetPersonFromId(2);
			Assert.That(person == null);

			//Register voter
			b = voterClient.RegisterVoter(person1);
			Assert.False(b);

			//unregister voter
			b = voterClient.UnregisterVoter(person2);
			Assert.False(b);

			//Valid tables
			arr = voterClient.ValidTables();
			Assert.That(arr == null);
		}

		/// <summary>
		/// A server that will listen for 10 seconds
		/// </summary>
		private void SetupServer() {
			VoterServer voterServer = new VoterServer(System.Net.Dns.GetHostName());
			voterServer.CprToPersonRequest = ((name, cpr) => cpr == "1" ? person1 : emptyPerson);
			voterServer.VoterIdToPersonRequest = ((name, id) => id == 2 ? person2 : emptyPerson);
			voterServer.RegisterVoteRequest = ((name, person) => !person.Voted);
			voterServer.UnregisterVoteRequest = ((name, person) => !person.Voted);
			voterServer.ValidTableRequest = ((name) => tables);
			voterServer.ListenForCalls(10000);
		}
	}
}