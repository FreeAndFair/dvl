namespace SmallTubaTestSuite.Entities {
	using System;
	using System.Collections;

	using NUnit.Framework;

	using SmallTuba.Entities;
	using SmallTuba.Utility;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// For testing the PersonEntity object and its
	/// fellow objects; PersonValueObject,
	/// PersonDataAccessObject and PersonResource.
	/// 
	/// Also tests the LogEntity, its fellow
	/// objects and the abstracts of the entity
	/// objects.
	/// 
	/// To use the test, one needs to make sure that
	/// the Connector class can establish connection
	/// to the external database - otherwise the 
	/// tests will fail! See the ConnectorTestSuite
	/// for more information.
	/// </summary>
	[TestFixture]
	public class EntityTestSuite {
		/// <summary>
		/// Set external data source debug mode to true.
		/// </summary>
		[SetUp]
		public void SetUp() {
			Debug.ExternalDataSources = true;
		}

		/// <summary>
		/// Set external data source debug mode to false.
		/// </summary>
		[TearDown]
		public void TearDown() {
			Debug.ExternalDataSources = false;
		} 

		/// <summary>
		/// Test the shallow Person object.
		/// </summary>
		[Test]
		public void TestPersonObject() {
			var person = new PersonEntity();
			person.Load(new Hashtable { { "id", 2 } } );

			var a = person.ToObject();
			var b = person.ToObject();

			var c = person.ToObject();
			c.FirstName = "George";

			var d = c;

			Assert.That(a.Equals(b));
			Assert.That(a.Equals((object) b));

			Assert.False(c.Equals(b));
			Assert.False(c.Equals((object) b));

			Assert.That(c.Equals(d));
			Assert.That(c.Equals((object) d));

			Assert.False(c.Equals((Person) null));
			Assert.False(c.Equals((object) null));
			Assert.False(c.Equals(new PersonEntity()));


			Assert.That(a.GetHashCode() == -1070491939);

			Assert.That(a.ToString() == "2,0123456789,Christian,Olsson, Table of Fish, 08-12-2011 10:08:41, True");
		}

		/// <summary>
		/// Test the creation, updating and deletion
		/// of a PersonEntity.
		/// </summary>
		[Test]
		public void TestPersonCreationUpdatingAndDeletion() {
			var person = new PersonEntity();
			person.Firstname = "Jan";
			person.Lastname = "Aagaard Meier";
			person.Cpr = "0123456789";
			person.VoterId = 1337;
			person.PollingVenue = "ITU";
			person.PollingTable = "42";

			person.Save();

			person = new PersonEntity();
			person.Load(new Hashtable { { "polling_table", "42" } });
			
			Assert.That(person.Exists());
			Assert.That(person.Firstname == "Jan");
			Assert.That(person.Lastname == "Aagaard Meier");
			Assert.That(person.Cpr == "0123456789");
			Assert.That(person.VoterId == 1337);
			Assert.That(person.PollingVenue == "ITU");
			Assert.That(person.PollingTable == "42");

			person.Firstname = "Niels";
			person.VoterId = 314;

			person.Save();

			person = new PersonEntity();
			person.Load(new Hashtable { { "polling_table", "42" } });
			
			Assert.That(person.Exists());
			Assert.That(person.Firstname == "Niels");
			Assert.That(person.Lastname == "Aagaard Meier");
			Assert.That(person.Cpr == "0123456789");
			Assert.That(person.VoterId == 314);
			Assert.That(person.PollingVenue == "ITU");
			Assert.That(person.PollingTable == "42");

			person.Delete();
			
			person = new PersonEntity();
			person.Load(new Hashtable { { "polling_table", "42" } });

			Assert.That(!person.Exists());
		}

		/// <summary>
		/// Test the creation, updating and deletion
		/// of a LogEntity.
		/// </summary>
		[Test]
		public void TestLogCreationUpdatingAndDeletion() {
			var log = new LogEntity {
				PersonDbId = 1,
				Action = "register",
				Client = "someclient 8",
				PollingTable = "8",
				Timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
			};

			log.Save();
			
			log = new LogEntity();
			log.Load(new Hashtable { { "client", "someclient 8" } });

			Assert.That(log.Exists());
			Assert.That(log.PersonDbId == 1);
			Assert.That(log.Action == "register");
			Assert.That(log.Client == "someclient 8");
			Assert.That(log.PollingTable == "8");
			Assert.That(log.Timestamp > 0);
			
			log.Action = "unregister";
			log.PollingTable = "5";

			log.Save();
			
			log = new LogEntity();
			log.Load(new Hashtable { { "client", "someclient 8" } });
			
			Assert.That(log.Exists());
			Assert.That(log.PersonDbId == 1);
			Assert.That(log.Action == "unregister");
			Assert.That(log.Client == "someclient 8");
			Assert.That(log.PollingTable == "5");
			Assert.That(log.Timestamp > 0);

			log.Delete();
			
			log = new LogEntity();
			log.Load(new Hashtable { { "client", "someclient 8" } });

			Assert.That(!log.Exists());
		}

		/// <summary>
		/// Test the interaction from PersonEntity
		/// to LogEntity
		/// </summary>
		[Test]
		public void TestPersonAndLogInteractivity() {
			var person = new PersonEntity();
			person.Load(new Hashtable { { "id", 1 } });

			Assert.That(true);

			var logsBefore = person.GetLogs();
			var mostRecentLogBefore = person.GetMostRecentLog();

			Assert.That(logsBefore.Count == 0);
			Assert.That(mostRecentLogBefore == null);

			var logA = new LogEntity {
				PersonDbId = person.DbId,
				Action = "register",
				Client = "someotherclient 8",
				PollingTable = "8",
				Timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds
			};

			logA.Save();

			Assert.That(logA.Exists());

			var logB = new LogEntity {
				PersonDbId = person.DbId,
				Action = "unregister",
				Client = "someotherclient 8",
				PollingTable = "8",
				Timestamp = (int) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds + 1
			};

			logB.Save();

			Assert.That(logB.Exists());

			var logsAfter = person.GetLogs();
			var mostRecentLogAfter = person.GetMostRecentLog();

			Assert.That(logsAfter.Count == 2);

			Assert.That(mostRecentLogAfter != null);
			Assert.That(mostRecentLogAfter.Exists());
			Assert.That(mostRecentLogAfter.Action == "unregister");

			logA.Delete();
			logB.Delete();

			Assert.That(!logA.Exists());
			Assert.That(!logB.Exists());
		}

		/// <summary>
		/// Testing the methods in the PersonResource.
		/// </summary>
		[Test]
		public void TestPersonResource() {
			var resource = new PersonResource();
			resource.SetFirstname("Henrik");
			resource.SetLastname("Haugbølle");
			resource.SetCpr("0123456789");
			resource.SetVoterId(3306);
			resource.SetPollingVenue("Venue of Awesome");
			resource.SetPollingTable("Table of Win");
			
			var result = resource.Build();

			Assert.That(resource.GetCount() == 1);
			Assert.That(resource.GetCountTotal() == 1);

			Assert.That(result[0].Firstname == "Henrik");
		}

		/// <summary>
		/// Testing the methods in the LogResource.
		/// </summary>
		[Test]
		public void TestLogResource() {
			var person = new PersonEntity();
			person.Load(new Hashtable { { "id", 2 } });

			var resource = new LogResource();
			resource.SetPerson(person);
			
			var result = resource.Build();

			Assert.That(resource.GetCount() == 1);
			Assert.That(resource.GetCountTotal() == 1);

			Assert.That(result[0].Action == "register");
		}

		/// <summary>
		/// Test the AbstractResource through the
		/// PersonResource.
		/// 
		/// Testing the methods SetOrder and SetLimit.
		/// </summary>
		[Test]
		public void TestAbstractResourceOrderAndLimit() {
			var resource = new PersonResource();
			resource.SetOrder("id", "desc");
			resource.SetLimit(2);

			var result = resource.Build();

			Assert.That(resource.GetCount() == 2);
			Assert.That(resource.GetCountTotal() == 3);

			Assert.That(result[0].DbId == 3);
			Assert.That(result[1].DbId == 2);
		}

		/// <summary>
		/// Test the AbstractResource through the
		/// PersonResource.
		/// 
		/// Testing the method SetGroupBy.
		/// </summary>
		[Test]
		public void TestAbstractResourceGroupBy() {
			var resource = new PersonResource();
			resource.SetGroupBy("cpr");

			var result = resource.Build();

			Assert.That(resource.GetCount() == 1);
			Assert.That(resource.GetCountTotal() == 1);
		}

		/// <summary>
		/// Test the AbstractResource through the
		/// PersonResource.
		/// 
		/// Testing the methods SetOffset and SetLimit;
		/// </summary>
		[Test]
		public void TestAbstractResourceLimitAndOffset() {
			var resource = new PersonResource();
			resource.SetOffset(1);
			resource.SetLimit(1);
			resource.SetOrder("id", "asc");

			var result = resource.Build();

			Assert.That(resource.GetCount() == 1);
			Assert.That(resource.GetCountTotal() == 3);

			Assert.That(result[0].Firstname == "Christian");
		}
	}
}

