namespace SmallTuba.DataGenerator {
	using System.Xml;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The data generator uses information given
	/// by a DataSource object to generate xml data
	/// about municipalities, polling venues, voters
	/// and polling tables.
	/// </summary>
	public class DataGenerator {
		private readonly DataSource dataSource;

		public DataGenerator() {
			this.dataSource = new DataSource();
			this.dataSource.Load();
		}

		public int NumberOfMunicipalities { get; set; }
		public int NumberOfPollingVenues { get; set; }
		public int NumberOfVoters { get; set; }

		public string FileDestination { get; set; }

		/// <summary>
		/// Generate a xml document containing information
		/// about voters taking the given parameters
		/// into consideration.
		/// </summary>
		public void Generate() {
			XmlDocument doc = new XmlDocument();

			XmlDeclaration dec = doc.CreateXmlDeclaration("1.0", null, null);
			dec.Encoding = "UTF-8";

			doc.AppendChild(dec);

			doc = GenerateMunicipalites(doc);

			doc.Save(FileDestination);
		}

		/// <summary>
		/// Generate the municipality xml elements.
		/// </summary>
		/// <param name="doc">The document of which the elements should be attached.</param>
		/// <returns>The document with the newly attached elements.</returns>
		private XmlDocument GenerateMunicipalites(XmlDocument doc) {
			XmlElement municipalities = doc.CreateElement("Municipalities");

			for (var i = 0; i < this.NumberOfMunicipalities; i++) {
				XmlElement municipality = doc.CreateElement("Municipality");

				var muni = this.dataSource.GetMunicipality();

				XmlElement name = doc.CreateElement("Name");
				name.InnerText = muni[0];
				municipality.AppendChild(name);

				XmlElement street = doc.CreateElement("Street");
				street.InnerText = muni[1];
				municipality.AppendChild(street);

				XmlElement city = doc.CreateElement("City");
				city.InnerText = muni[2];
				municipality.AppendChild(city);

				municipality.AppendChild(this.GeneratePollingVenues(doc, muni));

				municipalities.AppendChild(municipality);
			}

			doc.AppendChild(municipalities);

			return doc;
		}

		/// <summary>
		/// Generate the polling venue xml elements.
		/// </summary>
		/// <param name="doc">The document of which the elements should be attached.</param>
		/// <param name="municipality">The information of the municipality the polling venues should be in.</param>
		/// <returns>The document with the newly attached elements.</returns>
		private XmlElement GeneratePollingVenues(XmlDocument doc, string[] municipality) {
			XmlElement pollingVenues = doc.CreateElement("PollingVenues");

			for (var i = 0; i < this.NumberOfPollingVenues; i++) {
				XmlElement pollingVenue = doc.CreateElement("PollingVenue");

				var venue = this.dataSource.GetPollingVenue(municipality);
				
				XmlElement name = doc.CreateElement("Name");
				name.InnerText = venue[0];
				pollingVenue.AppendChild(name);

				XmlElement street = doc.CreateElement("Street");
				street.InnerText = venue[1];
				pollingVenue.AppendChild(street);

				XmlElement city = doc.CreateElement("City");
				city.InnerText = venue[2];
				pollingVenue.AppendChild(city);

				pollingVenue.AppendChild(this.GenerateVoters(doc, venue));

				pollingVenues.AppendChild(pollingVenue);
			}

			return pollingVenues;
		}

		/// <summary>
		/// Generate the voter xml elements.
		/// </summary>
		/// <param name="doc">The document of which the elements should be attached.</param>
		/// <param name="venue">The information of the polling venue the voters should be in.</param>
		/// <returns>The document with the newly attached elements.</returns>
		private XmlElement GenerateVoters(XmlDocument doc, string[] venue) {
			XmlElement voters = doc.CreateElement("Voters");

			for (var i = 0; i < this.NumberOfVoters; i++) {
				XmlElement voter = doc.CreateElement("Voter");

				var person = this.dataSource.GetVoter(venue);
				
				XmlElement firstName = doc.CreateElement("FirstName");
				firstName.InnerText = person[0];
				voter.AppendChild(firstName);

				XmlElement lastName = doc.CreateElement("LastName");
				lastName.InnerText = person[1];
				voter.AppendChild(lastName);

				XmlElement street = doc.CreateElement("Street");
				street.InnerText = person[2];
				voter.AppendChild(street);

				XmlElement city = doc.CreateElement("City");
				city.InnerText = person[3];
				voter.AppendChild(city);

				XmlElement cprNo = doc.CreateElement("CprNo");
				cprNo.InnerText = person[4];
				voter.AppendChild(cprNo);

				XmlElement pollingTable = doc.CreateElement("PollingTable");
				pollingTable.InnerText = person[5];
				voter.AppendChild(pollingTable);

				voters.AppendChild(voter);
			}

			return voters;
		}
	}
}
