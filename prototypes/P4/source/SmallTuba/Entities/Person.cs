namespace SmallTuba.Entities {
	using System;
	using System.Collections.Generic;

	using SmallTuba.Utility;

	/// <author>Christian Olsson (chro@itu.dk)</author>
	/// <author>Henrik Haugbølle (hhau@itu.dk)</author>
	/// <version>2011-12-12</version>
	/// <summary>
	/// A person representation. Partly reflects properties of the
	/// PersonEntity class, but also implements some comparison and
	/// sorting methods for convinient use.
	/// </summary>
	[Serializable]
	public class Person {
		public int DbId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Street { get; set; }
		public string City { get; set; }
		public string Cpr { get; set; }
		public int VoterId { get; set; }
		public string PollingVenue { get; set; }
		public string PollingTable { get; set; }
		public bool Voted { get; set; }
		public int VotedTime { get; set; }
		public string VotedPollingTable { get; set; }
		public bool Exists { get; set; }

		/// <summary>
		/// A comparator to be used when sorting 
		/// persons by their full names.
		/// </summary>
		/// <returns>The comparator used in the sort.</returns>
		public static IComparer<Person> NameSort() {
			return new SortPersonsName();
		}

		/// <summary>
		/// Equals method comparing two person objects.
		/// </summary>
		/// <param name="other">The person to compare this instance to.</param>
		/// <returns>True if the instances are equal, false otherwise.</returns>
	    public bool Equals(Person other) {
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return other.DbId == DbId && Equals(other.FirstName, FirstName) && Equals(other.LastName, LastName) && Equals(other.Street, Street) && Equals(other.City, City) && other.Cpr == Cpr && other.VoterId == VoterId && Equals(other.PollingVenue, PollingVenue) && Equals(other.PollingTable, PollingTable) && other.Voted.Equals(Voted) && other.VotedTime == VotedTime && Equals(other.VotedPollingTable, VotedPollingTable) && other.Exists.Equals(Exists);
		}

		/// <summary>
		/// Comparison of two objects. The overridden equals
		/// method inherited from Object.
		/// </summary>
		/// <param name="obj">the object to compare this instance to.</param>
		/// <returns>True if the instances are equal, false otherwise.</returns>
		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof(Person)) return false;

			return Equals((Person)obj);
		}

		/// <summary>
		/// Calculate the hash code of the instance.
		/// </summary>
		/// <returns>The hash code of the instance.</returns>
		public override int GetHashCode() {
			unchecked {
				var result = DbId;
				result = (result * 397) ^ (FirstName != null ? FirstName.GetHashCode() : 0);
				result = (result * 397) ^ (LastName != null ? LastName.GetHashCode() : 0);
				result = (result * 397) ^ (Street != null ? Street.GetHashCode() : 0);
				result = (result * 397) ^ (City != null ? City.GetHashCode() : 0);
				result = (result * 397) ^ (Cpr != null ? Cpr.GetHashCode() : 0);
				result = (result * 397) ^ VoterId;
				result = (result * 397) ^ (PollingVenue != null ? PollingVenue.GetHashCode() : 0);
				result = (result * 397) ^ (PollingTable != null ? PollingTable.GetHashCode() : 0);
				result = (result * 397) ^ Voted.GetHashCode();
				result = (result * 397) ^ VotedTime;
				result = (result * 397) ^ (VotedPollingTable != null ? VotedPollingTable.GetHashCode() : 0);
				result = (result * 397) ^ Exists.GetHashCode();

				return result;

			}
		}

		/// <summary>
		/// Represent instance as a string.
		/// </summary>
		/// <returns>The current person instance represented as a string.</returns>
		public override string ToString() {
			return DbId + "," + Cpr + "," + FirstName + "," + LastName + ", " + PollingTable + ", " + TimeConverter.ConvertFromUnixTimestamp(VotedTime) + ", " + Voted;
		}

		/// <summary>
		/// Implementation of name comparator.
		/// Compares the full name of a person (both
		/// the first name and the last name).
		/// </summary>
	    private class SortPersonsName : IComparer<Person> {
			/// <summary>
			/// Compares two person objects.
			/// </summary>
			/// <param name="a">The first person.</param>
			/// <param name="b">The second person.</param>
			/// <returns>Whether the one person is greater than, equals or lesser than the other.</returns>
	        public int Compare(Person a, Person b) {
	            var name1 = a.FirstName + " " + a.LastName;
	            var name2 = b.FirstName + " " + b.LastName;

	            return name1.CompareTo(name2);
	        }
	    }
	}
}
