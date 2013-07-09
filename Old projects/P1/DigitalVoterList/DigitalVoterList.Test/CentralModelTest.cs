namespace DigitalVoterList.Test
{
    using System.Collections.Generic;

    using Central.Central.Models;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Unit Tests for the overall Model class in 'Central'.
    /// NOTICE: A database connection is required to instantiate one or more of the submodels.
    /// </summary>
    [TestClass]
    public class CentralModelTest
    {
        private Model model;

        /// <summary>
        /// Instantiate a model with 2 different sub-models open.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.model = new Model();
            this.model.OpenSubModel(Model.ChangeType.VCG);
            this.model.OpenSubModel(Model.ChangeType.VBM);
        }

        /// <summary> 
        /// Test that submodels can be opened and retrieved. 
        /// </summary>
        [TestMethod]
        public void OpenSubModels()
        {
            // Verify that the submodels opened by the SetUp() are correct.
            IEnumerator<ISubModel> en = this.model.GetSubModels();
            
            en.MoveNext();
            Assert.IsInstanceOfType(en.Current, typeof(VoterCardGenerator));

            en.MoveNext();
            Assert.IsInstanceOfType(en.Current, typeof(VoterBoxManager));

            // Verify that these are the two only models.
            Assert.IsFalse(en.MoveNext());
        }

        /// <summary> 
        /// Test that submodels can be closed and are gone thereafter. 
        /// </summary>
        [TestMethod]
        public void CloseSubModels()
        {
            // Retrieve and close the VoterCardGenerator.
            IEnumerator<ISubModel> en = this.model.GetSubModels();
            en.MoveNext();
            ISubModel vcg = en.Current;
            this.model.CloseSubModel(vcg);
            
            // Retrieve submodels and verify that only the VoterBoxManager remains.
            en = this.model.GetSubModels();
            en.MoveNext();
            Assert.IsInstanceOfType(en.Current, typeof(VoterBoxManager));
            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test that subscribers to the model are notified correctly when submodels are opened.
        /// </summary>
        [TestMethod]
        public void OpenNotifications()
        {
            // Subscribe to open events.
            var opened = new List<ISubModel>();
            this.model.SubModelOpened += (t, m) => opened.Add(m);

            // Open some submodels.
            this.model.OpenSubModel(Model.ChangeType.VCG);                  // Open
            Assert.IsTrue(opened.Count == 1);                               // Was I notified?
            Assert.IsInstanceOfType(opened[0], typeof(VoterCardGenerator)); // With the right type?

            this.model.OpenSubModel(Model.ChangeType.VBM);                  // Open
            Assert.IsTrue(opened.Count == 2);                               // Was I notified?
            Assert.IsInstanceOfType(opened[1], typeof(VoterBoxManager));    // With the right type?
        }

        /// <summary>
        /// Test that subscribers to the model are notified correctly when submodels are closed.
        /// </summary>
        [TestMethod]
        public void CloseNotifications()
        {
            // Subscribe to close events.
            var closed = new List<ISubModel>();
            this.model.SubModelClosed += closed.Add;

            // Retrieve submodels.
            IEnumerator<ISubModel> en = this.model.GetSubModels();
            en.MoveNext();
            ISubModel sm1 = en.Current;
            en.MoveNext();
            ISubModel sm2 = en.Current;

            // Close them.
            this.model.CloseSubModel(sm1);          // Close
            Assert.IsTrue(closed.Count == 1);       // Was I notified?
            Assert.IsTrue(closed[0].Equals(sm1));   // With the right instance?
            
            this.model.CloseSubModel(sm2);          // Close
            Assert.IsTrue(closed.Count == 2);       // Was I notified?
            Assert.IsTrue(closed[1].Equals(sm2));   // With the right instance?
        }
    }
}
