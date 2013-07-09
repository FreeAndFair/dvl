// <copyright file="PollingStationDOTest.GetHashCode01.g.cs">Copyright �  2011</copyright>
// <auto-generated>
// This file contains automatically generated unit tests.
// Do NOT modify this file manually.
// 
// When Pex is invoked again,
// it might remove or update any previously generated unit tests.
// 
// If the contents of this file becomes outdated, e.g. if it does not
// compile anymore, you may delete this file and invoke Pex again.
// </auto-generated>

namespace DBComm.PexTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Pex.Framework.Generated;

    using global::DBComm.DBComm.DO;

    public partial class PollingStationDOTest
    {
[TestMethod]
[PexGeneratedBy(typeof(PollingStationDOTest))]
public void GetHashCode01420()
{
    PollingStationDO pollingStationDO;
    int i;
    pollingStationDO =
      new PollingStationDO(default(uint?), default(uint?), "", (string)null);
    i = this.GetHashCode01(pollingStationDO);
    Assert.AreEqual<int>(757602046, i);
    Assert.IsNotNull((object)pollingStationDO);
    Assert.IsNull((object)(pollingStationDO.PrimaryKey));
    Assert.IsNull((object)(pollingStationDO.MunicipalityId));
    Assert.AreEqual<string>("", pollingStationDO.Name);
    Assert.AreEqual<string>((string)null, pollingStationDO.Address);
    Assert.IsNotNull(pollingStationDO.Voters);
}
[TestMethod]
[PexGeneratedBy(typeof(PollingStationDOTest))]
public void GetHashCode01402()
{
    PollingStationDO pollingStationDO;
    int i;
    pollingStationDO = new PollingStationDO
                           (default(uint?), default(uint?), (string)null, (string)null);
    i = this.GetHashCode01(pollingStationDO);
    Assert.AreEqual<int>(0, i);
    Assert.IsNotNull((object)pollingStationDO);
    Assert.IsNull((object)(pollingStationDO.PrimaryKey));
    Assert.IsNull((object)(pollingStationDO.MunicipalityId));
    Assert.AreEqual<string>((string)null, pollingStationDO.Name);
    Assert.AreEqual<string>((string)null, pollingStationDO.Address);
    Assert.IsNotNull(pollingStationDO.Voters);
}
    }
}