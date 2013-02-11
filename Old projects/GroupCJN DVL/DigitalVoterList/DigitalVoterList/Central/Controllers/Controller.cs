// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Controllers
{
    using global::Central.Central.Models;
    using global::Central.Central.Views;

    /// <summary>
    /// The main 'controller' of the application. 
    /// It has the sole responsibility of instantiating instances of the sub-controllers.
    /// 
    /// Monitors the main view for open events and initializes appropiate controllers.
    /// </summary>
    public class Controller
    {
        private readonly Model model; // The assocciated model.
        private readonly View view; // The associated view.
        
        private VoterSelectionController vsController; // Voter selection controller (default controller)

        /// <summary> Initializes a new instance of the <see cref="Controller"/> class. </summary>
        /// <param name="model">The model object to rebort to.</param>
        /// <param name="view">The view to listen to.</param>
        public Controller(Model model, View view)
        {
            this.model = model;
            this.view = view;

            // Subscribe to View.
            view.SubViewOpened += ViewOpened;

            this.InitDefault();
        }

        /// <summary> 
        /// React to a request for a view to be opened; 
        /// Instantiate an appropiate controller. 
        /// </summary>
        /// <param name="type">The type of view to be opened.</param>
        /// <param name="subView">The View to be used.</param>
        public void ViewOpened(Model.ChangeType type, ISubView subView)
        {
            switch(type)
            {
                case Model.ChangeType.VCG :
                    var vcgModel = (VoterCardGenerator)subView.GetModel();
                    var vcgView = (VoterCardGeneratorWindow)subView;
                    vcgView.AddClosingHandler(this.model.CloseSubModel);  
                    new VoterCardGeneratorController(vcgModel, vcgView);
                    break;
                case Model.ChangeType.VBM :
                    var vbmModel = (VoterBoxManager)subView.GetModel();
                    var vbmView = (VoterBoxManagerWindow)subView;
                    vbmView.AddClosingHandler(this.model.CloseSubModel);
                    new VoterBoxManagerController(vbmModel, vbmView);
                    break;
            }
        }

        /// <summary>
        /// Instantiate default sub-controller.
        /// </summary>
        private void InitDefault()
        {
            VoterSelectionWindow vsView = view.VoterSelectionView;
            vsView.AddVCGClickedHandler(model.OpenSubModel);
            vsView.AddVBMClickedHandler(model.OpenSubModel);
            vsController = new VoterSelectionController(model.VoterSelection, view.VoterSelectionView);
        }
    }
}
