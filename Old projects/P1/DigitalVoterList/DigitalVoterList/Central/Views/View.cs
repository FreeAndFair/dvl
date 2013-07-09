// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Views
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    using global::Central.Central.Models;

    /// <summary>
    /// The main 'view' of the application. 
    /// It has the sole responsibility of managing instances of the sub-views (windows forms).
    /// Monitors the main 'model' for sub-models being opened and closed.
    /// </summary>
    public class View
    {
        private readonly VoterSelectionWindow vsView; // Voter Selection View (default view).
        private readonly List<ISubView> subViews = new List<ISubView>(); // Currently open sub-views.
        private Model model; // The associated model.

        /// <summary>
        /// Initializes a new instance of the <see cref="View"/> class.
        /// </summary>
        /// <param name="model"> The associated model. </param>
        public View(Model model)
        {
            this.model = model;

            // Subscribe to changes in the model.
            model.SubModelOpened += this.OpenView;
            model.SubModelClosed += this.CloseView;

            // Initiate default sub-view
            this.vsView = new VoterSelectionWindow(model.VoterSelection);
        }

        // Custom delegate for handler of the SubViewOpened event.
        public delegate void ViewChangedHandler(Model.ChangeType type, ISubView view);

        /// <summary> Notify me when a sub-view has been opened. </summary>
        public event ViewChangedHandler SubViewOpened;

        /// <summary> May I have the default sub-view? (Voter Selection Window) </summary>
        public VoterSelectionWindow VoterSelectionView 
        { 
            get
            {
                return vsView;
            } 
        }

        /// <summary>
        /// Which sub-views are currently open?
        /// </summary>
        /// <returns>An enumerator of the current sub-views.</returns>
        public IEnumerator<ISubView> GetSubViews()
        {
            return subViews.GetEnumerator();
        }

        /// <summary> Show the default view (VoterSelectionWindow). </summary>
        public void ShowView()
        {
            this.vsView.Show();
        }

        /// <summary>
        /// Open and initialize a new sub-view of the given type using the given sub-model.
        /// </summary>
        /// <param name="type">Type of the new sub-view.</param>
        /// <param name="subModel">Sub-model of the new sub-view.</param>
        private void OpenView(Model.ChangeType type, ISubModel subModel)
        {
            ISubView subView = null;
            switch (type)
            {
                case Model.ChangeType.VCG:
                    subView = new VoterCardGeneratorWindow((VoterCardGenerator)subModel);
                    break;
                case Model.ChangeType.VBM:
                    subView = new VoterBoxManagerWindow((VoterBoxManager)subModel);
                    break;
            }
            subViews.Add(subView);
            if (SubViewOpened != null) SubViewOpened(type, subView);
        }

        /// <summary>
        /// Close a sub-view associated with a given sub-model.
        /// </summary>
        /// <param name="subModel">The sub-model of the sub-view to be closed.</param>
        private void CloseView(ISubModel subModel)
        {
            ISubView subView = subViews.Find(v => v.GetModel().Equals(subModel));
            subViews.Remove(subView);

            var form = (Form)subView;
            if (!form.Disposing) form.Close();
        }
    }
}
