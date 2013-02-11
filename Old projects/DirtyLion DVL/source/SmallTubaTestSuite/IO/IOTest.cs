namespace SmallTubaTestSuite.IO
{
    using System.Collections.Generic;
    using System.IO;
    using NUnit.Framework;
    using SmallTuba.Entities;
    using SmallTuba.IO;

    /// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// This test class tests the file loader and the file saver.
    /// The testdata.xml and schema.xml have to be located in the in the same
    /// folder as the .exe file before the tests can be run successfully
    /// </summary>
    [TestFixture]
    public class IOTest{
        private FileLoader fileLoader;
        private FileSaver fileSaver;
        private List<PollingVenue> pollingVenues;
        private string path;

        /// <summary>
        /// Load a test file in, and creates some folders for test purpose
        /// </summary>
        [SetUp]
        public void SetUp(){
            FileLoader fileLoader = new FileLoader();
            pollingVenues = fileLoader.GetPollingVenues(@"IO\TestInput.xml", ((o, e) => { }));
            path = Directory.CreateDirectory("Test").FullName;
            fileSaver = new FileSaver(path, "TestVenue");
        }
        
        /// <summary>
        /// Deletes the test files and folders
        /// </summary>
        [TearDown]
        public void TearDown(){
            foreach (var file in Directory.GetFiles(path+"\\TestVenue")){
                File.Delete(file);
            }
          
            Directory.Delete(path + "\\TestVenue");
            Directory.Delete(path);
        }
        
        /// <summary>
        /// Test that the fileloader parses the xml file correct into polling venues objects
        /// </summary>
        [Test]
        public void FileLoaderTest(){    
            int count = 0;
            foreach (var pollingVenue in pollingVenues){
                count += pollingVenue.Persons.Count;
            }
            Assert.True(pollingVenues.Count == 2); // Number of polling venues
            Assert.AreEqual(count, 200); //Number of totals voters
        }

        /// <summary>
        /// Tests that the polling card save method generates a file on the harddrive
        /// </summary>
        [Test]
        public void FileSaverPollingCardTest(){
            fileSaver.SavePollingCards(pollingVenues[0], "Test election", "01-01-01-2011");
            Assert.True(File.Exists(path + "\\TestVenue\\PollingCards.pdf"));
        }

        /// <summary>
        /// Tests that the voter lists save method generates the right number of voterlists
        /// and they are saved to the harddrive
        /// </summary>
        [Test]
        public void FileSaverVoterListsTest(){
            fileSaver.SaveVoterList(pollingVenues[0].Persons, "Test election", "010101-2011");
            Assert.AreEqual(Directory.GetFiles(path + "\\TestVenue", "VoterListTable*.pdf").Length, 19);
        }

        /// <summary>
        /// Tests that the voter data save method generates a file on the harddrive
        /// </summary>
        [Test]
        public void FileSaverVoterDataTest(){
            fileSaver.SaveVoters(pollingVenues[0]);
            Assert.True(File.Exists(path + "\\TestVenue\\voters.csv"));
        }

    }
}
