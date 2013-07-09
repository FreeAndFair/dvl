// <copyright file="MunicipalityDOTest.cs">Copyright ©  2011</copyright>

namespace DBComm.PexTests
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using System.Collections.Generic;

    using global::DBComm.DBComm.DO;

    [TestClass]
    [PexClass(typeof(MunicipalityDO))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class MunicipalityDOTest
    {
        [PexMethod]
        public uint? PrimaryKeyGet([PexAssumeUnderTest]MunicipalityDO target)
        {
            uint? result = target.PrimaryKey;
            return result;
            // TODO: add assertions to method MunicipalityDOTest.PrimaryKeyGet(MunicipalityDO)
        }
        [PexMethod]
        public ICollection<PollingStationDO> PollingStationsGet([PexAssumeUnderTest]MunicipalityDO target)
        {
            ICollection<PollingStationDO> result = target.PollingStations;
            return result;
            // TODO: add assertions to method MunicipalityDOTest.PollingStationsGet(MunicipalityDO)
        }
        [PexMethod]
        public void UpdateValues([PexAssumeUnderTest]MunicipalityDO target, IDataObject dummy)
        {
            target.UpdateValues(dummy);
            // TODO: add assertions to method MunicipalityDOTest.UpdateValues(MunicipalityDO, IDataObject)
        }
        [PexMethod]
        public string ToString01([PexAssumeUnderTest]MunicipalityDO target)
        {
            string result = target.ToString();
            return result;
            // TODO: add assertions to method MunicipalityDOTest.ToString01(MunicipalityDO)
        }
        [PexMethod]
        public void ResetAssociations([PexAssumeUnderTest]MunicipalityDO target)
        {
            target.ResetAssociations();
            // TODO: add assertions to method MunicipalityDOTest.ResetAssociations(MunicipalityDO)
        }
        [PexMethod]
        public int GetHashCode01([PexAssumeUnderTest]MunicipalityDO target)
        {
            int result = target.GetHashCode();
            return result;
            // TODO: add assertions to method MunicipalityDOTest.GetHashCode01(MunicipalityDO)
        }
        [PexMethod]
        public bool FullyInitialized([PexAssumeUnderTest]MunicipalityDO target)
        {
            bool result = target.FullyInitialized();
            return result;
            // TODO: add assertions to method MunicipalityDOTest.FullyInitialized(MunicipalityDO)
        }
        [PexMethod]
        public bool Equals01([PexAssumeUnderTest]MunicipalityDO target, object obj)
        {
            bool result = target.Equals(obj);
            return result;
            // TODO: add assertions to method MunicipalityDOTest.Equals01(MunicipalityDO, Object)
        }
        [PexMethod]
        public MunicipalityDO Constructor01()
        {
            MunicipalityDO target = new MunicipalityDO();
            return target;
            // TODO: add assertions to method MunicipalityDOTest.Constructor01()
        }
        [PexMethod]
        public MunicipalityDO Constructor(
            uint? id,
            string address,
            string city,
            string name
        )
        {
            MunicipalityDO target = new MunicipalityDO(id, address, city, name);
            return target;
            // TODO: add assertions to method MunicipalityDOTest.Constructor(Nullable`1<UInt32>, String, String, String)
        }
    }
}
