// -----------------------------------------------------------------------
// <copyright file="LogModel.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.Log
{
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using DBComm.DBComm;
    using DBComm.DBComm.DAO;
    using DBComm.DBComm.DO;

    /// <summary>
    /// Responsible for storing and updating log data.
    /// </summary>
    public class LogModel
    {
        private readonly string password;
        private readonly string server;

        private BindingList<LogDO> logs;
        private LogDAO lDAO;

        private IEnumerable<VoterDO> voters;
        private VoterDAO vDAO;

        private LogFilter filter;

        private int totalVoters;
        private int votedVoters;

        /// <summary>
        /// Create a new model that fetches data from the specified server.
        /// </summary>
        /// <param name="password">The password to the server</param>
        /// <param name="server">The address of the server.</param>
        public LogModel(string password, string server)
        {
            this.password = password;
            this.server = server;

            var conn = DigitalVoterList.GetInstance("groupCJN", this.password, this.server);

            this.logs = new BindingList<LogDO>();
            this.lDAO = new LogDAO(conn);

            this.vDAO = new VoterDAO(conn);

            this.Update();

            this.filter = new LogFilter();
        }

        /// <summary>
        /// Gets the current log entries.
        /// </summary>
        public BindingList<LogDO> Logs
        {
            get
            {
                return this.logs;
            }
        }

        /// <summary>
        /// Gets the total number of voters
        /// </summary>
        public int TotalVoters
        {
            get
            {
                return totalVoters;
            }
        }

        /// <summary>
        /// Get the total number of voters who have voted.
        /// </summary>
        public int VotedVoters
        {
            get
            {
                return votedVoters;
            }
        }

        public void UpdateFilter(LogFilter f)
        {
            this.filter = f;
            this.logs.Clear();
            this.Update();
        }

        /// <summary>
        /// Update the logs based on the current filter.
        /// </summary>
        public void Update()
        {
            var result = this.lDAO.Read(l =>
                (this.filter.Activity != null ? l.Activity == this.filter.Activity : true) &&
                (this.filter.Cpr != null ? l.Cpr == this.filter.Cpr : true) &&
                (this.filter.From != null ? l.Time >= this.filter.From : true) &&
                (this.filter.To != null ? l.Time <= this.filter.To : true) &&
                (this.filter.Table != null ? l.Table == this.filter.Table : true));

            foreach (var logDO in result)
            {
                if (!logs.Contains(logDO)) logs.Add(logDO);
            }

            voters = vDAO.Read(v => true).ToList();

            totalVoters = voters.Count();

            votedVoters = voters.Count(v => v.Voted == true);
        }
    }
}