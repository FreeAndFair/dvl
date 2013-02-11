using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DigitalVoterList.Test
{
    using Central.Central.Models;
    using Central.Central.Views;

    [TestClass]
    public class CentralModelViewTest
    {
        private Model model;
        private View view;

        /// <summary>
        /// Instantiate a model and a view subscribing to it.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            this.model = new Model();
            this.view = new View(this.model);
        }

        /// <summary>
        /// Test that the correct sub-views are made when sub-models are opened.
        /// </summary>
        [TestMethod]
        public void OpenSubSystems()
        {
            // Verify that no views are open.
            IEnumerator<ISubView> en = this.view.GetSubViews();
            Assert.IsFalse(en.MoveNext());

            // Open a single view.
            this.model.OpenSubModel(Model.ChangeType.VCG);
            en = this.view.GetSubViews();
            en.MoveNext();
            
            // Verify that the correct view has been opened.
            Assert.IsInstanceOfType(en.Current, typeof(VoterCardGeneratorWindow));
            Assert.IsFalse(en.MoveNext());

            // Open a second.
            this.model.OpenSubModel(Model.ChangeType.VBM);
            en = this.view.GetSubViews();
            en.MoveNext();

            // Verify that the correct view(s) have been opened.
            Assert.IsInstanceOfType(en.Current, typeof(VoterCardGeneratorWindow));
            en.MoveNext();
            Assert.IsInstanceOfType(en.Current, typeof(VoterBoxManagerWindow));
            Assert.IsFalse(en.MoveNext());
        }

        /// <summary>
        /// Test that the correct sub-views are closed when sub-models are closed.
        /// </summary>
        [TestMethod]
        public void CloseSubSystems()
        {
            // Set up an object scenario similar to the end of 'OpenSubSystems()'.
            // ie. a model/view system with 2 sub-systems open.
            this.model.OpenSubModel(Model.ChangeType.VCG);
            this.model.OpenSubModel(Model.ChangeType.VBM);

            // Retrieve the 2 sub-systems.
            IEnumerator<ISubModel> en = this.model.GetSubModels();
            en.MoveNext();
            ISubModel vcg = en.Current;
            Assert.IsInstanceOfType(vcg, typeof(VoterCardGenerator));
            en.MoveNext();
            ISubModel vbm = en.Current;
            Assert.IsInstanceOfType(vbm, typeof(VoterBoxManager));
            Assert.IsFalse(en.MoveNext());

            // Order the VoterCardGenerator to be closed.
            this.model.CloseSubModel(vcg);

            // Retrieve sub-views and verify that only the VoterBoxManagerWindow remains.
            IEnumerator<ISubView> env = this.view.GetSubViews();
            env.MoveNext();
            Assert.IsInstanceOfType(env.Current, typeof(VoterBoxManagerWindow));
            Assert.IsFalse(env.MoveNext());

            // Order the VoterBoxManager to be clsoed.
            this.model.CloseSubModel(vbm);

            // Retrieve sub-views and verify that no sub-views remain.
            env = this.view.GetSubViews();
            Assert.IsFalse(env.MoveNext());
        }

        /// <summary>
        /// Test that subscribers to the view are notified correctly when submodels and subviews are opened.
        /// </summary>
        [TestMethod]
        public void OpenNotifications()
        {
            // Subscribe to view opened events.
            var opened = new List<ISubView>();
            this.view.SubViewOpened += (t, sv) => opened.Add(sv);

            // Open some submodels.
            this.model.OpenSubModel(Model.ChangeType.VCG);                          // Open
            Assert.IsTrue(opened.Count == 1);                                       // Was I notified?
            Assert.IsInstanceOfType(opened[0], typeof(VoterCardGeneratorWindow));   // With the right type?

            this.model.OpenSubModel(Model.ChangeType.VBM);                          // Open
            Assert.IsTrue(opened.Count == 2);                                       // Was I notified?
            Assert.IsInstanceOfType(opened[1], typeof(VoterBoxManagerWindow));      // With the right type?
        }
    }
}
