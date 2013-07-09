namespace SmallTuba.DataGenerator {
	using System;
	using System.Collections.Generic;
	using System.IO;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The data source gathers information from
	/// a range of .txt files on the harddisk.
	/// 
	/// These files contain information about
	/// municipalities, polling venue names, first 
	/// names of men, first names of women, last
	/// names and streets.
	/// 
	/// It can also generate fake, invalid cpr numbers
	/// and birthdays.
	/// </summary>
	public class DataSource {
		private readonly Random random;

		/// <summary>
		/// Create a new data source.
		/// </summary>
		public DataSource() {
			this.random = new Random();

			this.FirstNamesMen = new List<string>();
			this.FirstNamesWomen = new List<string>();
			this.LastNames = new List<string>();
			this.Municipalities = new List<string>();
			this.PollingVenues = new List<string>();
			this.Streets = new List<string>();
		}

		private List<string> FirstNamesMen { get; set; }
		private List<string> FirstNamesWomen { get; set; }
		private List<string> LastNames { get; set; }
		private List<string> Municipalities { get; set; }
		private List<string> PollingVenues { get; set; }
		private List<string> Streets { get; set; }

		/// <summary>
		/// Create a random number between 1 and limit.
		/// </summary>
		/// <param name="limit">The upper bound of the random number.</param>
		/// <returns>A random integer.</returns>
		public int RandomNumber(int limit) {
			return this.RandomNumber(1, limit);
		}

		/// <summary>
		/// Creata random number between the given
		/// upper and lower bound. The lower bound
		/// most be positive.
		/// </summary>
		/// <param name="offset">The lower bound of the random number.</param>
		/// <param name="limit">The upper bound of the random number</param>
		/// <returns>A random integer.</returns>
		public int RandomNumber(int offset, int limit) {
			return this.random.Next(offset > -1 ? offset : 0, limit);
		}

		/// <summary>
		/// Get a random municipality.
		/// </summary>
		/// <returns>A municipality.</returns>
		public string[] GetMunicipality() {
			var index = this.RandomNumber(this.Municipalities.Count);
			var muni = this.Municipalities[index];

			var munis = muni.Split(' ');
			var name = munis[0]+" "+munis[1];

			var street = this.GenerateStreet();
			var city = (munis.Length == 3) ? munis[2]+" "+munis[0] : munis[0];

			return new[] { name, street, city };
		}

		/// <summary>
		/// Get a polling venue in the given municipality.
		/// </summary>
		/// <returns>A polling venue.</returns>
		public string[] GetPollingVenue(string[] municipality) {
			var index = this.RandomNumber(this.PollingVenues.Count);

			var venue = this.PollingVenues[index];
			var street = this.GenerateStreet();
			var city = municipality[2];

			return new[] { venue, street, city };
		}

		/// <summary>
		/// Get a voter in the given polling venue.
		/// </summary>
		/// <returns>A voter.</returns>
		public string[] GetVoter(string[] venue) {
			var gender = (this.RandomNumber(0, 2) % 2 == 1);
			var index = this.RandomNumber(this.LastNames.Count);

			string firstName;
			var lastName = this.LastNames[index];
			var cprNo = this.GenerateBirthday();
			var street = this.GenerateStreet();
			var city = venue[2];
			var pollingTable = this.RandomNumber(20)+"";

			if (gender) {
				index = this.RandomNumber(this.FirstNamesMen.Count);
				firstName = this.FirstNamesMen[index];
				cprNo += this.GenerateCprMen();
			} else {
				index = this.RandomNumber(this.FirstNamesWomen.Count);
				firstName = this.FirstNamesWomen[index];
				cprNo += this.GenerateCprWomen();
			}
			
			return new[] { firstName, lastName, street, city, cprNo, pollingTable };
		}

		/// <summary>
		/// Generate a birthday.
		/// </summary>
		/// <returns>A valid birthday between 1911 and 1993.</returns>
		private string GenerateBirthday() {
			return string.Format("{0:d2}", this.RandomNumber(1, 30))+
				string.Format("{0:d2}", this.RandomNumber(1, 12)) + 
				string.Format("{0:d2}", this.RandomNumber(11, 93));
		}

		/// <summary>
		/// A male cpr number.
		/// </summary>
		/// <returns>The generated cpr number.</returns>
		private string GenerateCprMen() {
			var cpr = this.RandomNumber(1858, 2057);

			if (cpr % 2 == 0) {
				cpr++;
			}

			return cpr+"";
		}

		/// <summary>
		/// A female cpr number.
		/// </summary>
		/// <returns>The generated cpr number.</returns>
		private string GenerateCprWomen() {
			var cpr = this.RandomNumber(1858, 2057);

			if (cpr % 2 == 1) {
				cpr++;
			}

			return cpr+"";
		}

		/// <summary>
		/// Generate a street with a number.
		/// </summary>
		/// <returns>A street with a number.</returns>
		private string GenerateStreet() {
			var index = this.RandomNumber(Streets.Count);
			var street = Streets[index];

			street += " "+this.RandomNumber(200);

			return street;
		}

		/// <summary>
		/// Load the data from the .txt files.
		/// </summary>
		public void Load() {
			this.LoadFirstNamesMen();
			this.LoadFirstNamesWomen();
			this.LoadLastNames();
			this.LoadMunicipalities();
			this.LoadPollingVenues();
			this.LoadStreets();
		}

		/// <summary>
		/// Load male firstnames.
		/// </summary>
		private void LoadFirstNamesMen() {
			var sr = new StreamReader("../../../data/firstnames-men.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.FirstNamesMen.Add(line);
			}
		}

		/// <summary>
		/// Load female firstnames.
		/// </summary>
		private void LoadFirstNamesWomen() {
			var sr = new StreamReader("../../../data/firstnames-women.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.FirstNamesWomen.Add(line);
			}
		}

		/// <summary>
		/// Load lastnames.
		/// </summary>
		private void LoadLastNames() {
			var sr = new StreamReader("../../../data/lastnames.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.LastNames.Add(line);
			}
		}

		/// <summary>
		/// Load municipalities.
		/// </summary>
		private void LoadMunicipalities() {
			var sr = new StreamReader("../../../data/municipalities.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.Municipalities.Add(line);
			}
		}

		/// <summary>
		/// Load polling venues.
		/// </summary>
		private void LoadPollingVenues() {
			var sr = new StreamReader("../../../data/polling-venues.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.PollingVenues.Add(line);
			}
		}

		/// <summary>
		/// Load streets.
		/// </summary>
		private void LoadStreets() {
			var sr = new StreamReader("../../../data/streets.txt");

			string line;
			while ((line = sr.ReadLine()) != null) {
				this.Streets.Add(line);
			}
		}
	}
}
