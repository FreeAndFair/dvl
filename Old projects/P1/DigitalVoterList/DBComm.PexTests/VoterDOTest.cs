// <copyright file="VoterDOTest.cs">Copyright ©  2011</copyright>

namespace DBComm.PexTests
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::DBComm.DBComm.DO;

    [TestClass]
    [PexClass(typeof(VoterDO))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class VoterDOTest
    {
        [PexMethod]
        public uint? PrimaryKeyGet([PexAssumeUnderTest]VoterDO target)
        {
            uint? result = target.PrimaryKey;
            return result;
            // TODO: add assertions to method VoterDOTest.PrimaryKeyGet(VoterDO)
        }
        [PexMethod]
        public PollingStationDO PollingStationGet([PexAssumeUnderTest]VoterDO target)
        {
            PollingStationDO result = target.PollingStation;
            return result;
            // TODO: add assertions to method VoterDOTest.PollingStationGet(VoterDO)
        }
        [PexMethod]
        public string CprStringGet([PexAssumeUnderTest]VoterDO target)
        {
            string result = target.CprString;
            return result;
            // TODO: add assertions to method VoterDOTest.CprStringGet(VoterDO)
        }
        [PexMethod]
        public void UpdateValues([PexAssumeUnderTest]VoterDO target, IDataObject dummy)
        {
            target.UpdateValues(dummy);
            // TODO: add assertions to method VoterDOTest.UpdateValues(VoterDO, IDataObject)
        }
        [PexMethod]
        public void ResetAssociations([PexAssumeUnderTest]VoterDO target)
        {
            target.ResetAssociations();
            // TODO: add assertions to method VoterDOTest.ResetAssociations(VoterDO)
        }
        [PexMethod]
        public int GetHashCode01([PexAssumeUnderTest]VoterDO target)
        {
            int result = target.GetHashCode();
            return result;
            // TODO: add assertions to method VoterDOTest.GetHashCode01(VoterDO)
        }
        [PexMethod]
        public bool FullyInitialized([PexAssumeUnderTest]VoterDO target)
        {
            bool result = target.FullyInitialized();
            return result;
            // TODO: add assertions to method VoterDOTest.FullyInitialized(VoterDO)
        }
        [PexMethod]
        public bool Equals01([PexAssumeUnderTest]VoterDO target, object obj)
        {
            bool result = target.Equals(obj);
            return result;
            // TODO: add assertions to method VoterDOTest.Equals01(VoterDO, Object)
        }
        [PexMethod]
        public VoterDO Constructor01()
        {
            VoterDO target = new VoterDO();
            return target;
            // TODO: add assertions to method VoterDOTest.Constructor01()
        }
        [PexMethod]
        public VoterDO Constructor(
            uint? pollingStationId,
            uint? cpr,
            string name,
            string address,
            string city,
            bool? cardPrinted,
            bool? voted
        )
        {
            VoterDO target = new VoterDO(pollingStationId, cpr, name, address, city, cardPrinted, voted);
            return target;
            // TODO: add assertions to method VoterDOTest.Constructor(Nullable`1<UInt32>, Nullable`1<UInt32>, String, String, String, Nullable`1<Boolean>, Nullable`1<Boolean>)
        }
    }
}
