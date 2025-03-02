// <copyright file="ModelTest.initializeStaticDAO.g.cs">Copyright �  2011</copyright>
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

namespace DigitalVoterList.PexTests
{
    using Microsoft.Pex.Framework.Generated;

    using DBComm.DBComm.DO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using global::PollingTable.PollingTable;

    public partial class ModelTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ModelTest))]
[Ignore]
[PexDescription("the test state was: duplicate path")]
public void initializeStaticDAOThrowsMySqlException292()
{
    using (PexTraceListenerContext.Create())
    {
      Model model;
      model = new Model();
      model.currentVoter = (VoterDO)null;
      model.AdminPass = (string)null;
      this.initializeStaticDAO(model);
    }
}
[TestMethod]
[PexGeneratedBy(typeof(ModelTest))]
[Ignore]
[PexDescription("the test state was: path bounds exceeded")]
public void initializeStaticDAO992()
{
    Model model;
    model = new Model();
    model.currentVoter = (VoterDO)null;
    model.AdminPass = (string)null;
    this.initializeStaticDAO(model);
}
    }
}
