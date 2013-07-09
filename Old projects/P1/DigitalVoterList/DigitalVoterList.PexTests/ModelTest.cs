// <copyright file="ModelTest.cs">Copyright ©  2011</copyright>
///Author: Claes Martinsen (but mostly pex)

namespace DigitalVoterList.PexTests
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using DBComm.DBComm.DAO;

    using global::PollingTable.PollingTable;

    [TestClass]
    [PexClass(typeof(Model))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ModelTest
    {
        [PexMethod]
        public void WriteToConfig([PexAssumeUnderTest]Model target)
        {
            target.WriteToConfig();
            // TODO: add assertions to method ModelTest.WriteToConfig(Model)
        }
        [PexMethod]
        public void UnregisterCurrentVoter([PexAssumeUnderTest]Model target)
        {
            target.UnregisterCurrentVoter();
            // TODO: add assertions to method ModelTest.UnregisterCurrentVoter(Model)
        }
        [PexMethod]
        public void StaticPvdaoSet(PessimisticVoterDAO value)
        {
            Model.StaticPvdao = value;
            // TODO: add assertions to method ModelTest.StaticPvdaoSet(PessimisticVoterDAO)
        }
        [PexMethod]
        public PessimisticVoterDAO StaticPvdaoGet()
        {
            PessimisticVoterDAO result = Model.StaticPvdao;
            return result;
            // TODO: add assertions to method ModelTest.StaticPvdaoGet()
        }
        [PexMethod]
        public void SetupInfoSet([PexAssumeUnderTest]Model target, SetupInfo value)
        {
            target.SetupInfo = value;
            // TODO: add assertions to method ModelTest.SetupInfoSet(Model, SetupInfo)
        }
        [PexMethod]
        public SetupInfo SetupInfoGet([PexAssumeUnderTest]Model target)
        {
            SetupInfo result = target.SetupInfo;
            return result;
            // TODO: add assertions to method ModelTest.SetupInfoGet(Model)
        }
        [PexMethod]
        public void RegisterCurrentVoter([PexAssumeUnderTest]Model target)
        {
            target.RegisterCurrentVoter();
            // TODO: add assertions to method ModelTest.RegisterCurrentVoter(Model)
        }
        [PexMethod]
        public void initializeStaticDAO([PexAssumeUnderTest]Model target)
        {
            target.initializeStaticDAO();
            // TODO: add assertions to method ModelTest.initializeStaticDAO(Model)
        }
        [PexMethod]
        public bool CprLetterVal(string cpr)
        {
            bool result = Model.CprLetterVal(cpr);
            return result;
            // TODO: add assertions to method ModelTest.CprLetterVal(String)
        }
        [PexMethod]
        public bool CprLengtVal(uint cpr)
        {
            bool result = Model.CprLengtVal(cpr);
            return result;
            // TODO: add assertions to method ModelTest.CprLengtVal(UInt32)
        }
        [PexMethod]
        public Model Constructor()
        {
            Model target = new Model();
            return target;
            // TODO: add assertions to method ModelTest.Constructor()
        }
        [PexMethod]
        public void AdminPassSet([PexAssumeUnderTest]Model target, string value)
        {
            target.AdminPass = value;
            // TODO: add assertions to method ModelTest.AdminPassSet(Model, String)
        }
        [PexMethod]
        public string AdminPassGet([PexAssumeUnderTest]Model target)
        {
            string result = target.AdminPass;
            return result;
            // TODO: add assertions to method ModelTest.AdminPassGet(Model)
        }
    }
}
