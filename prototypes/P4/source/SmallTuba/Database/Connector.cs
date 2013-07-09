namespace SmallTuba.Database {
	using System;
	using System.Collections;
	using System.Diagnostics.Contracts;
	using MySql.Data.MySqlClient;

	using SmallTuba.Utility;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The Connector is meant to be some kind of proxy/driver for the 
	/// underlying external data source. This Connector interfaces 
	/// a MySQL database.
	/// 
	/// The primary tasks of the Connector class is to establish connection
	/// to the external data source, to terminate the connection, to fetch
	/// data, and to execute queries.
	/// 
	/// The Connector is meant to act like a singleton, so only one instance
	/// is in play at all time. Therefore you have to use the GetConnector()
	/// method for instantiation.
	/// </summary>
	public class Connector {
		private const string Server = "localhost";
		private const string Port = "3306";
		private const string Database = "dirtylion";
		private const string Uid = "dirtylion";
		private const string Password = "abcd1234";

		private MySqlConnection connection;
		private int count;

		private static Connector instance;

		/// <summary>
		/// Empty private constructor for no normal instantiation.
		/// </summary>
		private Connector() { }

		public static Connector GetConnector() {
			return instance ?? (instance = new Connector());
		}

		/// <summary>
		/// Connect to the external data source.
		/// </summary>
		public void Connect() {
			if (!IsConnected()) {
				this.connection = new MySqlConnection("Server=" + Server + ";Port=" + Port + ";Database=" + Database + ";UID=" + Uid + ";Password=" + Password + ";Pooling=false");
				this.count = 0;

				try {
					this.connection.Open();
				} catch (Exception e) {
					Debug.WriteLine(e.ToString());
				}
			}
		}

		/// <summary>
		/// Disconnect from the external data source.
		/// </summary>
		public void Disconnect() {
			Contract.Requires(IsConnected());

			if (this.connection != null) {
				this.connection.Close();
				this.connection = null;
			}
		}

		/// <summary>
		/// Check whether a connection to the database is established.
		/// </summary>
		/// <returns>A boolean which tells whether a connection is established.</returns>
		[Pure]
		public bool IsConnected() {
			return (this.connection != null);
		}

		/// <summary>
		/// Executes a fetch query towards the external data source.
		/// The query has to be a valid MySql SELECT statement.
		/// 
		/// Returns an ArrayList of Hashtables, where each entry in the ArrayList 
		/// corresponds to a row in the data source. The key/value relationship in the
		/// Hashtables corresponds to the column/value releationship in the data source.
		/// </summary>
		/// <param name="query">The query to be executed towards the data source.</param>
		/// <returns>An ArrayList of Hashtables</returns>
		public ArrayList ExecuteQuery(string query) {
			Contract.Requires(IsConnected());

			var command = new MySqlCommand { Connection = this.connection, CommandText = query };
			var reader = command.ExecuteReader();

			var results = new ArrayList();

			while (reader.Read()) {
				var row = new Hashtable();

				for (var i = 0; i < reader.FieldCount; i++) {
					row.Add(reader.GetName(i), reader.GetValue(i));
				} 

				results.Add(row);
			}

			this.count = results.Count;

			reader.Close();

			return results;
		}

		/// <summary>
		/// Executes an UPDATE, INSERT, DELETE or any other non-fetching
		/// SQL statement towards the connected database.
		/// </summary>
		/// <param name="query">The query to be executed towards the data source.</param>
		public int ExecuteNoneQuery(string query) {
			Contract.Requires(IsConnected());

			var command = new MySqlCommand { Connection = this.connection, CommandText = query };
			command.ExecuteNonQuery();

			this.count = 0;

			command = new MySqlCommand { Connection = this.connection, CommandText = "SELECT LAST_INSERT_ID();" };

			return Convert.ToInt32(command.ExecuteScalar());
		}

		/// <summary>
		/// Gets the number of rows fetched from the last fetch-query.
		/// 
		/// Will return 0 if the last query was a none-query.
		/// </summary>
		/// <returns>An integer.</returns>
		public int GetCount() {
			return this.count;
		}
			
		/// <summary>
		/// Gets the number of total rows possible to fetch without 
		/// limitations such as limit and offset from the last fetch-query.
		/// 
		/// Will return 0 if the last query was a none-query.
		/// </summary>
		/// <returns>An integer.</returns>
		public int GetCountTotal() {
			Contract.Requires(IsConnected());

			var command = new MySqlCommand { Connection = this.connection, CommandText = "SELECT FOUND_ROWS();" };

			return Convert.ToInt32(command.ExecuteScalar());
		}
	}
}
		