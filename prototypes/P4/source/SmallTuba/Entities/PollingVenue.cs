namespace SmallTuba.Entities {
	using System.Collections.Generic;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// Used to encapsulate information about 
	/// an polling venue. The class contains all
	/// the voters, the address of the polling venue
	/// and the address of the municipality of which
	/// the pollign venue is located in.
	/// </summary>
	public class PollingVenue {
		public List<Person> Persons { get; set; }
		public Address PollingVenueAddress { get; set; }
		public Address MunicipalityAddress { get; set; }
	}
}