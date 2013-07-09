// -----------------------------------------------------------------------
// <copyright file="VoterBoxManger.cs" company="DVL">
// Author: Jan Meier, Niels Søholm
// </copyright>
// -----------------------------------------------------------------------

namespace Central.Central.Views
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    using DBComm.DBComm.DO;

    using global::Central.Central.Models;

    using IDataObject = DBComm.DBComm.DO.IDataObject;

    /// <summary>
    /// Window for representing and manipulating the Voter Box Selector.
    /// Depicts the current state of the Voter Box Selector.
    /// </summary>
    public partial class VoterSelectionWindow : Form, ISubView
    {
        private List<EventHandler> pshandlers;
        private VoterSelection model;

        /// <summary>
        /// A view for controlling and monitoring the Voter Selector.
        /// 
        /// Depicts the current state of the Voter Selector
        /// Raises events about user input (to be listened to by controller(s)).
        /// </summary>
        /// <param name="model"></param>
        public VoterSelectionWindow(VoterSelection model)
        {
            InitializeComponent();

            this.model = model;

            this.pshandlers = new List<EventHandler>();

            // Get initial values (default selection = no filter)
            this.cbxMunicipalities.DataSource = model.Municipalities;
            this.cbxPollingStation.DataSource = model.PollingStations;
            this.setVoterCount(model.VoterCount);

            // Subscribe to updates in model
            model.PollingStationsChanged += this.UpdatePollingStations;
            model.VoterCountChanged += this.UpdateVoterCount;
            model.SelectedMunicipalityChanged += this.UpdateSelectedMunicipality;
            model.SelectedPollingStationChanged += this.UpdateSelectedPollingStation;

            // Setup text box
            this.txbCPRNO.KeyPress += this.TextBoxOnlyAllowDigits;
        }

        /// <summary> Replace the list of polling stations with this list. </summary>
        /// <param name="pollingStations">The new list of polling stations.</param>
        public void UpdatePollingStations(IEnumerable<PollingStationDO> pollingStations)
        {
            this.cbxPollingStation.DataSource = pollingStations;
        }

        /// <summary> Replace the voter count label with this. </summary>
        /// <param name="voterCount">The new voter count.</param>
        public void UpdateVoterCount(int voterCount)
        {
            this.setVoterCount(voterCount);
        }

        /// <summary> Make this municipality the selected municipality. </summary>
        /// <param name="municipality">The municipality to be selected.</param>
        public void UpdateSelectedMunicipality(MunicipalityDO municipality)
        {
            this.cbxMunicipalities.SelectedIndex = this.cbxMunicipalities.Items.IndexOf(municipality);
        }

        /// <summary> Make this polling station the selected polling station. </summary>
        /// <param name="pollingStation">The polling station to be selected.</param>
        public void UpdateSelectedPollingStation(PollingStationDO pollingStation)
        {
            this.cbxPollingStation.SelectedIndex = this.cbxPollingStation.Items.IndexOf(pollingStation);
        }

        // Custom delegates for the events below
        public delegate void CBInputChangedHandler(IDataObject changedTo);
        public delegate void TextInputChangedHandler(string changedTo);

        /// <summary> Notify me when the polling station selection changes. </summary>
        public void AddPSSelectionChangedHandler(CBInputChangedHandler handler)
        {
            ComboBox cps = cbxPollingStation;

            EventHandler pshandler = (o, eA) => handler(cps.SelectedItem as IDataObject);
            pshandlers.Add(pshandler);

            cps.SelectedIndexChanged += pshandler;
        }

        /// <summary>Temporarily disable handlers for polling station selection.</summary>
        public void DisablePSSelectionHandlers()
        {
            foreach (EventHandler handler in pshandlers)
            {
                cbxPollingStation.SelectedIndexChanged -= handler;
            }
        }

        /// <summary>Enable handlers for polling station selection.</summary>
        public void EnablePSSelectionHandlers()
        {
            foreach (EventHandler handler in pshandlers)
            {
                cbxPollingStation.SelectedIndexChanged += handler;
            }
        }

        /// <summary>
        /// Reset the CPR text field.
        /// </summary>
        public void ResetCPRText()
        {
            this.txbCPRNO.Text = string.Empty;
        }

        /// <summary> Notify me when the municipality selection changes. </summary>
        public void AddMSelectionChangedHandler(CBInputChangedHandler handler)
        {
            ComboBox cmp = cbxMunicipalities;
            cmp.SelectedIndexChanged += (o, eA) => handler((IDataObject)cmp.SelectedItem);
        }

        /// <summary> Notify me when the CPR number text changes.</summary>
        public void addCPRTextChangedHandler(TextInputChangedHandler handler)
        {
            TextBox tbc = txbCPRNO;
            tbc.TextChanged += (o, eA) => handler(tbc.Text);
        }

        /// <summary> Notify me when the 'Voter Card Generator' button is clicked. </summary>
        public void AddVCGClickedHandler(Action<Model.ChangeType> handler)
        {
            tsbVCG.Click += (o, eA) => handler(Model.ChangeType.VCG);
            tsmVCG.Click += (o, eA) => handler(Model.ChangeType.VCG);
        }

        /// <summary> Notify me when the 'Voter Box Manager' button is clicked. </summary>
        public void AddVBMClickedHandler(Action<Model.ChangeType> handler)
        {
            tsbVBM.Click += (o, eA) => handler(Model.ChangeType.VBM);
            tsmVBM.Click += (o, eA) => handler(Model.ChangeType.VBM);
        }

        /// <summary> May I have the model associated with this view? </summary>
        /// <returns>The associated model.</returns>
        public ISubModel GetModel() 
        {
            return model; // In an ideal world this would be a property, but interfaces can't contain properties.
        }

        /// <summary> Set the Voter Count Label. </summary>
        /// <param name="count">The new voter count value.</param>
        private void setVoterCount(int count)
        {
            lblVoterCount.Text = count + " voters selected.";
        }

        /// <summary>
        /// Make sure that the textbox only allowes digits.
        /// </summary>
        /// <param name="sender">TextBox calling.</param>
        /// <param name="e">Arguments about the event.</param>
        private void TextBoxOnlyAllowDigits(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }
    }
}
