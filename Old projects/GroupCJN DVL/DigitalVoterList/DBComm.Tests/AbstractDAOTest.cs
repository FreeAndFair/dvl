// -----------------------------------------------------------------------
// <copyright file="AbstractDAOTest.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.ManualTests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using DBComm;
    using DBComm.DAO;
    using DBComm.DataGeneration;
    using DBComm.DO;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Summary description for AbstractDAOTest
    /// </summary>
    [TestClass]
    public class AbstractDAOTest
    {
        [TestInitialize]
        public void Setup()
        {
            var connection = new MySqlConnection(
                    "server=localhost;" + "port=3306;" + "uid=groupCJN;" + "password=abc123;" + "Sql Server Mode=true;"
                    + "database=groupcjn;");

            var creator = new DBCreator(connection);

            var generator = new Generator(DigitalVoterList.GetDefaultInstance());

            generator.Generate(10, 100, 5000);
        }

        [TestMethod]
        public void ReadOperation1()
        {
            // Arrange
            var dao = new VoterDAO();

            // Act
            Expression<Func<VoterDO, bool>> f = v => v.Name.StartsWith("A");
            Func<VoterDO, bool> func = f.Compile();
            var result = dao.Read(f);
            Debug.Assert(result.Count() > 0, "No voters matched the arranged query, please generate some new data!");
            // The above is very unlikely to fail since we are generating 5000 voters, and the generator only has 50 names to choose from, but we still have to check it.

            // Assert
            foreach (VoterDO voter in result)
            {
                Debug.Assert(func.Invoke(voter), "The predicate did not hold for some voter.");
            }
        }

        [TestMethod]
        public void ReadOperation2()
        {
            // Arrange
            var dao = new VoterDAO();
            dao.Update(v => v.Name.StartsWith("K"), new VoterDO(null, null, null, null, null, null, true));
            // Change some voters to have status voted = true;

            // Act
            Expression<Func<VoterDO, bool>> f = v => v.Voted == true;
            Func<VoterDO, bool> func = f.Compile();
            var result = dao.Read(f);
            Debug.Assert(result.Count() > 0, "No voters matched the arranged query, please generate some new data!");
            // The above is very unlikely to fail since we are generating 5000 voters, and the generator only has 50 names to choose from, but we still have to check it.

            // Assert
            foreach (VoterDO voter in result)
            {
                Debug.Assert(func.Invoke(voter), "The predicate did not hold for some voter.");
            }
        }

        [TestMethod]
        public void ReadOperation3()
        {
            // Arrange
            var pdao = new PollingStationDAO();
            PollingStationDO pollingStation = pdao.Read(p => true).First();
            // Pick a random polling station.

            var dao = new VoterDAO();

            // Act
            Expression<Func<VoterDO, bool>> f = v => v.PollingStationId == pollingStation.PrimaryKey;
            Func<VoterDO, bool> func = f.Compile();
            var result = dao.Read(f);
            Debug.Assert(result.Count() > 0, "No voters matched the arranged query, please generate some new data!");
            // The above is very unlikely to fail since we are generating 5000 voters, and the generator only has 50 names to choose from, but we still have to check it.

            // Assert
            foreach (VoterDO voter in result)
            {
                Debug.Assert(func.Invoke(voter));
            }
        }

        [TestMethod]
        public void DeleteOperation1()
        {
            // Arrange
            var dao = new VoterDAO();

            // Assert (contained in contracts)
            dao.Delete(v => v.Name.StartsWith("K"));
        }

        [TestMethod]
        public void DeleteOperation2()
        {
            // Arrange
            var dao = new VoterDAO();
            dao.Update(v => v.Name.StartsWith("B"), new VoterDO(null, null, null, null, null, null, true));
            // Change some voters to have status voted = true;

            // Assert (contained in contracts)
            dao.Delete(v => v.Voted == true);
        }

        [TestMethod]
        public void CreateOperation1()
        {
            // Arrange
            var dao = new VoterDAO();

            uint cpr = 0;
            while (true)
            {
                // Continue to generate CPRs until nothing is returned from the db, i.e. the key is not contained so we can safely create it.
                Data data = new Data();
                cpr = data.GetCPR();

                if (dao.Read(v => v.PrimaryKey == cpr).Count() == 0)
                {
                    break;
                }
            }

            // Assert (contained in contracts)
            dao.Create(new VoterDO(1, cpr, "Tester", "TestRoad", "TestCity", false, false));
        }
    }
}
