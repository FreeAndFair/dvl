using SmallTuba.Entities;
using SmallTuba.Network.RPC;
using SmallTuba.Utility;

namespace SmallTuba {
	using System;

	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args) {
			var dg = new DataGenerator.DataGenerator();
			dg.NumberOfMunicipalities = 20;
			dg.NumberOfPollingVenues = 20;
			dg.NumberOfVoters = 2000;

			dg.FileDestination = "XmlTest.xml";

			dg.Generate();


			
			int server = int.Parse(args[0]);
			if (server == 0)
			{
				Console.Out.WriteLine(System.Net.Dns.GetHostName() + " = name");
				VoterServer voterServer = new VoterServer(System.Net.Dns.GetHostName());
				DateTime time = DateTime.Now;
				int unix = (int)TimeConverter.ConvertToUnixTimestamp(time.ToUniversalTime());
				voterServer.CprToPersonRequest = ((name,cpr) => new Person(){Cpr = cpr.ToString(), FirstName = "Ole", DbId = 42, LastName = "Henriksen", VotedPollingTable = "2", VotedTime = unix, Voted = false, Exists = true});
				voterServer.VoterIdToPersonRequest = ((name,id) => new Person() { Cpr = "42", FirstName = "Kim", DbId = id, LastName = "Larsen", VotedPollingTable = "3", VotedTime = unix, Voted = true, Exists = false});
				voterServer.RegisterVoteRequest = ((name,person) => !person.Voted);
				voterServer.UnregisterVoteRequest = ((name,person) => !person.Voted);
				voterServer.ValidTableRequest = ((name) => new string[]{"Table 1", "Table 2", "Table 3"});
				voterServer.ListenForCalls(0);
			}
		}
	}
}
