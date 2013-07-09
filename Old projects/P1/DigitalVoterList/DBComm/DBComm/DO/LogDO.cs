// -----------------------------------------------------------------------
// <copyright file="LogDO.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DO
{
    using System;
    using System.Data.Linq.Mapping;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The class representing a Log entry entity
    /// </summary>
    [Table(Name = "log")]
    public class LogDO : IDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public LogDO(uint? table, uint? cpr, ActivityEnum? activity)
        {
            this.Table = table;
            this.Cpr = cpr;
            this.Activity = activity;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogDO"/> class.
        /// </summary>
        public LogDO()
        {

        }

        ///// <summary>
        ///// Gets the primary key of this data object.
        ///// </summary>
        [Column(Name = "id")]
        public uint? PrimaryKey { get; private set; }

        /// <summary>
        /// Gets Time.
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public DateTime? Time { get; private set; }

        /// <summary>
        /// Gets Table.
        /// </summary>
        [Column(Name = "tableNo", IsPrimaryKey = true)]
        public uint? Table { get; set; }

        /// <summary>
        /// Gets Cpr.
        /// </summary>
        [Column]
        public uint? Cpr { get; set; }

        /// <summary>
        /// Gets Activity.
        /// </summary>
        [Column(IsPrimaryKey = true)]
        public ActivityEnum? Activity { get; set; }

        /// <summary>
        /// Has all the fields of the object been initialized?
        /// </summary>
        /// <returns>
        /// True if all fields are non-null.
        /// </returns>
        public bool FullyInitialized()
        {
            return Table != null && Cpr != null && Activity != null;
        }

        /// <summary>
        /// Set the values of this object to the values of the dummy object, if the dummys value is non-null.
        /// </summary>
        /// <param name="dummy">
        /// The dummy object, representing new values.
        /// </param>
        public void UpdateValues(IDataObject dummy)
        {
            LogDO logDummy = dummy as LogDO;
            Contract.Assert(logDummy != null);

            this.Time = logDummy.Time ?? this.Time;
            this.Table = logDummy.Table ?? this.Table;
            this.Cpr = logDummy.Cpr ?? this.Cpr;
            this.Activity = logDummy.Activity ?? this.Activity;
        }

        /// <summary>
        /// Reset all associations.
        /// </summary>
        public void ResetAssociations()
        {
            // No associations, so nothing to reset here.
        }
    }
}