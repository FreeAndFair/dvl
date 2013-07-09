using System;
using System.Data.SQLite;
using System.Data;

namespace DVLTerminal.Utilities
{
    /// <summary>
    /// An API for working with the SQLite database.
    /// </summary>
    /// Author: Michael Oliver Urhøj Mortensen & Rasmus Greve
    public class Database
    {
        private static Database _sqlite;
        private readonly SQLiteConnection _connection;
        private readonly SQLiteCommand _sqlComm;
        private readonly string _dbConnection;
        private SQLiteDataReader _reader;

        public static Database GetInstance
        {
            get { return _sqlite ?? (_sqlite = new Database()); }
        }
        
        /// <summary>
        /// Initialises a Databse object with standard information about the databse file
        /// name. Default is "valid_voters.db".
        /// </summary>
        /// Author: Michael Oliver Urhøj Mortensen
        private Database()
        {
            _dbConnection = "Data Source=valid_voters.db";
            _connection = new SQLiteConnection(_dbConnection);
            _sqlComm = new SQLiteCommand(_connection);
            _connection.Open();
        }

        /// <summary>
        /// Destructer closes the open database connection.
        /// </summary>
        ~Database()
        {
            //_reader.Close();
            _connection.Close();
        }

        /// <summary>
        /// Executes a non-query to the database provided in the constructor.
        /// </summary>
        /// <param name="query">The non-query to be executed.</param>
        /// Author: Michael Oliver Urhøj Mortensen
        public void ExecuteNonQuery(string query)
        {
            try
            {
                new SQLiteCommand(_connection) { CommandText = query }.ExecuteNonQuery();
            }
            catch (Exception)
            {
                // Should an exception get thrown and catched, recall the method.
                // The SQLite-query will most likely work 2nd or 3rd time, if 1st fails,
                // because the error appears on random occasions, and not in a predictable manner.
                ExecuteNonQuery(query);
            }
        }

        /// <summary>
        /// Executes a query to the database provided in the constructor.
        /// Attention: There are no checks of whether or not the SQL command
        /// entered will harm the database. Use with caution.
        /// </summary>
        /// <param name="query">The query to be executed.</param>
        /// Author: Michael Oliver Urhøj Mortensen
        public DataTable ExecuteQuery(string query)
        {
            try
            {
                var dt = new DataTable();
                _sqlComm.CommandText = query;
                _reader = _sqlComm.ExecuteReader();
                dt.Load(_reader);
                _reader.Close();
                return dt;
            }
            catch (Exception)
            {
                // Should an exception get thrown and catched, recall the method.
                // The SQLite-query will most likely work 2nd or 3rd time, if 1st fails,
                // because the error appears on random occasions, and not in a predictable manner.
                return ExecuteQuery(query);
            }
        }
       
        /// <summary>
        /// Validate that a given voternumber is in the database
        /// </summary>
        /// <param name="voterNumber">The voternumber to validate</param>
        /// <returns>True if it's a valid voternumer</returns>
        public bool ValidVoterNumber(ulong voterNumber)
        {
            return ExecuteQuery("SELECT * FROM Eligible_Voter WHERE Voting_Num = " + voterNumber + " LIMIT 1;").Rows.Count == 1;
        }

        /// <summary>
        /// Validate that the give voternumber and cpr check number matches
        /// </summary>
        /// <param name="voterNumber">The voternumber to use</param>
        /// <param name="cprCheckNumber">The cpr check number to test</param>
        /// <returns>True if it's the right cpr check number</returns>
        public bool ValidCprCheckNumber(ulong voterNumber, ushort cprCheckNumber)
        {
            var securityNum = Convert.ToUInt64(ExecuteQuery("SELECT cpr FROM Eligible_Voter WHERE Voting_Num = " + voterNumber + ";").Rows[0][0]);
            return (securityNum % 10000 == cprCheckNumber);
        }

        /// <summary>
        /// Has a given person voted
        /// </summary>
        /// <param name="voterNumber">The voternumber of the person to test</param>
        /// <returns>True if the person has voted</returns>
        public bool HasVoted(ulong voterNumber)
        {
            return Convert.ToBoolean(ExecuteQuery("SELECT Has_Voted FROM Eligible_Voter WHERE Voting_Num = " + voterNumber + ";").Rows[0][0]);
        }

        /// <summary>
        /// Set whether a given person has voted
        /// </summary>
        /// <param name="voterNumber">The voternumber of the person to update</param>
        /// <param name="voted">Whether or not the person has voted</param>
        public void SetVoted(ulong voterNumber, bool voted)
        {
            ExecuteNonQuery("UPDATE Eligible_Voter SET Has_Voted = " + (voted ? 1 : 0) + " WHERE Voting_Num = " + voterNumber + ";");
        }

        /// <summary>
        /// Get the current election progress
        /// </summary>
        /// <returns></returns>
        public uint GetHasVotedCount()
        {
          return Convert.ToUInt32(ExecuteQuery("SELECT count(*) FROM Eligible_Voter WHERE Has_Voted = 1;").Rows[0][0]);
        }

        /// <summary>
        /// Get the total number of voters at this location
        /// </summary>
        /// <returns></returns>
        public uint GetTotalVotersCount()
        {
            return Convert.ToUInt32(ExecuteQuery("SELECT count(*) FROM Eligible_Voter;").Rows[0][0]);
        }

    }
}