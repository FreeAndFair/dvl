// ----------------------------------------------------------------------
// <copyright file="Program.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm
{
    using System;
    using global::DBComm.DBComm.DataGeneration;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Program
    {
        static void Main(string[] args)
        {
            // Create Database.
            var dbc = new DBCreator(new MySqlConnection(
                   "server=localhost;" + "port=3306;" + "password=abc123;" + "uid=groupCJN;"));
            // Initialize generator.
            var g = new Generator(DigitalVoterList.GetInstance("groupCJN", "abc123", "localhost", "3306"));

            // Generate!
            g.Generate(10, 100, 500);

            Console.WriteLine("done");
        }
    }
}
