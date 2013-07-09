using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigitalVoterList.Test
{
    using System.Collections.Generic;
    using System.IO;

    using Central.Central.Controllers;
    using Central.Central.Models;
    using Central.Central.Views;

    using DBComm.DBComm.DAO;
    using DBComm.DBComm.DO;

    /// <summary>
    /// Inserts specific test data into the database and runs voter card 
    /// generations with various grouping properties.
    /// 
    /// NOTICE: Asserts based on output file names. Sadly human verification 
    /// is required if the contents of the pdf-files have to be verified 
    /// completely. 
    /// 
    /// The summery of each test states what the output should look like.
    /// </summary>
    [TestClass]
    public class VoterCardGeneratorTest
    {
        // Destination for the test output.
        private const string destination = "C:\\VoterCards\\Tests";

        private VoterFilter filter;
        private VoterCardGenerator vcg;
        private VoterCardGeneratorWindow vcgView;
        private VoterCardGeneratorController vcgController;

        private MunicipalityDO mun;
        private PollingStationDO[] ps = new PollingStationDO[3];
        private VoterDO[] v = new VoterDO[10];
            
        [TestInitialize]
        public void SetUp()
        {
            if (!Directory.Exists(destination)) Directory.CreateDirectory(destination);

            // Create a municipality
            var mDAO = new MunicipalityDAO();
            mDAO.Create(new MunicipalityDO(9998, "Teststreet 42", "4242 Testcity", "Municipality of Test"));
            IEnumerator<MunicipalityDO> muns = 
                mDAO.Read(m => m.Name.StartsWith("Municipality of Test")).GetEnumerator();
            muns.MoveNext();
            this.mun = muns.Current;

            // Create 3 polling stations.
            var pDAO = new PollingStationDAO();
            pDAO.Create(new PollingStationDO(this.mun.PrimaryKey, 10000, "Test Polling Station 1", "Teststreet 44"));
            pDAO.Create(new PollingStationDO(this.mun.PrimaryKey, 10001, "Test Polling Station 2", "Teststreet 45"));
            pDAO.Create(new PollingStationDO(this.mun.PrimaryKey, 10002, "Test Polling Station 3", "Teststreet 46"));
            IEnumerator<PollingStationDO> pss =
                pDAO.Read(po => po.Name.StartsWith("Test Polling Station")).GetEnumerator();
            int pi = 0;
            while(pss.MoveNext()) this.ps[pi++] = pss.Current;

            // Create 10 voters.
            var vDAO = new VoterDAO();
            vDAO.Create(new VoterDO(this.ps[0].PrimaryKey, 101264242, "Test Testson 1", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[1].PrimaryKey, 101264243, "Test Testson 2", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[2].PrimaryKey, 101264244, "Test Testson 3", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[0].PrimaryKey, 101264245, "Test Testson 4", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[0].PrimaryKey, 101264246, "Test Testson 5", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[1].PrimaryKey, 101264247, "Test Testson 6", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[1].PrimaryKey, 101264248, "Test Testson 7", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[2].PrimaryKey, 101264249, "Test Testson 8", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[2].PrimaryKey, 101264250, "Test Testson 9", "Teststreet 47", "4242 Testcity", false, false));
            vDAO.Create(new VoterDO(this.ps[2].PrimaryKey, 101264251, "Test Testson 10", "Teststreet 47", "4242 Testcity", false, false));
            IEnumerator<VoterDO> voters = vDAO.Read(vo => vo.Name.StartsWith("Test Testson")).GetEnumerator();
            int vi = 0;
            while (voters.MoveNext()) this.v[vi++] = voters.Current;

            // Setup Voter Card Generator sub-system.
            this.filter = new VoterFilter(mun);
            this.vcg = new VoterCardGenerator(filter);
            this.vcgView = new VoterCardGeneratorWindow(this.vcg);
            this.vcgController = new VoterCardGeneratorController(this.vcg, this.vcgView);
        }

        [TestCleanup]
        public void TearDown()
        {
            var vDAO = new VoterDAO();
            vDAO.Delete(vo => vo.Name.StartsWith("Test Testson"));
            var pDAO = new PollingStationDAO();
            pDAO.Delete(po => po.Name.StartsWith("Test Polling Station"));
            var mDAO = new MunicipalityDAO();
            mDAO.Delete(m => m.Name.StartsWith("Municipality of T"));
        }

        /// <summary>
        /// Pure generation.
        /// 
        /// It should output a single pdf-file called 'All.pdf' 
        /// which should contain all the 10 test voters.
        /// </summary>
        [TestMethod]
        public void PureGenerate()
        {
            string path = destination + "\\Pure";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.vcg.Generate(path, -1, -1);

            IEnumerator<string> files = Directory.EnumerateFiles(path).GetEnumerator();
            Assert.IsTrue(files.MoveNext());
            Assert.IsTrue(files.Current.Equals("All.pdf"));
            Assert.IsFalse(files.MoveNext());
        }

        /// <summary>
        /// Group by polling station generation.
        /// 
        /// It should output 3 pdf-files named after the 3 test polling stations.
        /// Each pdf-file should contain the voters associated with that polling station.
        /// </summary>
        [TestMethod]
        public void GroupGenerate()
        {
            string path = destination + "\\Group";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.vcg.Generate(path, 1, -1);

            IEnumerator<string> files = Directory.EnumerateFiles(path).GetEnumerator();
            for (int i = 0; i < 3; i++)
            {
                Assert.IsTrue(files.MoveNext());
                string file = files.Current;
                Assert.IsTrue(file.Equals("Test Polling Station" + i + ".pdf"));
            }
            Assert.IsFalse(files.MoveNext());
        }

        /// <summary>
        /// Pure generation with a batch limit of 2 voters/file.
        /// 
        /// It should output 5 pdf-files named All(n) where n is a number from 0 to 5.
        /// Each pdf-file should contain 2 voters.
        /// </summary>
        [TestMethod]
        public void LimitGenerate()
        {
            string path = destination + "\\Limit";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.vcg.Generate(destination, -1, 2);

            IEnumerator<string> files = Directory.EnumerateFiles(path).GetEnumerator();
            for (int i = 0; i < 5; i++)
            {
                Assert.IsTrue(files.MoveNext());
                string file = files.Current;
                Assert.IsTrue(file.Equals("All" + i + ".pdf"));
            }
            Assert.IsTrue(files.MoveNext());
        }

        /// <summary>
        /// Group by polling station generation with a batch limit of 2 voters/file.
        /// 
        /// It should output 2 files for each of the 3 polling stations.
        /// </summary>
        [TestMethod]
        public void LimitGroupGenerate()
        {
            string path = destination + "\\LimitGroup";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            this.vcg.Generate(destination, 1, 2);

            IEnumerator<string> files = Directory.EnumerateFiles(path).GetEnumerator();
            for (int i = 0; i < 3; i++ )
            {
                Assert.IsTrue(files.MoveNext());
                string file = files.Current;
                for (int g = 0; g < 2; g++)
                {
                    Assert.IsTrue(file.Equals("Test Polling Station" + i + "" + g + ".pdf"));
                    i++;
                }
                Assert.IsFalse(files.MoveNext());
            }
        }
    }
}
