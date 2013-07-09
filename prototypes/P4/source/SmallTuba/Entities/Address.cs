namespace SmallTuba.Entities {
	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// Used to encapsulate information about 
	/// an address, such as the name of the 
	/// "place/venue", the street and the city.
	/// </summary>
	public struct Address {
		public string Name { get; set; }
		public string Street { get; set; }
		public string City { get; set; }        
	}
}
