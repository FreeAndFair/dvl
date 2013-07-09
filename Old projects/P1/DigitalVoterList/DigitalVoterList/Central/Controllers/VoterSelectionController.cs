// -----------------------------------------------------------------------
// <copyright file="VoterSelectionController.cs" company="DVL">
// Authors: Jan Meier, Niels Søholm
// </copyright>
// -----------------------------------------------------------------------

namespace Central.Central.Controllers
{
    using System;

    using DBComm.DBComm.DO;

    using global::Central.Central.Models;
    using global::Central.Central.Views;

    /// <summary>
    /// The controller responsible for monitoring the VoterSelectionWindow (view)
    /// and propagating input in an appropiate fashion to the VoterCardGenerator (model).
    /// </summary>
    public class VoterSelectionController
    {
        private VoterSelection mainModel;
        private VoterSelectionWindow view;

        private bool updating = false;

        /// <summary> Initializes a new instance of the <see cref="VoterSelectionController"/> class. </summary>
        /// <param name="mainModel"> The main model. </param>
        /// <param name="view"> A voter selection view. </param>
        public VoterSelectionController(VoterSelection mainModel, VoterSelectionWindow view)
        {
            this.mainModel = mainModel;
            this.view = view;

            view.AddPSSelectionChangedHandler(this.PSSelectionChanged);
            view.AddMSelectionChangedHandler(this.MSelectionChanged);
            view.addCPRTextChangedHandler(this.CPRTextChanged);

            this.view.Closed += (o, eA) => Environment.Exit(-1);
        }

        /// <summary>
        /// React to municipality filter selection.
        /// </summary>
        /// <param name="changedTo">The municipality that has been selected.</param>
        public void MSelectionChanged(object changedTo)
        {
            if (!this.updating)
            {
                this.updating = true;

                MunicipalityDO m = changedTo as MunicipalityDO;
                VoterFilter filter = new VoterFilter(m);

                this.view.DisablePSSelectionHandlers();

                mainModel.ReplaceFilter(filter);
            }
            this.updating = false;

            this.view.EnablePSSelectionHandlers();
        }

        /// <summary> 
        /// React to polling station filter selection.
        /// </summary>
        /// <param name="changedTo">The polling station that has been selected.</param>
        public void PSSelectionChanged(object changedTo)
        {
            this.view.ResetCPRText();

            PollingStationDO p = changedTo as PollingStationDO;
            VoterFilter filter = new VoterFilter(p.Municipality, p);

            this.view.DisablePSSelectionHandlers();

            this.mainModel.ReplaceFilter(filter);

            this.view.EnablePSSelectionHandlers();
        }

        /// <summary>
        /// React to CPR number search.
        /// </summary>
        /// <param name="changedTo">The CPR number that is being searched for.</param>
        public void CPRTextChanged(string changedTo)
        {
            if (!this.updating)
            {
                this.updating = true;

                if (changedTo.Length == 10)
                {
                    long cpr = long.Parse(changedTo); // It is possible for the user to type in 9999999999, which would not fit an int.
                    if (cpr >= 101000001 && cpr <= 3112999999) mainModel.ReplaceFilter(new VoterFilter(null, null, (int)cpr));
                }
            }
            this.updating = false;
        }
    }
}