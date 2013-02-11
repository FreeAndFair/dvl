namespace ServerApplication {
	using System;
	using System.IO;

	using SmallTuba.Database;
	using SmallTuba.Entities;
	using SmallTuba.Utility;

	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// The ServerData class assists the server in importing
	/// and clearing the data handled by the database.
	/// 
	/// It only consists of two method; one for importing
	/// a given csv-file to the database and one for clearing
	/// the tables in the database.
	/// </summary>
	public class ServerData {
		/// <summary>
		/// Imports a csv file which path is given by the 
		/// commandline arguments.
		/// </summary>
		/// <param name="args">The commandline arguments split by ' '.</param>
		/// <returns>Whether the data was imported.</returns>
		public bool Import(string[] args) {
			var file = String.Join(" ", args);
			file = file.Substring(7);
			file = file.Trim();

			if (args.Length > 1 && file != "") {
				try {
					using (var reader = new StreamReader(file)) {
						Console.WriteLine(@"Importing from: "+file);

						string line;
						var i = 0;

						while ((line = reader.ReadLine()) != null) {
							if (i > 0) {
								var row = line.Split(';');

								var personEntity = new PersonEntity {
									Firstname = row[0],
									Lastname = row[1],
									Cpr = row[2],
									VoterId = Convert.ToInt32(row[3]),
									PollingTable = row[4],
									PollingVenue = row[5]
								};

								Console.WriteLine(@"Imported: "+personEntity.VoterId);

								personEntity.Save();
							}
							
							i++;
						}
					}

					return true;
				} catch (Exception e) { 
					Debug.WriteLine(e.ToString());
				}
			}
			
			return false;
		}

		/// <summary>
		/// Truncates a table in the database.
		/// </summary>
		/// <param name="args">The commandline arguments</param>
		/// <returns>Whether clearing of the table in the database went well.</returns>
		public bool Clear(string[] args) {
			if (args.Length > 1 && args[1] != null && args[1] != "" && (args[1] == "person" || args[1] == "log")) {
				var queryBuilder = new QueryBuilder();
				queryBuilder.SetType("truncate");
				queryBuilder.SetTable(args[1]);

				queryBuilder.ExecuteNoneQuery();

				return true;
			}

			return false;
		}
	}
}
