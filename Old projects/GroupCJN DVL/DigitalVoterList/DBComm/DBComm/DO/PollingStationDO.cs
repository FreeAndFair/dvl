// -----------------------------------------------------------------------
// <copyright file="PollingStationDO.cs" company="DVL">
// <author>Jan Meier</author>
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DO
{
    using System.Collections.Generic;
    using System.Data.Linq;
    using System.Data.Linq.Mapping;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// The class representing a Polling station entity
    /// </summary>
    [Table(Name = "pollingstation")]
    public class PollingStationDO : IDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PollingStationDO"/> class.
        /// </summary>
        /// <param name="municipalityId">
        /// The municipality Id.
        /// </param>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        public PollingStationDO(uint? municipalityId, uint? id, string name, string address)
        {
            this.MunicipalityId = municipalityId;
            this.PrimaryKey = id;
            this.Name = name;
            this.Address = address;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PollingStationDO"/> class. 
        /// Parameter-less constructor required for entity mapping. No field-
        /// initialization is done, i.e. all fields are null.
        /// </summary>
        public PollingStationDO()
        {
        }

        /// <summary>
        /// Gets Primary Key.
        /// </summary>
        [Column(Name = "id", IsPrimaryKey = true)]
        public uint? PrimaryKey { get; private set; }

        /// <summary>
        /// Gets the foreign key referencing a municipality.
        /// </summary>
        [Column]
        public uint? MunicipalityId { get; private set; }

        /// <summary>
        /// Gets Name.
        /// </summary>
        [Column(Name = "name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets Address.
        /// </summary>
        [Column]
        public string Address { get; private set; }

        private EntityRef<MunicipalityDO> _municipality = new EntityRef<MunicipalityDO>();

        /// <summary>
        /// Gets or sets Municipality associated with tihs polling station.
        /// </summary>
        [Association(Name = "FK_PT_M", IsForeignKey = true, Storage = "_municipality", ThisKey = "MunicipalityId")]
        public MunicipalityDO Municipality
        {
            get
            {
                return _municipality.Entity;
            }

            private set
            {
                Contract.Requires(value != null);
                _municipality.Entity = value;
            }
        }

        private EntitySet<VoterDO> _voters = new EntitySet<VoterDO>();

        /// <summary>
        /// Gets or sets Voters associated with this polling station.
        /// </summary>
        [Association(Name = "FK_V_PS", Storage = "_voters", ThisKey = "PrimaryKey", OtherKey = "PollingStationId")]
        public ICollection<VoterDO> Voters
        {
            get
            {
                return _voters;
            }

            private set
            {
                Contract.Requires(value != null);
                _voters.AddRange(value);
            }
        }

        /// <summary>
        /// Has all the fields of the object been initialized?
        /// </summary>
        /// <returns>
        /// True if all fields are non-null.
        /// </returns>
        public bool FullyInitialized()
        {
            return this.Address != null && this.PrimaryKey != null && this.MunicipalityId != null && this.Name != null;
        }

        /// <summary>
        /// Set the values of this object to the values of the dummy object, if the dummys value is non-null.
        /// </summary>
        /// <param name="dummy">
        /// The dummy object, representing new values.
        /// </param>
        public void UpdateValues(IDataObject dummy)
        {
            PollingStationDO pollingDummy = dummy as PollingStationDO;
            Contract.Assert(pollingDummy != null);

            this.Address = pollingDummy.Address ?? this.Address;
            this.PrimaryKey = pollingDummy.PrimaryKey ?? this.PrimaryKey;
            this.MunicipalityId = pollingDummy.MunicipalityId ?? this.MunicipalityId;
            this.Name = pollingDummy.Name ?? this.Name;
        }

        public void ResetAssociations()
        {
            this._municipality = new EntityRef<MunicipalityDO>();
            this._voters = new EntitySet<VoterDO>();
        }

        public override string ToString()
        {
            return this.Name ?? string.Empty;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = obj as PollingStationDO;

            return other.PrimaryKey == this.PrimaryKey && other.Address == this.Address && other.MunicipalityId == this.MunicipalityId
                   && other.Name == this.Name;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            if (Name != null)
                return this.Name.GetHashCode();

            return 0;
        }
    }
}