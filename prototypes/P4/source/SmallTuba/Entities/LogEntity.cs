namespace SmallTuba.Entities {
	using System.Collections;

	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The Log class contains information on commands
	/// executed by a client interacting with a person
	/// entity in the system.
	/// object.
	/// </summary>
	public class LogEntity : AbstractEntity {
		public static readonly string Table = "log";
		public static readonly string[] Columns = { "id", "person_id", "action", "client", "polling_table", "timestamp" };

		/// <summary>
		/// Create a new log entity object.
		/// </summary>
		public LogEntity() {
			this.ValueObject = new LogValueObject();
			this.DataAccessObject = new LogDataAccessObject();
		}

		/// <summary>
		/// Create a new log entity object and set
		/// the given values on it.
		/// </summary>
		/// <param name="values">The values to entity should be created with.</param>
		public LogEntity(Hashtable values) : this() {
			this.ValueObject.SetValues(values);
		}

		public int PersonDbId { 
			get { return this.ValueObject["person_id"] != null ? (int) this.ValueObject["person_id"] : 0; } 
			set { this.ValueObject["person_id"] = value; }
		}
		public string Action { 
			get { return this.ValueObject["action"] != null ? (string) this.ValueObject["action"] : ""; } 
			set { this.ValueObject["action"] = value; }
		}
		public string Client { 
			get { return this.ValueObject["client"] != null ? (string) this.ValueObject["client"] : ""; } 
			set { this.ValueObject["client"] = value; }
		}
		public string PollingTable { 
			get { return this.ValueObject["polling_table"] != null ? (string) this.ValueObject["polling_table"] : ""; } 
			set { this.ValueObject["polling_table"] = value; }
		}
		public int Timestamp { 
			get { return this.ValueObject["timestamp"] != null ? (int) this.ValueObject["timestamp"] : 0; } 
			set { this.ValueObject["timestamp"] = value; }
		}
	}
}