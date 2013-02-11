// -----------------------------------------------------------------------
// <copyright file="PessimisticVoterDAO.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DAO
{
    using System.Data;
    using System.Diagnostics.Contracts;
    using global::DBComm.DBComm.DO;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// <para>A limited DAO supporting pessimisic transaction control.</para>
    /// <para>This DAO only supports a limit set of operations, and is
    /// meant for use solely during the election.</para>
    /// </summary>
    public class PessimisticVoterDAO
    {

        private MySqlTransaction transaction;

        private readonly MySqlConnection connection;
        private int timeout;

        public PessimisticVoterDAO(string server, string password)
        {
            this.connection = DigitalVoterList.GetConnectionInstance("groupCJN", password, server);
        }

        /// <summary>
        /// Open the connection and begin a new transaction.
        /// </summary>
        /// <param name="commTimeout">
        /// The timeout for each command in the transaction. Default is 3 seconds.
        /// </param>
        public void StartTransaction(int commTimeout = 3)
        {
            if (!this.TransactionStarted())
            {
                this.timeout = commTimeout;
                this.connection.Open();
                this.transaction = this.connection.BeginTransaction(IsolationLevel.Serializable);
            }
        }

        /// <summary>
        /// Stop the current transaction and close the connection.
        /// </summary>
        public void EndTransaction()
        {
            Contract.Requires(this.TransactionStarted());
            Contract.Ensures(!this.TransactionStarted());

            transaction.Commit();
            transaction = null;

            connection.Close();
        }

        /// <summary>
        /// Is there an open transaction?
        /// </summary>
        /// <returns>True if there is an open transaction, false otherwise.</returns>
        public bool TransactionStarted()
        {
            return transaction != null;
        }

        /// <summary>
        /// Read voter by id.
        /// </summary>
        /// <param name="id">The id of the voter.</param>
        /// <returns>The voter corresponding to the id, null if no voter is found.</returns>
        [Pure]
        public VoterDO Read(uint id)
        {
            Contract.Requires(this.TransactionStarted());
            Contract.Ensures(Contract.Result<VoterDO>() != null ? Contract.Result<VoterDO>().PrimaryKey == id : true);

            // Command building
            MySqlCommand command = this.connection.CreateCommand();

            command.Connection = this.connection;
            command.Transaction = this.transaction;
            command.CommandTimeout = this.timeout;

            // Query building
            command.CommandText = "call voter_read( @id_param );";

            MySqlParameter p = new MySqlParameter("@id_param", MySqlDbType.UInt32) { Value = id };
            command.Parameters.Add(p);

            // Query execution
            MySqlDataReader reader = command.ExecuteReader();

            VoterDO voter = null;
            while (reader.Read())
            {
                uint pollingStationId = uint.Parse(reader["pollingStationId"].ToString());
                uint cpr = uint.Parse(reader["cpr"].ToString());
                string name = reader["name"].ToString();
                string address = reader["address"].ToString();
                string city = reader["city"].ToString();
                bool crdPrinted = bool.Parse(reader["cardPrinted"].ToString());
                bool voted = bool.Parse(reader["voted"].ToString());

                voter = new VoterDO(pollingStationId, cpr, name, address, city, crdPrinted, voted);
            }

            reader.Close();

            return voter;
        }

        /// <summary>
        /// Update a voter with a given id to the values in the dummy.
        /// </summary>
        /// <param name="id">
        /// The id of the voter to be updated.
        /// </param>
        /// <param name="votedStatus">
        /// The voted Status.
        /// </param>
        /// <returns>
        /// </returns>
        public bool Update(uint id, bool votedStatus)
        {
            Contract.Requires(this.TransactionStarted());
            Contract.Ensures(this.Read(id) == null || this.Read(id).Voted == votedStatus);

            // Command building
            MySqlCommand command = this.connection.CreateCommand();

            command.Connection = this.connection;
            command.Transaction = this.transaction;
            command.CommandTimeout = this.timeout;

            // Query building
            command.CommandText = "call voter_update( @voted_param , @id_param );";

            MySqlParameter votedParam = new MySqlParameter("@voted_param", SqlDbType.TinyInt) { Value = votedStatus };
            command.Parameters.Add(votedParam);

            MySqlParameter idParam = new MySqlParameter("@id_param", MySqlDbType.UInt32) { Value = id };
            command.Parameters.Add(idParam);

            // Query execution
            command.ExecuteNonQuery();

            return true;
        }
    }
}