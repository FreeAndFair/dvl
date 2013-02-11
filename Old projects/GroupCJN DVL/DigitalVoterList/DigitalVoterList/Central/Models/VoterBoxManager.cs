// -----------------------------------------------------------------------
// <copyright file="VoterBoxManger.cs" company="DVL">
// Author: Jan Meier, Niels Søholm
// </copyright>
// -----------------------------------------------------------------------

namespace Central.Central.Models
{
    using System.Collections.Generic;
    using System.Linq;

    using DBComm.DBComm;
    using DBComm.DBComm.DAO;
    using DBComm.DBComm.DO;

    /// <summary>
    /// The model for a voter box manager, the class responsible for transfering data to voting boxes
    /// </summary>
    public class VoterBoxManager : ISubModel
    {
        public IEnumerable<VoterDO> Voters { get; set; }
        public IEnumerable<MunicipalityDO> Municipalities { get; set; }
        public IEnumerable<PollingStationDO> PollingStations { get; set; }
        public VoterFilter Filter { get; private set; }

        private PollingStationDAO pDAO;
        private MunicipalityDAO mDAO;
        private VoterDAO vDAO;

        public VoterBoxManager(VoterFilter filter)
        {
            this.Filter = filter;
        }

        /// <summary>
        /// Do the polling stations on the VoterBox match the current selection?
        /// </summary>
        /// <returns>Answer to query (yes = true | no = false)</returns>
        public bool ValidatePollingStations()
        {
            var serverPS = pDAO.Read(v => true).ToList();

            foreach (var ps in this.PollingStations)
            {
                if (!serverPS.Contains(ps))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Do the municipalities on the VoterBox match the current selection?
        /// </summary>
        /// <returns>Answer to query (yes = true | no = false)</returns>
        public bool ValidateMunicipalities()
        {
            var serverMunicipalities = mDAO.Read(v => true).ToList();

            foreach (var m in this.Municipalities)
            {
                if (!serverMunicipalities.Contains(m))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Do the voters on the VoterBox match the current selection?
        /// </summary>
        /// <returns>Answer to query (yes = true | no = false)</returns>
        public bool ValidateVoters()
        {
            var serverVoters = vDAO.Read(v => true).ToList();

            foreach (var voter in this.Voters)
            {
                if (!serverVoters.Contains(voter))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Fetch data from the db based on the current filter.
        /// </summary>
        public void FetchData()
        {
            pDAO = new PollingStationDAO(DigitalVoterList.GetDefaultInstance());
            mDAO = new MunicipalityDAO(DigitalVoterList.GetDefaultInstance());
            vDAO = new VoterDAO(DigitalVoterList.GetDefaultInstance());

            VoterFilter f = this.Filter;

            if (f.CPRNO != 0)
            {
                this.Voters = vDAO.Read(v => v.PrimaryKey == f.CPRNO);
                VoterDO voter = this.Voters.First();
                this.PollingStations = pDAO.Read(ps => ps.PrimaryKey == voter.PollingStationId);
                PollingStationDO pollingStation = this.PollingStations.First();
                this.Municipalities = mDAO.Read(m => m.PrimaryKey == pollingStation.MunicipalityId);
            }
            else if (f.PollingStation != null)
            {
                this.PollingStations = pDAO.Read(ps => ps.PrimaryKey == f.PollingStation.PrimaryKey);
                this.Voters = vDAO.Read(v => v.PollingStationId == f.PollingStation.PrimaryKey);
                this.Municipalities = mDAO.Read(m => m.PrimaryKey == f.PollingStation.MunicipalityId);
            }
            else if (f.Municipality != null)
            {
                this.Municipalities = mDAO.Read(m => m.PrimaryKey == f.Municipality.PrimaryKey);
                this.PollingStations = pDAO.Read(p => p.MunicipalityId == f.Municipality.PrimaryKey);

                this.Voters = Enumerable.Empty<VoterDO>();
                foreach (var ps in this.PollingStations)
                {
                    PollingStationDO ps1 = ps;
                    this.Voters = this.Voters.Concat(vDAO.Read(v => v.PollingStationId == ps1.PrimaryKey));
                }
            }
        }

        /// <summary>
        /// Insert the data that was fected onto the remote server.
        /// </summary>
        /// <param name="server">The address of the server.</param>
        /// <param name="port">The port of the server.</param>
        /// <param name="user">The username.</param>
        /// <param name="password">The password.</param>
        public void InsertData(string server, string port, string user, string password)
        {
            foreach (var municipality in this.Municipalities)
            {
                municipality.ResetAssociations();
            }
            foreach (var pollingStation in this.PollingStations)
            {
                pollingStation.ResetAssociations();
            }
            foreach (var voter in this.Voters)
            {
                voter.ResetAssociations();
            }

            var context = DigitalVoterList.GetInstance(user, password, server, port);

            mDAO = new MunicipalityDAO(context);
            pDAO = new PollingStationDAO(context);
            vDAO = new VoterDAO(context);

            mDAO.Create(this.Municipalities);
            pDAO.Create(this.PollingStations);
            vDAO.Create(this.Voters);
        }
    }
}
