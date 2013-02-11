// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Controllers
{
    using System.IO;
    using System.Windows.Forms;

    using global::Central.Central.Models;
    using global::Central.Central.Views;

    /// <summary>
    /// The controller responsible for monitoring the VoterCardGeneartorWindow (view)
    /// and propagating input in an appropiate fashion to the VoterCardGenerator (model).
    /// </summary>
    public class VoterCardGeneratorController
    {
        private readonly VoterCardGenerator model;
        private readonly VoterCardGeneratorWindow view;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoterCardGeneratorController"/> class.
        /// </summary>
        /// <param name="model"> The associated model (Voter Card Generator). </param>
        /// <param name="view"> The associated view (Voter Card Generator Window). </param>
        public VoterCardGeneratorController(VoterCardGenerator model, VoterCardGeneratorWindow view)
        {
            this.model = model;
            this.view = view;

            // Subscribe to View
            view.AddGenerateHandler(this.GenerateHandler);
            view.AddAbortHandler(model.Abort);

            // Show View
            view.Show();
        }

        /// <summary>
        /// React to generate request.
        /// Validate selection and destination before passing on final generate order.
        /// </summary>
        /// <param name="destination">Destination folder for resulting files (folder path).</param>
        /// <param name="property">Index of the desired grouping function.</param>
        /// <param name="limit">Batch size limit (voters / file).</param>
        public void GenerateHandler(string destination, int property, int limit)
        {
            // Selection validation.
            int old = model.ValidateSelection();
            DialogResult result = DialogResult.None;
            if (old > 0) result = MessageBox.Show("The selection contains " + old + " voters who already had their voter cards generated previously, do you wish to continue?", "Cards already generated", MessageBoxButtons.YesNo);
            if (old == -1) return;
            if (result == DialogResult.No) return;

            // Destination validation.
            result = DialogResult.None;
            if (Directory.Exists(destination)) result = MessageBox.Show("The specified folder does not exist and will be created, do you wish to continue?", "Unknown folder", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK) Directory.CreateDirectory(destination);
            else if (result == DialogResult.Cancel) return;

            view.GeneratingMode("Initializing..");
            model.Generate(destination, property, limit);
        }
    }
}
