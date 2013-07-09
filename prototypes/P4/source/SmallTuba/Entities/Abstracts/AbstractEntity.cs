namespace SmallTuba.Entities.Abstracts {
	using System.Collections;
	using System.Diagnostics.Contracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The AbstractEntity provides common load, save and
	/// delete methods for all entities inheriting from it.
	/// 
	/// It also provides the global property Id, which also
	/// entities must have in their data set.
	/// </summary>
	public abstract class AbstractEntity {
		protected AbstractValueObject ValueObject;
		protected AbstractDataAccessObject DataAccessObject;

		/// <summary>
		/// Load the entity by the given parameters.
		/// See the AbstractDataAccessObject for further information.
		/// </summary>
		/// <param name="parameters">The conditions to be taking into consideration.</param>
		public void Load(Hashtable parameters) {
			Contract.Requires(parameters != null);
			Contract.Requires(parameters.Count > 0);

			this.ValueObject.SetValues(this.DataAccessObject.Load(parameters));
		}

		/// <summary>
		/// Save the current entity.
		/// </summary>
		public void Save() {
			var id = this.DataAccessObject.Save(this.ValueObject.GetValues());
			
			this.DbId = this.Exists() ? this.DbId : id;
		}

		/// <summary>
		/// Delete the current entity. The entity must
		/// exists if the method is called.
		/// </summary>
		public void Delete() {
			Contract.Requires(Exists());

			this.DataAccessObject.Delete(DbId);

			this.DbId = 0;
		}

		/// <summary>
		/// Returns whether the entity yet exists in the external data source.
		/// </summary>
		/// <returns>Whether the entity exists.</returns>
		[Pure]
		public bool Exists() {
			return (this.DbId > 0);
		}

		public int DbId { 
			get { return this.ValueObject["id"] != null ? (int) this.ValueObject["id"] : 0; } 
			set { this.ValueObject["id"] = value; }
		}
	}
}
