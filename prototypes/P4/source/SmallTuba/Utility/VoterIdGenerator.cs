
namespace SmallTuba.Utility {
    using System;
    using System.Diagnostics.Contracts;

    /// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// Generates a unique voter id number
    /// </summary>
    public class VoterIdGenerator {
        private static int prevId;
        private static int id;

        /// <summary>
        /// Generates a voter id based on unix time
        /// </summary>
        /// <returns>voter id</returns>
        public static int CreateVoterId() {
            Contract.Ensures(Contract.OldValue(id)<Contract.Result<int>());

            id = TimeConverter.ConvertToUnixTimestamp(DateTime.Now);
            while (id <= prevId) {
                id++;
            }
            prevId = id;
            return id;
        }
    }
}
