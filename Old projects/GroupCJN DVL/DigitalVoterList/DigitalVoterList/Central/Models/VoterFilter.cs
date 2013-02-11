// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VoterCardGenerator.cs" company="DVL">
//   Author: Niels Søholm (nm@9la.dk)
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Central.Central.Models
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using DBComm.DBComm.DO;

    /// <summary>
    /// An immutable filter describing a subset of voters.
    /// </summary>
    public class VoterFilter
    {
        /// <summary> Initializes a new instance of the <see cref="VoterFilter"/> class. </summary>
        /// <param name="municipality"> The selected municipality (null means not selected).</param>
        /// <param name="pollingStation"> The Selected polling station (null means not selected). </param>
        /// <param name="cprno"> The selected CPR number (0 means not selected). </param>
        public VoterFilter(MunicipalityDO municipality = null, PollingStationDO pollingStation = null, int cprno = 0)
        {
            this.Municipality = municipality;
            this.PollingStation = pollingStation;
            this.CPRNO = cprno;
        }

        /// <summary> What is the selected municipality? </summary>
        public MunicipalityDO Municipality { get; private set; }

        /// <summary> What is the selected polling station? </summary>
        public PollingStationDO PollingStation { get; private set; }

        /// <summary> What is the selected cprnr? </summary>
        public int CPRNO { get; private set; }

        /// <summary> What would this filter look like as a predicate? </summary>
        /// <returns> A predicate representing the filter. </returns>
        public Expression<Func<VoterDO, bool>> ToPredicate()
        {
            return v =>
                (this.Municipality == null || v.PollingStation.Municipality.Equals(this.Municipality)) && 
                (this.PollingStation == null || v.PollingStation.Equals(this.PollingStation)) && 
                (this.CPRNO == 0 || v.PrimaryKey.Equals(this.CPRNO));
        }
        
        /// <summary> At least one property should be specified. </summary>
        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Municipality != null || PollingStation != null || CPRNO != 0);
        }
    }
}