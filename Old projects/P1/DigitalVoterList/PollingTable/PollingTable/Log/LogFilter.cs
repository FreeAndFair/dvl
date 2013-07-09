// -----------------------------------------------------------------------
// <copyright file="LogFilter.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace PollingTable.PollingTable.Log
{
    using System;

    using DBComm.DBComm.DO;

    /// <summary>
    /// A class representing filter parameters from the log window.
    /// </summary>
    public struct LogFilter
    {
        public uint? Table { get; set; }

        public uint? Cpr { get; set; }

        public ActivityEnum? Activity { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }
    }
}
