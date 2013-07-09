namespace SmallTuba.Entities {
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The Person class represents a real-life person, with
	/// relevant-to-the-system information associated such as
	/// the persons name and CPR number.
	/// </summary>
	public class PersonEntity : AbstractEntity {
		public static readonly string Table = "Person";
		public static readonly string[] Columns = { "id", "firstname", "lastname", "cpr", "voter_id", "polling_venue", "polling_table" };

		private List<LogEntity> logs;

		/// <summary>
		/// Create a new person entity object.
		/// </summary>
		public PersonEntity() {
			this.ValueObject = new PersonValueObject();
			this.DataAccessObject = new PersonDataAccessObject();

			logs = null;
		}

		/// <summary>
		/// Create a new person entity and set
		/// the given values on it.
		/// </summary>
		/// <param name="values">The values which the person entity should be created with.</param>
		public PersonEntity(Hashtable values) : this() {
			Contract.Requires(values != null);

			this.ValueObject.SetValues(values);
		}

		public string Firstname { 
			get { return this.ValueObject["firstname"] != null ? (string) this.ValueObject["firstname"] : ""; } 
			set { this.ValueObject["firstname"] = value; }
		}
		public string Lastname { 
			get { return this.ValueObject["lastname"] != null ? (string) this.ValueObject["lastname"] : ""; } 
			set { this.ValueObject["lastname"] = value; }
		}
		public string Cpr { 
			get { return this.ValueObject["cpr"] != null ? (string) this.ValueObject["cpr"] : ""; } 
			set { this.ValueObject["cpr"] = value; }
		}
		public int VoterId { 
			get { return this.ValueObject["voter_id"] != null ? (int) this.ValueObject["voter_id"] : 0; } 
			set { this.ValueObject["voter_id"] = value; }
		}
		public string PollingVenue { 
			get { return this.ValueObject["polling_venue"] != null ? (string) this.ValueObject["polling_venue"] : ""; } 
			set { this.ValueObject["polling_venue"] = value; }
		}
		public string PollingTable { 
			get { return this.ValueObject["polling_table"] != null ? (string) this.ValueObject["polling_table"] : ""; } 
			set { this.ValueObject["polling_table"] = value; }
		}

		public bool Voted {
			get { return this.GetMostRecentLog() != null && this.GetMostRecentLog().Action == "register"; }
		}

		public int VotedTime {
			get { return this.GetMostRecentLog() != null && this.GetMostRecentLog().Action == "register" ? this.GetMostRecentLog().Timestamp : 0; }
		}

		public string VotedPollingTable {
			get { return this.GetMostRecentLog() != null && this.GetMostRecentLog().Action == "register" ? this.GetMostRecentLog().PollingTable : ""; }
		}

		/// <summary>
		/// Get all log entities in the database
		/// concerning this person.
		/// </summary>
		/// <returns>All the logs in the database concerning this person.</returns>
		public List<LogEntity> GetLogs() {
			if (Exists()) {
				var resource = new LogResource();
				resource.SetPerson(this);
				resource.SetOrder("timestamp", "desc");

				logs = resource.Build();
			}

			return logs;
		}

		/// <summary>
		/// Get the most recent log inserted in the database.
		/// </summary>
		/// <returns>The most recent log entitiy inserted in the database.</returns>
		public LogEntity GetMostRecentLog() {
			return this.GetLogs() != null && this.GetLogs().Count > 0 ? logs[0] : null;
		}

		/// <summary>
		/// Create a Person object from the data currently
		/// stored in the PersonEntity object.
		/// </summary>
		/// <returns>A Person object reflecting this entity.</returns>
		public Person ToObject() {
			return new Person {
			    DbId = DbId,
			    FirstName = Firstname,
			    LastName = Lastname,
			    Cpr = Cpr,
			    VoterId = VoterId,
			    PollingVenue = PollingVenue,
			    PollingTable = PollingTable,
			    Voted = Voted,
			    VotedTime = VotedTime,
				VotedPollingTable = VotedPollingTable,
			    Exists = Exists()
			};
		}
	}
}