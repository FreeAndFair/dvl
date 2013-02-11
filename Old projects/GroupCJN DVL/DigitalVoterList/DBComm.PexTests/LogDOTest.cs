// <copyright file="LogDOTest.cs">Copyright ©  2011</copyright>

namespace DBComm.PexTests
{
    using System;

    using Microsoft.Pex.Framework;
    using Microsoft.Pex.Framework.Validation;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::DBComm.DBComm.DO;

    [TestClass]
    [PexClass(typeof(LogDO))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class LogDOTest
    {
        [PexMethod]
        public LogDO Constructor(
            uint? table,
            uint? cpr,
            ActivityEnum? activity
        )
        {
            LogDO target = new LogDO(table, cpr, activity);
            return target;
            // TODO: add assertions to method LogDOTest.Constructor(Nullable`1<UInt32>, Nullable`1<UInt32>, Nullable`1<ActivityEnum>)
        }
    }
}
