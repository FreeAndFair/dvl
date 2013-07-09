namespace SmallTuba.Entities.Abstracts {
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;

	using SmallTuba.Database;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	/// <summary>
	/// The AbstractResource is a rather primitive class.
	/// It provides some general methods used in resources,
	/// including SetOrder, SetLimit, SetOffset, GetCount
	/// and GetCountTotal.
	/// 
	/// The class is not meant for instantiation but must
	/// be inherited by a real entity resource.
	/// </summary>
	public abstract class AbstractResource<T> {
		protected QueryBuilder QueryBuilder;
		
		/// <summary>
		/// Construct the object and instantiance an
		/// instance of QueryBuilder to be used for querying.
		/// </summary>
		protected AbstractResource() {
			this.QueryBuilder = new QueryBuilder();
		}

		public void SetOrder(string order, string direction) {
			Contract.Requires(order != null);
			Contract.Requires(direction != null);
			Contract.Requires(direction == "asc" || direction == "desc");

			this.QueryBuilder.AddOrder(order, direction);
		}

		public void SetLimit(int limit) {
			Contract.Requires(limit > 0);

			this.QueryBuilder.SetLimit(limit);
		}

		public void SetOffset(int offset) {
			Contract.Requires(offset >= 0);

			this.QueryBuilder.SetOffset(offset);
		}

		public void SetGroupBy(string groupBy) {
			Contract.Requires(groupBy != null);

			this.QueryBuilder.SetGroupBy(groupBy);
		}

		public int GetCount() {
			return this.QueryBuilder.GetCount();
		}

		public int GetCountTotal() {
			return this.QueryBuilder.GetCountTotal();
		}

		/// <summary>
		/// Method must be overridden in child class.
		/// </summary>
		/// <returns>A list of the entities build.</returns>
		public abstract List<T> Build();
	}
}