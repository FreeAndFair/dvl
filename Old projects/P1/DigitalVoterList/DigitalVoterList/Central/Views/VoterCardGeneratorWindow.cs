// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Views
{
    using System;
    using System.Windows.Forms;

    using global::Central.Central.Models;

    /// <summary>
    /// A view for representing and manipulating a Voter Card Generator.
    /// 
    /// Depicts the current state of a Voter Card Generator.
    /// Raises events about user input (to be listened to by controllers).
    /// </summary>
    public partial class VoterCardGeneratorWindow : Form, ISubView
    {
        private const string DefaultDestination = "C:\\VoterCards";
        private readonly VoterCardGenerator model;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoterCardGeneratorWindow"/> class.
        /// </summary>
        /// <param name="model"> Model depicted by the view. </param>
        public VoterCardGeneratorWindow(VoterCardGenerator model)
        {
            InitializeComponent();
            this.model = model;
            this.InitialValues();    // Set up initial values for controls.
            this.SubscribeToModel(); // Subscribe to changes in model.
        }

        /// <summary> Notify me when the generate button is clicked. </summary>
        /// <param name="handler">The handler to be called upon click.</param>
        public void AddGenerateHandler(Action<String, int, int> handler)
        {
            btnGenerate.Click += (a, eA) => this.SetUpGeneration(handler);
        }

        /// <summary> Notify me when the abort button is clicked. </summary>
        /// <param name="handler">The handler to be called upon click.</param>
        public void AddAbortHandler(Action handler)
        {
            btnAbort.Click += (a, eA) => handler();
        }

        /// <summary> Notify me when the view has been ordered to close. </summary>
        /// <param name="handler">The handler to be called upon closing.</param>
        public void AddClosingHandler(Action<ISubModel> handler)
        {
            this.Disposed += (o, eA) => handler(model);
        }

        /// <summary>
        /// Switch to 'Generating Mode'.
        /// </summary>
        /// <param name="statusText">The status text to be shown.</param>
        public void GeneratingMode(String statusText)
        {
            btnAbort.Visible = true;
            btnGenerate.Visible = false;
            this.lblStatus.Text = statusText;
        }

        /// <summary>
        /// Return to 'Inactive Mode'.
        /// </summary>
        /// <param name="statusText">The status text to be shown.</param>
        public void InactiveMode(String statusText)
        {
            btnGenerate.Visible = true;
            btnAbort.Visible = false;
            this.lblStatus.Text = statusText;
        }

        /// <summary> May I have the associated model? </summary>
        /// <returns>The associated model.</returns>
        public ISubModel GetModel()
        {
            return model; // In an ideal world this would be a property, but interfaces can't contain properties.
        }

        /// <summary>
        /// Show database error dialog.
        /// </summary>
        public void ShowDBError()
        {
            MessageBox.Show(
                "The connection to the voter box has been lost. Please contact a manager to restore connection.", "Connection error");
        }

        /// <summary>
        /// Set up initial values for controls.
        /// </summary>
        private void InitialValues()
        {
            VoterFilter filter = model.Filter;
            if (filter != null)
            {
                if (filter.Municipality != null) txbMunicipality.Text = filter.Municipality.Name;
                if (filter.PollingStation != null) txbPollingStation.Text = filter.PollingStation.Name;
                if (filter.CPRNO > 0) txbCPR.Text = filter.CPRNO.ToString();
            }
            txbDestination.Text = DefaultDestination;
        }

        /// <summary>
        /// Subscribe to changes in model.
        /// </summary>
        private void SubscribeToModel()
        {
            this.model.VoterDonePercChanged += (i => pbrVoters.Value = i);
            this.model.GroupCountChanged += (i => pbrGroups.Maximum = i);
            this.model.GroupDoneCountChanged += (i => pbrGroups.Value = i);
            this.model.CurrentGroupChanged += this.UpdateStatus;
            this.model.GenerationEnded += this.InactiveMode;
            this.model.UnableToConnectEvent += this.ShowDBError;
        }

        /// <summary>
        /// Update the status label.
        /// (Requires an explicit refresh to avoid being overshadowed by ProgressBar updates).
        /// </summary>
        /// <param name="statusText">The status text to be shown.</param>
        private void UpdateStatus(string statusText)
        {
            this.lblStatus.Text = statusText;
            this.Refresh();
        }

        /// <summary>
        /// Open FolderBrowser upon 'Browse' button clicked.
        /// </summary>
        /// <param name="sender">The caller (browse button).</param>
        /// <param name="e">Parameters describing the event.</param>
        private void Browse(object sender, EventArgs e)
        {
            destinationFolderBrowser.ShowDialog();
            txbDestination.Text = destinationFolderBrowser.SelectedPath;
        }

        /// <summary>
        /// Prepare for generation and call generate handler when done.
        /// </summary>
        /// <param name="handler">The generate handler.</param>
        private void SetUpGeneration(Action<String, int, int> handler)
        {
            // Retrieve grouping input, if checked by user.
            string destination = txbDestination.Text;
            int property = -1;
            int limit = -1;
            if (chbProperty.Checked) property = cbxProperty.SelectedIndex;
            if (chbLimit.Checked) limit = Convert.ToInt32(txbLimit.Text);
            
            // Go! (call generate handler)
            handler(destination, property, limit);
        }
    }
}
