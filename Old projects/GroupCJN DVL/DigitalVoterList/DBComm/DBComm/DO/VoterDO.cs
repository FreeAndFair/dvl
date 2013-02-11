// -----------------------------------------------------------------------
// <copyright file="VoterDO.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DO
{
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics.Contracts;


    /// <summary>
    /// The class representing a Voter entity
    /// </summary>
    [Table(Name = "voter")]
    public class VoterDO : IDataObject
    {
        private uint? primaryKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="VoterDO"/> class.
        /// </summary>
        /// <param name="pollingStationId">
        /// The polling station id.
        /// </param>
        /// <param name="cpr">
        /// The cpr no.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="city">
        /// The city.
        /// </param>
        /// <param name="cardPrinted">
        /// The card printed status.
        /// </param>
        /// <param name="voted">
        /// The voted status.
        /// </param>
        public VoterDO(uint? pollingStationId, uint? cpr, string name, string address, string city, bool? cardPrinted, bool? voted)
        {
            Contract.Requires(cpr == null || (cpr >= 101000001 && cpr <= 3112999999));

            this.PollingStationId = pollingStationId;
            this.primaryKey = cpr;
            this.Name = name;
            this.Address = address;
            this.City = city;
            this.CardPrinted = cardPrinted;
            this.Voted = voted;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VoterDO"/> class. 
        /// Parameter-less constructor required for entity mapping. No field-
        /// initialization is done, i.e. all fields are null.
        /// </summary>
        public VoterDO()
        {
        }

        /// <summary>
        /// Gets PrimaryKey.
        /// </summary>
        [Column(IsPrimaryKey = true, Name = "cpr")]
        public uint? PrimaryKey
        {
            get
            {
                return this.primaryKey;
            }

            private set
            {
                Contract.Requires(value >= 101000001 && value <= 3112999999);
                this.primaryKey = value;
            }
        }

        /// <summary>
        /// Gets Cpr as a string.
        /// </summary>
        [Pure]
        public string CprString
        {
            get
            {
                Contract.Requires(PrimaryKey != null);
                Contract.Ensures(Contract.Result<string>().Length == 10);

                string result = this.PrimaryKey.ToString();

                if (result.Length < 10)
                {
                    return "0" + result;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the id of the polling station this voter is associated with.
        /// </summary>
        [Column]
        public uint? PollingStationId { get; private set; }

        /// <summary>
        /// Gets Name.
        /// </summary>
        [Column]
        public string Name { get; private set; }

        /// <summary>
        /// Gets Address.
        /// </summary>
        [Column]
        public string Address { get; private set; }

        /// <summary>
        /// Gets City.
        /// </summary>
        [Column]
        public string City { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a card has been printed for this voter.
        /// </summary>
        [Column]
        public bool? CardPrinted { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this voter has voted.
        /// </summary>
        [Column]
        public bool? Voted { get; set; }

        private EntityRef<PollingStationDO> _pollingStation;

        /// <summary>
        /// Gets or sets PollingStation associated with this voter..
        /// </summary>
        [Association(Name = "FK_V_PS", Storage = "_pollingStation", ThisKey = "PollingStationId")]
        public PollingStationDO PollingStation
        {
            get
            {
                return _pollingStation.Entity;
            }

            private set
            {
                Contract.Requires(value != null);
                _pollingStation.Entity = value;
            }
        }

        /// <summary>
        /// Reset all associations.
        /// </summary>
        public void ResetAssociations()
        {
            this._pollingStation = new EntityRef<PollingStationDO>();
        }

        /// <summary>
        /// Is the object fully initialized, i.e. do all fields have non-null values?
        /// </summary>
        /// <returns>True if no fields are null.</returns>
        public bool FullyInitialized()
        {
            return PollingStationId != null && PrimaryKey != null && Name != null && Address != null && CardPrinted != null
                   && Voted != null;
        }

        /// <summary>
        /// Set the values of this object to the values of the dummy object, if the dummys value is non-null.
        /// </summary>
        /// <param name="dummy">
        /// The dummy object, representing new values.
        /// </param>
        public void UpdateValues(IDataObject dummy)
        {
            Contract.Requires(dummy != null); // Re-stipulate this contract, since it must be checked before the added contracts.
            Contract.Requires(dummy.GetType() == this.GetType());
            Contract.Requires(dummy.PrimaryKey == null || (((VoterDO)dummy).PrimaryKey >= 101000001 && ((VoterDO)dummy).PrimaryKey <= 3012999999));

            VoterDO voterDummy = dummy as VoterDO;
            Contract.Assert(voterDummy != null);

            this.PollingStationId = voterDummy.PollingStationId ?? this.PollingStationId;
            this.PrimaryKey = voterDummy.PrimaryKey ?? this.PrimaryKey;
            this.Name = voterDummy.Name ?? this.Name;
            this.City = voterDummy.City ?? this.City;
            this.Address = voterDummy.Address ?? this.Address;
            this.CardPrinted = voterDummy.CardPrinted ?? this.CardPrinted;
            this.Voted = voterDummy.Voted ?? this.Voted;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant((this.PrimaryKey >= 101000001 && this.PrimaryKey <= 3112999999) || this.PrimaryKey == null);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as VoterDO;

            return other.PrimaryKey == this.PrimaryKey && other.Name == this.Name && other.PollingStationId == this.PollingStationId
                   && other.Address == this.Address && other.City == this.City && other.Voted == this.Voted && other.CardPrinted == this.CardPrinted;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            var result = 0;
            if (this.Name != null) result += this.Name.GetHashCode();

            return result;
        }
    }
}
