namespace ServerApplication {
	using System.Collections;
	using System.Collections.Generic;

	using NUnit.Framework;

	using SmallTuba.Entities;
	using SmallTuba.Utility;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// Testing the interface of the server. Making sure
	/// that the reponses to the request is the correct
	/// data to be send over the network etc.
	/// </summary>
	[TestFixture]
	public class ServerTestSuite {
		private Server server;
		
		/// <summary>
		/// Initialize an instance of the Server class
		/// to test against. Set external data source
		/// debug mode to true.
		/// </summary>
		[TestFixtureSetUp]
		public void TestSetUp() {
			this.server = new Server();

			Debug.ExternalDataSources = true;
		}

		/// <summary>
		/// Set external data source debug mode to false.
		/// </summary>
		[TestFixtureTearDown]
		public void TestTearDown() {
			Debug.ExternalDataSources = false;
		} 

		/// <summary>
		/// Testing the CprToPersonRequestHandler method
		/// with a cpr from a valid, existing person.
		/// </summary>
		[Test]
		public void TestCprToPersonRequestHandlerWithExistingPerson() {
			var person = this.server.CprToPersonRequestHandler("test client", "0123456789");
			
			Assert.That(person.Exists);
			Assert.That(person.DbId == 1);
			Assert.That(person.FirstName == "Henrik");
			Assert.That(person.LastName == "Haugbølle");
			Assert.That(person.Cpr == "0123456789");
			Assert.That(person.VoterId == 3306);
			Assert.That(person.PollingVenue == "Venue of Awesome");
			Assert.That(person.PollingTable == "Table of Win");
		}
		
		/// <summary>
		/// Testing the CprToPersonRequestHandler method
		/// with a cpr from a non-existing/invalid person.
		/// </summary>
		[Test]
		public void TestCprToPersonRequestHandlerWithUnexistingPerson() {
			var person = this.server.CprToPersonRequestHandler("test client", "0711891952");
			
			Assert.That(person.Exists == false);
			Assert.That(person.DbId == 0);
			Assert.That(person.FirstName == "");
			Assert.That(person.LastName == "");
			Assert.That(person.Cpr == "");
			Assert.That(person.VoterId == 0);
			Assert.That(person.PollingVenue == "");
			Assert.That(person.PollingTable == "");
		}

		/// <summary>
		/// Testing the IdToPersonRequestHandler method
		/// with a id from a valid, existing person.
		/// </summary>
		[Test]
		public void TestVoterIdToPersonRequestHandlerWithExistingPerson() {
			var person = this.server.VoterIdToPersonRequestHandler("test client", 3306);
			
			Assert.That(person.Exists);
			Assert.That(person.DbId == 1);
			Assert.That(person.FirstName == "Henrik");
			Assert.That(person.LastName == "Haugbølle");
			Assert.That(person.Cpr == "0123456789");
			Assert.That(person.VoterId == 3306);
			Assert.That(person.PollingVenue == "Venue of Awesome");
			Assert.That(person.PollingTable == "Table of Win");
		}

		/// <summary>
		/// Testing the IdToPersonRequestHandler method
		/// with a id from a valid, existing person.
		/// </summary>
		[Test]
		public void TestVoterIdToPersonRequestHandlerWithUnexistingPerson() {
			var person = this.server.VoterIdToPersonRequestHandler("test client", 669);

			Assert.That(person.Exists == false);
			Assert.That(person.DbId == 0);
			Assert.That(person.FirstName == "");
			Assert.That(person.LastName == "");
			Assert.That(person.Cpr == "");
			Assert.That(person.VoterId == 0);
			Assert.That(person.PollingVenue == "");
			Assert.That(person.PollingTable == "");
		}

		/// <summary>
		/// Test registration of a voter/person which has
		/// not voted before and is a valid voter.
		/// </summary>
		[Test]
		public void TestRegisterVoteRequestHandlerWithExistingNonVotePerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 1 } });
			
			var person = personEntity.ToObject();

			Assert.That(this.server.RegisterVoteRequestHandler("test client", person));

			var logEntity = personEntity.GetMostRecentLog();
			logEntity.Delete();
		}

		/// <summary>
		/// Test registration of a voter/person which has
		/// voted before but is a valid voter.
		/// </summary>
		[Test]
		public void TestRegisterVoteRequestHandlerWithExistingVotePerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 2 } });
			
			var person = personEntity.ToObject();

			Assert.That(!this.server.RegisterVoteRequestHandler("test client", person));
		}

		/// <summary>
		/// Test registration with a non-existing person.
		/// </summary>
		[Test]
		public void TestRegisterVoteRequestHandlerWithUnexistingPerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 669 } });
			
			var person = personEntity.ToObject();

			Assert.That(!this.server.RegisterVoteRequestHandler("test client", person));
		}

		/// <summary>
		/// Test unregistration of a person who have not
		/// voted before but is a valid voter.
		/// </summary>
		[Test]
		public void TestUnregisterVoteRequestHandlerWithExistingNonVotePerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 1 } });
			
			var person = personEntity.ToObject();

			Assert.That(!this.server.UnregisterVoteRequestHandler("test client", person));
		}

		/// <summary>
		/// Test unregistration of a person who have 
		/// voted before and is a valid voter.
		/// </summary>
		[Test]
		public void TestUnregisterVoteRequestHandlerWithExistingVotePerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 2 } });
			
			var person = personEntity.ToObject();

			Assert.That(this.server.UnregisterVoteRequestHandler("test client", person));

			var logEntity = personEntity.GetMostRecentLog();
			logEntity.Delete();
		}

		/// <summary>
		/// Test unregistration with a non-existing person.
		/// </summary>
		[Test]
		public void TestUnregisterVoteRequestHandlerWitUnexistingPerson() {
			var personEntity = new PersonEntity();
			personEntity.Load(new Hashtable { { "id", 669 } });
			
			var person = personEntity.ToObject();

			Assert.That(!this.server.UnregisterVoteRequestHandler("test client", person));
		}

		/// <summary>
		/// Test that the server returns the correct tables
		/// from the data source and that it does not return
		/// the same table twice. 
		/// </summary>
		[Test]
		public void TestValidTableRequestHandler() {
			var tables = this.server.ValidTableRequestHandler("test client");

			var tablesList = new List<string>(tables);

			Assert.That(tablesList.Count == 3);
			Assert.That(tablesList.Contains("Table of Win"));
			Assert.That(tablesList.Contains("Table of Fish"));
			Assert.That(tablesList.Contains("Table of Calmness"));
		}
	}
}