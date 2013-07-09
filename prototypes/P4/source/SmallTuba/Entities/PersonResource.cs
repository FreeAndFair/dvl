namespace SmallTuba.Entities {
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The resource of the person. Used to fetch one or more
	/// persons from the external data source.
	/// 
	/// See the AbstractResource for description.
	/// 
	/// Adds the functionality of defining firstname, lastname,
	/// cpr, polling venue, polling table and voter id in the query.
	/// </summary>
	public class PersonResource : AbstractResource<PersonEntity> {

		public PersonResource SetFirstname(string firstname) {
			Contract.Requires(firstname != null);
			this.QueryBuilder.AddCondition("`firstname` = '"+firstname+"'");

			return this;
		}

		public PersonResource SetLastname(string lastname) {
			Contract.Requires(lastname != null);
			this.QueryBuilder.AddCondition("`lastname` = '"+lastname+"'");

			return this;
		}

		public PersonResource SetCpr(string cpr) {
			Contract.Requires(cpr != null);
			this.QueryBuilder.AddCondition("`cpr` = '"+cpr+"'");

			return this;
		}

		public PersonResource SetPollingVenue(string pollingVenue) {
			Contract.Requires(pollingVenue != null);
			this.QueryBuilder.AddCondition("`polling_venue` = '"+pollingVenue+"'");

			return this;
		}

		public PersonResource SetPollingTable(string pollingTable) {
			Contract.Requires(pollingTable != null);
			this.QueryBuilder.AddCondition("`polling_table` = '"+pollingTable+"'");

			return this;
		}

		public PersonResource SetVoterId(int voterId) {
			Contract.Requires(voterId > 0);
			this.QueryBuilder.AddCondition("`voter_id` = '"+voterId+"'");

			return this;
		}

		/// <summary>
		/// Executes the assembled query using the QueryBuilder
		/// object and generates a list of PersonEntity objects.
		/// </summary>
		/// <returns>The list of person objects fetched.</returns>
		public override List<PersonEntity> Build() {
			this.QueryBuilder.SetType("select");
			this.QueryBuilder.SetTable(PersonEntity.Table);
			this.QueryBuilder.SetColumns(PersonEntity.Columns);

			var results = this.QueryBuilder.ExecuteQuery();

			var entities = new List<PersonEntity>();

			foreach (var result in results) {
				entities.Add(new PersonEntity((Hashtable) result));
			}
			
			return entities;
		}
	}
}
