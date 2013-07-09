// -----------------------------------------------------------------------
// <copyright file="SetupInfo.cs" company="">
// Author: Claes Martinsen
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable
{
    /// <summary>
    /// Contains setup information for polling table.
    /// </summary>
    public struct SetupInfo
    {
        #region Constants and Fields

        public SetupInfo(string ip, uint tableNo)
        {
            this.ip = ip;
            this.tableNo = tableNo;
        }

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }

        public uint TableNo
        {
            get { return tableNo; }
            set { tableNo = value; }
        }

        private string ip;
        private uint tableNo;

        #endregion
    }
}