// -----------------------------------------------------------------------
// <copyright file="PVDAOTest.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------


using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DBComm.ManualTests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    using DBComm;
    using DBComm.DAO;
    using DBComm.DataGeneration;
    using DBComm.DO;
    using MySql.Data.MySqlClient;

    /// <summary>
    /// Test class for pessimistic voter DAO. Scenarios are described in the document "DVL_scenario_event_DAO.bon".
    /// </summary>
    [TestClass]
    public class PVDAOTest
    {
        private string server = "localhost";

        private string password = "abc123";

        private IEnumerable<VoterDO> voters;

        private IEnumerable<PessimisticVoterDAO> daos;

        [TestInitialize]
        public void Setup()
        {
            this.daos = new List<PessimisticVoterDAO>();

            var connection = new MySqlConnection(
                    "server=localhost;" + "port=3306;" + "uid=groupCJN;" + "password=abc123;" + "Sql Server Mode=true;"
                    + "database=groupcjn;");

            var creator = new DBCreator(connection);

            var generator = new Generator(DigitalVoterList.GetDefaultInstance());

            generator.Generate(1, 1, 3);

            VoterDAO voterDAO = new VoterDAO();
            this.voters = voterDAO.Read(v => true);
        }

        [TestCleanup]
        public void TearDown()
        {
            // Make sure that all transactions are closed properly, even for tests that throw exceptions.
            foreach (PessimisticVoterDAO dao in this.daos)
            {
                if (dao.TransactionStarted())
                {
                    dao.EndTransaction();
                }
            }
        }

        [TestMethod]
        public void NegativeConcurrencyTest()
        {
            /*
             * Testing method explained: 
             * My first thought was to simply expect this method to throw any kind
             * of MySqlException. However, such an exception is very broad, and can cover a
             * lot of failures, also some that we DON'T expect, and ones that do not mean a
             * passed test. E.g. the test might throw an excetion because the database
             * was not reachable, and then we cannot conclude anything about the concurrency that
             * we are testing. Therefore i chose to catch the exception myself, and check that the
             * error messgae contains something about timeout, which would mean the transaction was 
             * blocked by another one. If the read operation does not thrown an exception, as it is 
             * expected to do, the test fails as the program reaches assert false.
             */

            PessimisticVoterDAO dao1 = new PessimisticVoterDAO(server, password);
            PessimisticVoterDAO dao2 = new PessimisticVoterDAO(server, password);
            this.daos = this.daos.Concat(new List<PessimisticVoterDAO> { dao1, dao2 });

            uint voterID = (uint)this.voters.First().PrimaryKey;

            dao1.StartTransaction();
            dao1.Read(voterID);

            try
            {
                dao2.StartTransaction();
                dao2.Read(voterID);
                Debug.Assert(false);
            }
            catch (MySqlException e)
            {
                Debug.Assert(e.Message.Contains("timeout"));
            }
        }

        [TestMethod]
        public void PositiveConcurrencyTest()
        {
            PessimisticVoterDAO dao1 = new PessimisticVoterDAO(server, password);
            PessimisticVoterDAO dao2 = new PessimisticVoterDAO(server, password);
            this.daos = this.daos.Concat(new List<PessimisticVoterDAO> { dao1, dao2 });

            uint voterID = (uint)this.voters.First().PrimaryKey;

            dao1.StartTransaction();
            dao1.Read(voterID);
            dao1.EndTransaction();

            dao2.StartTransaction();
            dao2.Read(voterID);
            dao2.EndTransaction();
        }

        [TestMethod]
        public void PositiveConcurrencyTest2()
        {
            PessimisticVoterDAO dao1 = new PessimisticVoterDAO(server, password);
            PessimisticVoterDAO dao2 = new PessimisticVoterDAO(server, password);
            this.daos = this.daos.Concat(new List<PessimisticVoterDAO> { dao1, dao2 });

            uint voterID = (uint)this.voters.First().PrimaryKey;

            dao1.StartTransaction();
            dao1.Read(voterID);
            dao1.Update(voterID, true);
            dao1.EndTransaction();

            dao2.StartTransaction();
            dao2.Read(voterID);
            dao2.EndTransaction();
        }

        [TestMethod]
        public void PossitiveConcurrencyTest3()
        {
            PessimisticVoterDAO dao1 = new PessimisticVoterDAO(server, password);
            PessimisticVoterDAO dao2 = new PessimisticVoterDAO(server, password);
            this.daos = this.daos.Concat(new List<PessimisticVoterDAO> { dao1, dao2 });

            uint voter1ID = (uint)this.voters.First().PrimaryKey;
            uint voter2ID = (uint)this.voters.Last().PrimaryKey;
            Debug.Assert(voter1ID != voter2ID);

            dao1.StartTransaction();
            dao1.Read(voter1ID);

            dao2.StartTransaction();
            dao2.Read(voter2ID);
            dao2.Update(voter2ID, true);

            dao1.Update(voter1ID, true);

            dao1.EndTransaction();
            dao2.EndTransaction();
        }
    }
}