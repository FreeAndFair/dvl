namespace SmallTuba.Entities {
	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	public class PersonDataAccessObject : AbstractDataAccessObject {
		public PersonDataAccessObject() {
			Table = PersonEntity.Table;
			Columns = PersonEntity.Columns;
		}
	}
}
