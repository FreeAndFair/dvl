namespace SmallTubaTestSuite.Database {
	using System.Collections;
	using NUnit.Framework;
	using SmallTuba.Database;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// TestSuite for the Connector class. To use the
	/// class one most make sure that the Connector
	/// can actually connect to a valid MySQL database.
	/// See the Connector class for more information.
	/// 
	/// The database must contain a table called
	/// "PersonTestSuite" and the following columns:
	/// 
	/// id				INT			11
	/// firstname		VARCHAR		100
	/// lastname		VARCHAR		100
	/// cpr				VARCHAR		10
	/// voter_id		INT			11
	/// polling_venue	VARCHAR		100
	/// polling_table	VARCHAR		100
	/// 
	/// And the data as represented below:
	/// 
	/// id	firstname	lastname		cpr			voter_id	polling_venue		polling_table
	/// 1	Henrik		Haugbølle		0123456789	3306		Venue of Awesome	Table of Win
	/// 2	Christian	Olsson			0123456789	8889		Venue of Shame		Table of Fish
	/// 3	Kåre		Sylow Pedersen	0123456789	8080		Venue of Anger		Table of Calmness
	/// 
	/// Otherwise the test suite will fail the tests.
	/// </summary>
	[TestFixture]
	public class ConnectorTestSuite {
		private Connector connector;

		/// <summary>
		/// Instantiate the Connector and call the
		/// Connect() method to connect to the database.
		/// </summary>
		[SetUp]
		public void SetUp() {
			this.connector = Connector.GetConnector();
			this.connector.Connect();
		}

		/// <summary>
		/// Close the connection to the database by 
		/// calling the Disconnect() method.
		/// </summary>
		[TearDown]
		public void TearDown() {
			this.connector.Disconnect();
		} 

		/// <summary>
		/// Execute a SELECT query towards the database. Fetch
		/// all the content from the PersonTestSuite table and
		/// check that both id and firstnames matches.
		/// </summary>
		[Test]
		public void TestExecuteQuery() {
			var results = this.connector.ExecuteQuery("SELECT * FROM `PersonTestSuite`;");
			
			Assert.That(((int) ((Hashtable) results[0])["id"]) == 1);
			Assert.That(((string) ((Hashtable) results[0])["firstname"]) == "Henrik");

			Assert.That(((int) ((Hashtable) results[1])["id"]) == 2);
			Assert.That(((string) ((Hashtable) results[1])["firstname"]) == "Christian");

			Assert.That(((int) ((Hashtable) results[2])["id"]) == 3);
			Assert.That(((string) ((Hashtable) results[2])["firstname"]) == "Kåre");

			Assert.That(this.connector.GetCount() == 3);
			Assert.That(this.connector.GetCountTotal() == 3);
		}

		/// <summary>
		/// Execute an UPDATE statement towards the database,
		/// and check with a SELECT statement that the update
		/// query was executed correctly.
		/// 
		/// Afterwards reverse the update and check it, so that
		/// it is possible to re-run the test.
		/// </summary>
		[Test]
		public void TestExecuteNoneQuery() {
			this.connector.ExecuteNoneQuery("UPDATE PersonTestSuite SET `firstname` = 'Henrik Haugbølle' WHERE `id` = 1 LIMIT 1;");

			var results = this.connector.ExecuteQuery("SELECT * FROM `PersonTestSuite` WHERE `id` = 1 LIMIT 1;");
			
			Assert.That(((int) ((Hashtable) results[0])["id"]) == 1);
			Assert.That(((string) ((Hashtable) results[0])["firstname"]) == "Henrik Haugbølle");

			this.connector.ExecuteNoneQuery("UPDATE PersonTestSuite SET `firstname` = 'Henrik' WHERE `id` = 1 LIMIT 1;");
			
			results = this.connector.ExecuteQuery("SELECT * FROM `PersonTestSuite` WHERE `id` = 1 LIMIT 1;");
			
			Assert.That(((int) ((Hashtable) results[0])["id"]) == 1);
			Assert.That(((string) ((Hashtable) results[0])["firstname"]) == "Henrik");
		}
	}
}