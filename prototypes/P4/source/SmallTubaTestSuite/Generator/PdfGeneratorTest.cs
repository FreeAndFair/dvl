namespace SmallTubaTestSuite.Generator
{
    using System.IO;
    using NUnit.Framework;
    using SmallTuba.Entities;
    using SmallTuba.PdfGenerator;

    /// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// Test class for the two pdf generator classes
    /// </summary>
    [TestFixture]
    public class PdfGeneratorTest{
        
        /// <summary>
        /// Test that the generated polling card is located at the harddrive
        /// </summary>
        [Test]
        public void TestPollingCards()
        {
            PollingCards pollingCards = new PollingCards("test election", "01-01-01-2011", "09.00 - 20.00");
            Person person = new Person{
                    FirstName = "Hans",
                    LastName = "Sørensen",
                    Street = "Hovedgaden 10",
                    City = "2300 København S",
                    PollingTable = "1",
                };

            Address pollingVenue = new Address { Name = "Byskolen", Street = "Lærervej 8", City = "2300 København S" };
            Address sender = new Address { Name = "Rådhuset", Street = "Ministervej 8", City = "2100 København Ø" };
            pollingCards.CreatePollingCard(person, sender, pollingVenue);
            
            Assert.False(File.Exists("testpollingcard.pdf"));
            pollingCards.SaveToDisk("testpollingcard.pdf");
            Assert.True(File.Exists("testpollingcard.pdf"));
            File.Delete("testpollingcard.pdf");
        }

        /// <summary>
        /// Test that the generated voter list is located at the harddrive
        /// </summary>
        [Test]
        public void TestVoterList(){
            VoterList voterList = new VoterList(50, "test election", "01-01-01-2011", "1");
            Person person = new Person{
                FirstName = "Hans",
                LastName = "Sørensen",
                Street = "Hovedgaden 10",
                City = "2300 København S",
                PollingTable = "1",
                Cpr = "0101010101"
            };
            
            for(int i = 0; i<60; i++){
                voterList.AddVoter(person);
            }

            Assert.False(File.Exists("testvoterlist.pdf"));
            voterList.SaveToDisk("testvoterlist.pdf");
            Assert.True(File.Exists("testvoterlist.pdf"));
            File.Delete("testvoterlist.pdf");
        }

    }
}
