namespace SmallTuba.Entities.Abstracts {
	using System.Collections;
	using System.Diagnostics.Contracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	/// <summary>
	/// The AbstractValueObject is a rather primitive class
	/// holding a various range of values for any entity.
	/// 
	/// It is possible to set and get values by using the
	/// indexer of the AbstractValueObject or it is possible
	/// to set and get multiple/all values of the instance
	/// by using the herefore provided methods.
	/// 
	/// The class needs to be inherited to be used, because
	/// the protected Columns field needs to be initialized 
	/// with a range of valid keys.
	/// 
	/// One cannot set or get values of which the key is
	/// not contained in the Columns array.
	/// </summary>
	public abstract class AbstractValueObject {
		protected string[] Columns;
		private readonly Hashtable values;
		
		/// <summary>
		/// Constructs the object.
		/// </summary>
		protected AbstractValueObject () {
			this.values = new Hashtable();
		}
		
		/// <summary>
		/// The indexer of the object. When a value is set 
		/// or get, one must ensure that the key used is contained
		/// in the Columns array.
		/// </summary>
		/// <param name="key">The key of the value.</param>
		/// <returns>The value of the key.</returns>
		public object this[string key] {
			get { return this.values[key];  }
			set { this.values[key] = value;  }
		}
		
		/// <summary>
		/// Set multiple values. The method will iterate
		/// through the Hashtable given and try to set the
		/// values using the indexer of the object.
		/// 
		/// If a key/value is already set in the ValueObject
		/// and the values given does not contain its key, it 
		/// will not be overwritten.
		/// </summary>
		/// <param name="values">The values to be set.</param>
		public void SetValues(Hashtable values) {
			Contract.Requires(values != null);

			var enumerator = values.GetEnumerator();
			
			while (enumerator.MoveNext()) {
				this[(string) enumerator.Key] = enumerator.Value;
			}
		}
		
		/// <summary>
		/// Get all the values of the ValueObject.
		/// </summary>
		/// <returns>All the values of the ValueObject.</returns>
		public Hashtable GetValues() {
			return values;
		}
	}
}