// <copyright file="ModelTest.CprLetterVal.g.cs">Copyright �  2011</copyright>
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
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Microsoft.Pex.Framework.Generated;

    public partial class ModelTest
    {
[TestMethod]
[PexGeneratedBy(typeof(ModelTest))]
public void CprLetterVal146()
{
    bool b;
    b = this.CprLetterVal("2");
    Assert.AreEqual<bool>(true, b);
}
[TestMethod]
[PexGeneratedBy(typeof(ModelTest))]
public void CprLetterVal719()
{
    bool b;
    b = this.CprLetterVal((string)null);
    Assert.AreEqual<bool>(false, b);
}
    }
}
