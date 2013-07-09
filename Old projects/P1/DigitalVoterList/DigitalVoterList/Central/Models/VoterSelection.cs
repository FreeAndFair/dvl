// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Authors: Jan Meier, Niels Søholm
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DBComm.DBComm;
    using DBComm.DBComm.DAO;
    using DBComm.DBComm.DO;

    /// <summary>
    /// Responsible for composing a VoterFilter representing a selection of voters.
    /// </summary>
    public class VoterSelection : ISubModel
    {
        private int voterCount;
        private IEnumerable<PollingStationDO> pollingStations;
        private IEnumerable<MunicipalityDO> municipalities;
        private VoterFilter currentFilter;
        private MunicipalityDO selectedMunicipality;
        private PollingStationDO selectedPollingStation;

        private PollingStationDAO pDAO;
        private MunicipalityDAO mDAO;
        private VoterDAO vDAO;

        /// <summary> Initializes a new instance of the <see cref="VoterSelection"/> class with proper values for the default selection. </summary>
        public VoterSelection()
        {
            pDAO = new PollingStationDAO(DigitalVoterList.GetDefaultInstance());
            mDAO = new MunicipalityDAO(DigitalVoterList.GetDefaultInstance());
            vDAO = new VoterDAO(DigitalVoterList.GetDefaultInstance());

            // Call database to get initial values (no selection, ie. entire DB)
            try
            {
                Municipalities = mDAO.Read(o => true);
                PollingStations = pDAO.Read(o => true);
                voterCount = vDAO.Read(o => true).Count();
                currentFilter = null;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(
                    string.Format("The system was not able to connect to the server. The system said: {0}", e.Message));
                Environment.Exit(-1);
            }
        }

        // Custom delegates for the events below
        public delegate void VoterCountHandler(int voterCount);
        public delegate void PollingStationsHandler(IEnumerable<PollingStationDO> pollingStations);
        public delegate void SelectedMunicipalityHandler(MunicipalityDO municipality);
        public delegate void SelectedPollingStationHandler(PollingStationDO pollingStation);

        ///<summary> Notify me when the selected municipality changes. </summary>
        public event SelectedMunicipalityHandler SelectedMunicipalityChanged;
        ///<summary> Notify me when the selected polling station changess. </summary>
        public event SelectedPollingStationHandler SelectedPollingStationChanged;
        ///<summary> Notify me when the number of voters in the selection changes. </summary>
        public event VoterCountHandler VoterCountChanged;
        /// <summary> Notify me when the available polling station changes. </summary>
        public event PollingStationsHandler PollingStationsChanged;

        /// <summary> What is the number of selected voters? </summary>
        public int VoterCount
        {
            // Set the voter count and notify subscribers.
            get
            {
                return voterCount;
            }

            private set
            {
                this.voterCount = value;
                this.VoterCountChanged(this.voterCount);
            }
        }

        /// <summary> What are the available polling stations? </summary>
        public IEnumerable<PollingStationDO> PollingStations
        {
            // Set the polling stations and notify subscribers.
            get
            {
                return pollingStations;
            }

            private set
            {
                this.pollingStations =
                    new List<PollingStationDO>() { new PollingStationDO(uint.MaxValue, uint.MaxValue, "All Polling Stations", "NONE") }.Union(
                            value).ToList();
                this.OnPollingStationsChanged(this.pollingStations);
            }
        }

        /// <summary> What are the available municipalities? </summary>
        public IEnumerable<MunicipalityDO> Municipalities
        {
            get
            {
                return municipalities;
            }

            private set
            {
                this.municipalities =
                    new List<MunicipalityDO>() { new MunicipalityDO(uint.MaxValue, "NONE", "NONE", "All Municipalities") }.Union(
                        value).ToList();
            }
        }

        public MunicipalityDO SelectedMunicipality
        {
            get
            {
                return this.selectedMunicipality;
            }

            private set
            {
                this.selectedMunicipality = value;
                this.SelectedMunicipalityChanged(value);
            }
        }

        public PollingStationDO SelectedPollingStation
        {
            get
            {
                return this.selectedPollingStation;
            }

            private set
            {
                this.selectedPollingStation = value;
                this.SelectedPollingStationChanged(value);
            }
        }

        /// <summary> What is the current filter? </summary>
        public VoterFilter CurrentFilter
        {
            get
            {
                return currentFilter;
            }
        }

        /// <summary> Replace the current voter filter with this voter filter. </summary>
        public void ReplaceFilter(VoterFilter filter)
        {
            this.currentFilter = filter;

            IEnumerable<VoterDO> voters = null;
            if (filter.CPRNO != 0)
            {
                voters = vDAO.Read(v => v.PrimaryKey == filter.CPRNO);
                if (voters.Count() > 0)
                {
                    SelectedMunicipality = voters.Single().PollingStation.Municipality;
                    SelectedPollingStation = voters.Single().PollingStation;
                }
            }
            else if (filter.PollingStation != null)
            {
                if (filter.PollingStation.Name.Equals("All Polling Stations"))
                {
                    voters = vDAO.Read(v => v.PollingStation.MunicipalityId == selectedMunicipality.Id);
                }
                else
                {
                    voters = vDAO.Read(v => v.PollingStation == filter.PollingStation);
                    SelectedMunicipality = filter.PollingStation.Municipality;
                    PollingStations = pDAO.Read(p => p.MunicipalityId == filter.PollingStation.MunicipalityId);
                    SelectedPollingStation = filter.PollingStation;
                }
            }
            else if (filter.Municipality != null)
            {
                if (filter.Municipality.Name.Equals("All Municipalities"))
                {
                    voters = vDAO.Read(v => true);
                    PollingStations = pDAO.Read(o => true);
                }
                else
                {
                    voters = vDAO.Read(v => v.PollingStation.Municipality == filter.Municipality);
                    PollingStations = pDAO.Read(o => o.Municipality == filter.Municipality);
                }
            }

            VoterCount = voters.Count(); // The invariant for Filter stipulates, that at least one field will be initialized, and therefore this will never be null.
        }

        public void OnPollingStationsChanged(IEnumerable<PollingStationDO> pollingStations)
        {
            if (PollingStationsChanged != null) PollingStationsChanged(pollingStations);
        }
    }
}