namespace SmallTuba.Entities {
	using SmallTuba.Entities.Abstracts;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-07</version>
	public class LogDataAccessObject : AbstractDataAccessObject {
		public LogDataAccessObject() {
			Table = LogEntity.Table;
			Columns = LogEntity.Columns;
		}
	}
}
