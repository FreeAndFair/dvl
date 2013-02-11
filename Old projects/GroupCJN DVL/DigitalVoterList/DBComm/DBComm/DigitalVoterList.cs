// -----------------------------------------------------------------------
// <copyright file="DigitalVoterList.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm
{
    using System;
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.IO;
    using System.Linq;
    using global::DBComm.DBComm.DO;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// The data context representing the DVL database.
    /// </summary>
    [Database(Name = ("groupcjn"))]
    public class DigitalVoterList : DataContext
    {
        private const string Path = "c:/ServerSetupDVL.conf";

        private const string Default =
            "server=localhost;port=3306;uid=groupCJN;password=abc123;Sql Server Mode=true;database=groupcjn;";

        public Table<PollingStationDO> pollingStations;

        public Table<VoterDO> voters;

        public Table<MunicipalityDO> municipalities;

        public Table<LogDO> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalVoterList"/> class. 
        /// Initialize the datacontext via the base constructor.
        /// </summary>
        /// <param name="c">
        /// A working connection.
        /// </param>
        private DigitalVoterList(MySqlConnection c)
            : base(c)
        {
        }

        /// <summary>
        /// Gets ConnString. Tries to read from "c:/ServerSetupDVL.conf" or falls back to the default string
        /// </summary>
        private static string ConnString
        {
            get
            {
                try
                {
                    List<string> lines = File.ReadLines(Path).ToList();
                    string server = lines[0];
                    string port = lines[1];
                    string user = lines[2];
                    string password = lines[3];
                    return string.Format("server={0};uid={1};password={2};port={3};Sql Server Mode = true;database={4}", server, user, password, port, "groupcjn");
                }
                catch (Exception)
                {
                    return Default;
                }
            }
        }

        /// <summary>
        /// Create a new database with the connection parameters read from the config file.
        /// * server = localhost
        /// * port = 3306
        /// * uid = groupCJN
        /// * password = abc123
        /// * sql server mode = true
        /// * database = groupCJN
        /// </summary>
        /// <returns>A new datacontext instance.</returns>
        public static DigitalVoterList GetDefaultInstance()
        {
            return new DigitalVoterList(new MySqlConnection(ConnString));
        }

        /// <summary>
        /// Create a new, open database instance based on the connection parameters.
        /// </summary>
        /// <param name="user">The user. </param>
        /// <param name="password">The password. </param>
        /// /// <param name="dbName">The name of the database to connect to.</param>
        /// <param name="server">The server.</param>
        /// <param name="port">The port.</param>
        /// <returns>A new datacontext.</returns>
        public static DigitalVoterList GetInstance(string user, string password, string server, string port = "3306")
        {
            var conn = new MySqlConnection(
                string.Format(
                    "server={0};uid={1};password={2};port={3};Sql Server Mode = true;database={4}",
                    server,
                    user,
                    password,
                    port,
                    "groupcjn"
                    )
                    );
            conn.Open();

            return new DigitalVoterList(conn);
        }

        /// <summary>
        /// Create a new, open database instance based on the connection parameters.
        /// </summary>
        /// <param name="server">The server. </param>
        /// <returns>
        /// A new datacontext.
        /// </returns>
        public static DigitalVoterList GetInstanceFromServer(string server)
        {
            var conn = new MySqlConnection(
                string.Format(
                    "server={0};uid={1};password={2};port={3};Sql Server Mode = true;database={4}",
                    server,
                    "groupCJN",
                    "abc123",
                    "3306",
                    "groupcjn"));
            conn.Open();

            return new DigitalVoterList(conn);
        }

        /// <summary>
        /// Get a connection based on the given parameters.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="password">The password</param>
        /// <param name="server">The adress to the server. Assuming localhost if no adress is provided.</param>
        /// <param name="port">The port number to this connection. Assuming 3306 if no port is provided.</param>
        /// <returns>
        /// A new connection.
        /// </returns>
        public static MySqlConnection GetConnectionInstance(string user, string password, string server = "localhost", int port = 3306)
        {
            var conn = new MySqlConnection(
                string.Format(
                    "server={0};uid={1};password={2};port={3};Sql Server Mode = true;database={4}",
                    server,
                    user,
                    password,
                    port,
                    "groupcjn"));

            return conn;
        }
    }
}