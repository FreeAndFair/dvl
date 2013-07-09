namespace SmallTuba.Entities {
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The resource of the log. Used to fetch one or more
	/// logs from the external data source.
	/// 
	/// See the AbstractResource for description.
	/// 
	/// Adds the functionality of defining a person entity
	/// to search for logs belonging to.
	/// </summary>
	public class LogResource : AbstractResource<LogEntity> {
		/// <summary>
		/// Set the person the fetched logs must belong to.
		/// </summary>
		/// <param name="personEntity">The person which logs we will fetch.</param>
		/// <returns>The instance of the log resource for chaining purposes.</returns>
		public LogResource SetPerson(PersonEntity personEntity) {
			Contract.Requires(personEntity != null);
			Contract.Requires(personEntity.Exists());

			this.QueryBuilder.AddCondition("`person_id` = '"+personEntity.DbId+"'");

			return this;
		}

		/// <summary>
		/// Executes the assembled query using the QueryBuilder
		/// object and generates a list of LogEntity objects.
		/// </summary>
		/// <returns>The list of log objects fetched.</returns>
		public override List<LogEntity> Build() {
			this.QueryBuilder.SetType("select");
			this.QueryBuilder.SetTable(LogEntity.Table);
			this.QueryBuilder.SetColumns(LogEntity.Columns);

			var results = this.QueryBuilder.ExecuteQuery();

			var entities = new List<LogEntity>();

			foreach (var result in results) {
				entities.Add(new LogEntity((Hashtable) result));
			}
			
			return entities;
		}
	}
}
