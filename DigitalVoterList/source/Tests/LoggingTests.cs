#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="LoggingTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System;
  using System.IO;
  using System.Linq;

  using Aegis_DVL;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  /// <summary>
  /// The logging tests.
  /// </summary>
  [TestFixture] public class LoggingTests {
    #region Public Properties

    /// <summary>
    /// Gets the station.
    /// </summary>
    public Station Station { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The set up.
    /// </summary>
    [SetUp] public void SetUp() {
      this.Station = new Station(
        new TestUi(), SystemTestData.Key, SystemTestData.Password, SystemTestData.StationPort, 
          "LoggingTestsVoters.sqlite", "LoggingTestsLog.sqlite");
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      this.Station.Dispose();
      this.Station = null;
      File.Delete("LoggingTestsVoters.sqlite");
      File.Delete("LoggingTestsLog.sqlite");
    }

    /// <summary>
    /// The test.
    /// </summary>
    [Test] public void Test() {
      Assert.That(
        !this.Station.Logger.Export.Any(
          entry => entry.Message.ToString() == "Testing testing" && entry.Level == Level.Info));
      this.Station.Logger.Log("Testing testing", Level.Info);
      Assert.That(
        this.Station.Logger.Export.Any(
          entry => entry.Message.ToString() == "Testing testing" && entry.Level == Level.Info));
      this.Station.Logger.Export.ForEach(logentry => Console.WriteLine(logentry));
    }

    #endregion
  }
}
