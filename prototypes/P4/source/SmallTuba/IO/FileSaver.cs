namespace SmallTuba.IO
{
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.IO;
	using SmallTuba.Entities;
	using SmallTuba.PdfGenerator;

	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// This class exports polling cards, voter lists and data for the local polling venue.
	/// The class creates a new folder when it is instantiated, with the same name as the polling venue, on the path
	/// given as an argument to the constructor. All the file will be saved in this folder.
	/// </summary>
	public class FileSaver{
		private string path;

		/// <summary>
		/// May I have a new file saver for this polling venue and this path?
		/// </summary>
		/// <param name="path">The file path</param>
		/// <param name="pollingVenueName">Name of the polling venue</param>
		public FileSaver(string path, string pollingVenueName){
			Contract.Requires(path!= null);
			Contract.Requires(pollingVenueName != null);
			Contract.Requires(pollingVenueName.Length>0);
			Contract.Ensures(Directory.Exists(path));

			//Creates a new folder with the polling venue name  
			this.path = Directory.CreateDirectory(path+"\\"+pollingVenueName).FullName;
		}

		/// <summary>
		/// Can you devide these voters into voter lists, and save them to this location on the harddrive?
		/// </summary>
		/// <param name="persons">List of persons</param>
		/// <param name="electionName">The name of the election</param>
		/// <param name="electionDate">The date of the election</param>
		public void SaveVoterList(List<Person> persons, string electionName, string electionDate){
			Contract.Requires(persons != null);
			Contract.Ensures(Directory.GetFiles(path, "VoterListTable*.pdf").Length>0);

			Dictionary<string, VoterList> voterlists = this.CreateVoterListsForVenue(persons, electionName, electionDate);
			this.AddVotersToVoterlists(persons, voterlists);
			this.SaveVoterListsToDisk(voterlists);
		}

		/// <summary>
		/// Creates a map of voter lists based on a list of persons.
		/// The total number of different polling tables in the list of 
		/// persons is the number of voterlists in the map
		/// </summary>
		/// <param name="persons">List of persons</param>
		/// <param name="electionName">Name of the election</param>
		/// <param name="electionDate">Date of the election</param>
		/// <returns>A dictionary of voter lists where the key is the polling table number</returns>
		private Dictionary<string, VoterList> CreateVoterListsForVenue(List<Person> persons, string electionName, string electionDate){
			Dictionary<string, VoterList> voterlists = new Dictionary<string, VoterList>();
			foreach (var person in persons){
				//Check that the dictionary already contains the polling table
				if (!voterlists.ContainsKey(person.PollingTable)){
					voterlists.Add(person.PollingTable, new VoterList(50, electionName, electionDate, person.PollingTable));
				}
			}
			return voterlists;
		}

		/// <summary>
		/// Divides a list of persons into there respective voter lists
		/// </summary>
		/// <param name="persons">List of persons</param>
		/// <param name="voterlists">A map of the voter lists where the key is the table numbe</param>
		private void AddVotersToVoterlists(List<Person> persons, Dictionary<string, VoterList> voterlists){
			persons.Sort(Person.NameSort());
			foreach (var person in persons){
				voterlists[person.PollingTable].AddVoter(person);
			}          
		}

		/// <summary>
		/// Saves the voterlists to the diskdrive
		/// </summary>
		/// <param name="voterlists">A map of the voter lists where the key is the table number</param>
		private void SaveVoterListsToDisk(Dictionary<string, VoterList> voterlists){
			foreach (var pollingTable in voterlists.Keys){
				voterlists[pollingTable].SaveToDisk(this.path + "\\" + "VoterListTable"+pollingTable + ".pdf");
			}
		}

		/// <summary>
		/// Can you save the polling cards for this polling venue on the harddrive?
		/// </summary>
		/// <param name="pollingVenue">The polling venue</param>
		/// <param name="electionName">The name of the election</param>
		/// <param name="electionDate">The date of the election</param>
		public void SavePollingCards(PollingVenue pollingVenue, string electionName, string electionDate){
			Contract.Requires(pollingVenue != null);
			Contract.Ensures(File.Exists(this.path + "\\PollingCards.pdf"));

			var pollingCards = new PollingCards(electionName, electionDate, "09.00 - 20.00");

			foreach (var person in pollingVenue.Persons){
				pollingCards.CreatePollingCard(person, pollingVenue.MunicipalityAddress, pollingVenue.PollingVenueAddress);
			}

			pollingCards.SaveToDisk(this.path+"\\PollingCards.pdf");
		}

		/// <summary>
		/// Can you save these voters to a csv file seperated with a ;?
		/// </summary>
		/// <param name="pollingVenue">The polling venue</param>
		public void SaveVoters(PollingVenue pollingVenue){
			Contract.Requires(pollingVenue != null);
			Contract.Ensures(File.Exists(this.path + "\\Voters.csv"));

			var sw = new StreamWriter(this.path+"\\Voters.csv", false);
			sw.WriteLine("FirstName;LastName;Cpr;VoterId;PollingTable;PollingVenueName"); //Header in the csv file
			
			foreach (var person in pollingVenue.Persons){
				sw.WriteLine(person.FirstName+";"+person.LastName+";"+person.Cpr+";"+person.VoterId+";"+person.PollingTable+";"+pollingVenue.PollingVenueAddress.Name);
			}

			sw.Close();
		}
	}
}