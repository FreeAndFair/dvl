namespace SmallTuba.Entities.Abstracts {
	using System.Collections;
	using System.Diagnostics.Contracts;
	using SmallTuba.Database;
	
	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	/// <summary>
	/// The AbstractDataAccessObject generalizes the methods of a DataAccessObject for
	/// any given entity. The Load, Save and Delete methods should cover most uses but
	/// is suitable for overwriting if nessecary.
	/// 
	/// The AbstractDataAccessObject manages the communication to and from the external
	/// data source by loading, saving and deleting entities to and from it.
	/// 
	/// The AbstractDataAccessObject is only useable when inherit from, because the
	/// Table and Columns fields should be initialized to match a specific entity, which
	/// is not done by the AbstractDataAccessObject itself.
	/// </summary>
	public abstract class AbstractDataAccessObject {
		protected QueryBuilder QueryBuilder;
		public string Table;
		public string[] Columns;

		/// <summary>
		/// Constructor does nothing but initializing an
		/// instance of the QueryBuilder.
		/// </summary>
		protected AbstractDataAccessObject() {
			this.QueryBuilder = new QueryBuilder();
		}
		
		/// <summary>
		/// Loads and returns the values of an entity given a range of parameters. The 
		/// parameters is given as a Hashtable where the key must be the column 
		/// and the value must be the value the column must have.
		/// </summary>
		/// <example>
		/// // The following will load the values for a Person entity with id 8
		/// Hashtable values = PersonDataAccessObject.Load(new Hashtable { { "id", 8 } });
		/// </example>
		/// <param name="parameters">Conditions to be taking into consideration.</param>
		/// <returns>A Hashtable containing the values to build the entity.</returns>
		public Hashtable Load(Hashtable parameters) {
			Contract.Requires(parameters != null);
			Contract.Requires(parameters.Count > 0);

			this.QueryBuilder.SetType("select");
			this.QueryBuilder.SetTable(Table);
			this.QueryBuilder.SetColumns(Columns);
			this.QueryBuilder.SetLimit(1);

			var enumerator = parameters.GetEnumerator();
			while (enumerator.MoveNext()) {
				this.QueryBuilder.AddCondition("`"+enumerator.Key+"` = '"+enumerator.Value+"'");
			}

			var results = this.QueryBuilder.ExecuteQuery();

			return results.Count > 0 ? (Hashtable) results[0] : new Hashtable();
		}
		
		/// <summary>
		/// Saves the values of a entity. The values must be given as a Hashtable
		/// where the key/value relationship corresponds to the column/value
		/// relationship in the database.
		/// </summary>
		/// <param name="values">The values to be saved.</param>
		public int Save(Hashtable values) {
			Contract.Requires(values != null);
			Contract.Requires(values.Count > 0);

			var valuesFiltered = new string[Columns.Length];

			for (var i = 0; i < Columns.Length; i++) {
				if (values.ContainsKey(Columns[i])) {
					valuesFiltered[i] = values[Columns[i]].ToString();
				}
			}
			
			this.QueryBuilder.SetTable(Table);
			this.QueryBuilder.SetColumns(Columns);
			this.QueryBuilder.SetValues(valuesFiltered);
			this.QueryBuilder.SetLimit(1);

			if (values.ContainsKey("id") && values["id"].ToString() != "" && (int) values["id"] > 0) {
				this.QueryBuilder.SetType("update");
				this.QueryBuilder.AddCondition("`id` = '" + (int)values["id"] + "'");
			} else {
				this.QueryBuilder.SetType("insert");
			}

			return this.QueryBuilder.ExecuteNoneQuery();
		}
		
		/// <summary>
		/// Deletes a single entity by a given id.
		/// </summary>
		/// <param name="id">The id of the entity to be deleted.</param>
		public void Delete(int id) {
			Contract.Requires(id > 0);

			this.QueryBuilder.SetType("delete");
			this.QueryBuilder.SetTable(Table);
			this.QueryBuilder.AddCondition("`id` = '" + id + "'");
			this.QueryBuilder.SetLimit(1);

			this.QueryBuilder.ExecuteNoneQuery();
		}
	}
}
