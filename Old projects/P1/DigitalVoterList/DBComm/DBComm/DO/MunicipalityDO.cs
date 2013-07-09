// -----------------------------------------------------------------------
// <copyright file="MunicipalityDO.cs" company="DVL">
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
    /// The entity representing a Municipality entity
    /// </summary>
    [Table(Name = "municipality")]
    public class MunicipalityDO : IDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MunicipalityDO"/> class.
        /// </summary>
        /// <param name="id">
        /// The id.
        /// </param>
        /// <param name="address">
        /// The address.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        public MunicipalityDO(uint? id, string address, string city, string name)
        {
            this.Id = id;
            this.Address = address;
            this.City = city;
            this.Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MunicipalityDO"/> class. 
        /// Parameter-less constructor required for entity mapping. No field-
        /// initialization is done, i.e. all fields are null.
        /// </summary>
        public MunicipalityDO()
        {
        }

        /// <summary>
        /// Gets PrimaryKey.
        /// </summary>
        [Column(IsPrimaryKey = true, Name = "id")]
        public uint? PrimaryKey
        {
            get
            {
                return this.Id;
            }

            private set
            {
                this.Id = value;
            }
        }

        private EntitySet<PollingStationDO> _pStations = new EntitySet<PollingStationDO>();

        [Association(Name = "FK_PT_M", Storage = "_pStations", ThisKey = "PrimaryKey", OtherKey = "MunicipalityId")]
        public ICollection<PollingStationDO> PollingStations
        {
            get
            {
                return _pStations;
            }

            private set
            {
                Contract.Requires(value != null);
                _pStations.AddRange(value);
            }
        }

        /// <summary>
        /// Gets Id.
        /// </summary>
        public uint? Id { get; private set; }

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
        /// Gets Name.
        /// </summary>
        [Column]
        public string Name { get; private set; }

        public void ResetAssociations()
        {
            _pStations = new EntitySet<PollingStationDO>();
        }

        #region Implementation of IDataObject

        /// <summary>
        /// Has all the fields of the object been initialized?
        /// </summary>
        /// <returns>
        /// True if all fields are non-null.
        /// </returns>
        public bool FullyInitialized()
        {
            return Id != null && Address != null && Name != null && City != null;
        }

        /// <summary>
        /// Set the values of this object to the values of the dummy object, if the dummys value is non-null.
        /// </summary>
        /// <param name="dummy">
        /// The dummy object, representing new values.
        /// </param>
        public void UpdateValues(IDataObject dummy)
        {
            MunicipalityDO municipalityDummy = dummy as MunicipalityDO;
            Contract.Assert(municipalityDummy != null);

            this.Address = municipalityDummy.Address ?? this.Address;
            this.City = municipalityDummy.City ?? this.City;
            this.Id = municipalityDummy.Id ?? this.Id;
            this.Name = municipalityDummy.Name ?? this.Name;
        }

        #endregion

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

            var other = obj as MunicipalityDO;

            return other.PrimaryKey == this.PrimaryKey && other.Address == this.Address && other.City == this.City && other.Id == this.Id
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