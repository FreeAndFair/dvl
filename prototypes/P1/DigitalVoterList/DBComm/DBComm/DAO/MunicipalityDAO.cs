﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MunicipalityDAO.cs" company="DVL">
//   Jan Meier
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DBComm.DBComm.DAO
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    using global::DBComm.DBComm.DO;

    /// <summary>
    /// DAO for the municipalities table.
    /// </summary>
    public class MunicipalityDAO : AbstractDataAccessObject<MunicipalityDO>
    {
        /// <summary>
        /// Create a new DAO that connects to the default server.
        /// </summary>
        public MunicipalityDAO()
            : base()
        {
        }

        /// <summary>
        /// Create a new DAO that connects to the specified server.
        /// </summary>
        /// <param name="c">The connection.</param>
        public MunicipalityDAO(DigitalVoterList c)
            : base(c)
        {
        }

        /// <summary>
        /// Create this object.
        /// </summary>
        /// <param name="t">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// True if the object was created, false otherwise.
        /// </returns>
        public new bool Create(MunicipalityDO t)
        {
            // Method is re-implemented here, because it is not possible to add the ensures in the abstract DAO.
            Contract.Requires(t != null);
            Contract.Requires(t.FullyInitialized());
            Contract.Ensures(this.Read(o => o.PrimaryKey == t.PrimaryKey).Contains(t));
            return base.Create(t);
        }

        /// <summary>
        /// Create this object.
        /// </summary>
        /// <param name="t">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// True if the object was created, false otherwise.
        /// </returns>
        public new bool Create(IEnumerable<MunicipalityDO> t)
        {
            // Method is re-implemented here, because it is not possible to add the ensures in the abstract DAO.
            Contract.Requires(t != null);
            Contract.Requires(Contract.ForAll(t, ts => ts.FullyInitialized()));
            Contract.Ensures(Contract.ForAll(t, ts => this.Read(o => o.PrimaryKey == ts.PrimaryKey).Contains(ts)));
            return base.Create(t);
        }
    }
}
