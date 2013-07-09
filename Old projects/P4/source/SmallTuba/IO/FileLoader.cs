﻿namespace SmallTuba.IO{
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Xml;
	using System.Xml.Linq;
	using System.Xml.Schema;
	using System.Diagnostics.Contracts;
	using SmallTuba.Entities;
	using SmallTuba.Utility;

	/// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// This class loads a xml file from at path and validate it against a xml schema.
	/// If the format of the file is correct, the class parse it to a list of polling venues
	/// with information about the voters and municipality associated with it.
	/// </summary>
	public class FileLoader{

		/// <summary>
		/// Can you load this xml file, and parse it to a list of polling venues?
		/// </summary>
		/// <param name="path">The path of the file</param>
		/// <param name="notifier">Event subscriber if validation of the xml file fails</param>
		/// <returns>A list of polling venues</returns>
		public List<PollingVenue> GetPollingVenues(string path, ValidationEventHandler notifier){
			Contract.Requires(path != null);
			Contract.Requires(notifier != null);
			Contract.Requires(File.Exists(path));
			Contract.Requires(Path.GetExtension(path).Equals(".xml"));

			return this.LoadVenues(XDocument.Load(path), notifier);		
		}

		/// <summary>
		/// Parses the xml file into a list of polling venues 
		/// </summary>
		/// <param name="xDocument">The xml file</param>
		/// <param name="notifier">Event subscriber if validation of the xml file fails</param>
		/// <returns>A list of polling venues</returns>
		private List<PollingVenue> LoadVenues(XDocument xDocument, ValidationEventHandler notifier){
			if (!this.ValidateXmlFile(xDocument, notifier)){
				var pollingVenuesElements = from n in xDocument.Descendants("PollingVenue") select n;

				List<PollingVenue> pollingVenues = new List<PollingVenue>();
				foreach (var xElement in pollingVenuesElements){
					
					Address pollingVenueAddress = new Address{
						Name = xElement.Element("Name").Value,
						Street = xElement.Element("Street").Value,
						City = xElement.Element("City").Value
					};

					Address municipalityAddress = new Address{
						Name = xElement.Parent.Parent.Element("Name").Value,
						Street = xElement.Parent.Parent.Element("Street").Value,
						City = xElement.Parent.Parent.Element("City").Value
					};

					PollingVenue pollingVenue = new PollingVenue{
						Persons = this.LoadPersons(xElement),
						PollingVenueAddress = pollingVenueAddress,
						MunicipalityAddress = municipalityAddress
					};

					pollingVenues.Add(pollingVenue);
				}
				return pollingVenues;
			}

			return null;
		}

		/// <summary>
		/// Parses the xml file into a list of persons objects
		/// </summary>
		/// <param name="xelement">the xml file</param>
		/// <returns>List of persons</returns>
		private List<Person> LoadPersons(XElement xelement){
			List<Person> persons = new List<Person>();
			var personElements = from n in xelement.Descendants("Voter") select n;

			foreach (var element in personElements){
				Person person = new Person {
					FirstName = element.Element("FirstName").Value,
					LastName = element.Element("LastName").Value,
					Street = element.Element("Street").Value,
					City = element.Element("City").Value,
					Cpr = element.Element("CprNo").Value,
					PollingTable = element.Element("PollingTable").Value,
					VoterId = VoterIdGenerator.CreateVoterId() //Generates a unique voter id
				};

				persons.Add(person);
			}
			return persons;
		}

		/// <summary>
		/// Validate the loaded xml file against the xml schema
		/// </summary>
		/// <param name="xDocument">The xml file</param>
		/// <param name="notifier">Subscriber to the event, if the validation fails</param>
		/// <returns>The result of the validation</returns>
		private bool ValidateXmlFile(XDocument xDocument, ValidationEventHandler notifier){
			var schemas = new XmlSchemaSet();
			schemas.Add("", XmlReader.Create(@"IO\schema.xml"));
			bool error = false;
			xDocument.Validate(schemas, (o,e) => {
					notifier(o, e);//Notifies the subscriber
					error = true;
				});
			return error;
		}	
	}
}
