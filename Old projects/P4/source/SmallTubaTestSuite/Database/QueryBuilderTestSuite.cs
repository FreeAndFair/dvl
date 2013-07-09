namespace SmallTubaTestSuite.Database {
	using NUnit.Framework;
	using SmallTuba.Database;
	
	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	/// <summary>
	/// Test suite for the QueryBuilder class. The test suite
	/// consists of 4 test, testing the select, update, insert
	/// and delete types of the QueryBuilder.
	/// 
	/// Various parameters such as limits, offsets, orderings,
	/// condition et cetera are tested within these 4 tests.
	/// </summary>
	[TestFixture]
	public class QueryBuilderTestSuite {
		private QueryBuilder queryBuilder;
		
		[SetUp]
		public void SetUp () {
			this.queryBuilder = new QueryBuilder();
		}
		
		/// <summary>
		/// Assembling and testing a SELECT statement.
		/// 
		/// Also tested: table, columns, condition, 
		/// multiple orderings, limit and offset.
		/// </summary>
		[Test]
		public void TestSelect () {
			this.queryBuilder.SetType("select");
			this.queryBuilder.SetTable("person");
			this.queryBuilder.SetColumns(new [] { "id", "firstname", "lastname" });
			this.queryBuilder.AddCondition("firstname = 'henrik'");
			this.queryBuilder.AddCondition("firstname = 'henrik'", "or", "toberemoved");
			this.queryBuilder.RemoveCondition("toberemoved");
			this.queryBuilder.AddOrder("firstname", "desc");
			this.queryBuilder.AddOrder("lastname");
			this.queryBuilder.AddOrder("wrong order", "asc", "toberemoved");
			this.queryBuilder.RemoveOrder("toberemoved");
			this.queryBuilder.SetLimit(10);
			this.queryBuilder.SetOffset(5);
			
			var query = this.queryBuilder.Assemble();
			
			Assert.AreEqual("SELECT SQL_CALC_FOUND_ROWS `id`, `firstname`, `lastname` FROM `person` WHERE (firstname = 'henrik') ORDER BY `firstname` DESC, `lastname` ASC LIMIT 10 OFFSET 5", query);
		}
		
		/// <summary>
		/// Assembling and testing an UPDATE statement.
		/// 
		/// Also tested: table, columns, values and
		/// single condition.
		/// </summary>
		[Test]
		public void TestUpdate () {
			this.queryBuilder.SetType("update");
			this.queryBuilder.SetTable("person");
			this.queryBuilder.SetColumns(new [] { "id", "firstname", "lastname" });
			this.queryBuilder.SetValues(new [] { "4", "henrik", "haugbølle" });
			this.queryBuilder.AddCondition("firstname = 'henrik'");
			
			var query = this.queryBuilder.Assemble();
			
			Assert.AreEqual("UPDATE `person` SET `id` = '4', `firstname` = 'henrik', `lastname` = 'haugbølle' WHERE (firstname = 'henrik')", query);
		}
		
		/// <summary>
		/// Assembling and testing an INSERT statement.
		/// 
		/// Also tested: table, columns and values.
		/// </summary>
		[Test]
		public void TestInsert () {
			this.queryBuilder.SetType("insert");
			this.queryBuilder.SetTable("person");
			this.queryBuilder.SetColumns(new [] { "id", "firstname", "lastname" });
			this.queryBuilder.SetValues(new [] { "4", "henrik", "haugbølle" });
			
			var query = this.queryBuilder.Assemble();
			
			Assert.AreEqual("INSERT INTO `person` ( `id`, `firstname`, `lastname` ) VALUES ( '4', 'henrik', 'haugbølle' )", query);
		}
		
		/// <summary>
		/// Assembling and testing a DELETE statement.
		/// 
		/// Also tested: table and multiple conditions.
		/// </summary>
		[Test]
		public void TestDelete () {
			this.queryBuilder.SetType("delete");
			this.queryBuilder.SetTable("person");
			this.queryBuilder.AddCondition("id = 8", "or");
			this.queryBuilder.AddCondition("id = 6");
			
			var query = this.queryBuilder.Assemble();
			
			Assert.AreEqual("DELETE FROM `person` WHERE (id = 8) OR (id = 6)", query);
		}

		/// <summary>
		/// Assembling and testing a TRUNCATE statement.
		/// 
		/// Also tested: table.
		/// </summary>
		[Test]
		public void TestTruncate () {
			this.queryBuilder.SetType("truncate");
			this.queryBuilder.SetTable("log");
			
			this.queryBuilder.Assemble();

			var query = this.queryBuilder.GetQuery();
			
			Assert.AreEqual("TRUNCATE TABLE `log`", query);
		}
	}
}

