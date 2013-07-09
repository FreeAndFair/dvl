namespace SmallTubaTestSuite.Generator
{
    using SmallTuba.Utility;
    using NUnit.Framework;

    /// <author>Kåre Sylow Pedersen (ksyl@itu.dk)</author>
    /// <version>2011-12-12</version>
    /// <summary>
    /// Test class for the voter id generator class
    /// </summary>
    [TestFixture]
    public class VoterIdTest{

        /// <summary>
        /// Tests that the lastes generated id is the higest
        /// </summary>
        [Test]
        public void TestVoterID(){
            int id1 = VoterIdGenerator.CreateVoterId();
            int id2 = VoterIdGenerator.CreateVoterId();
            Assert.Greater(id2,id1);
        }
    }
}
