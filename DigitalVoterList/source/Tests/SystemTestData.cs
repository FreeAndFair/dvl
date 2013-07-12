// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SystemTestData.cs" company="">
//   
// </copyright>
// <summary>
//   A static class holding data relevant to multiple subsystem test suites.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="SystemTestData.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  /// <summary>
  ///   A static class holding data relevant to multiple subsystem test suites.
  /// </summary>
  public static class SystemTestData {
    #region Static Fields

    /// <summary>
    ///   What is the relative path to the public key used to encrypt data in the
    ///   test database?
    /// </summary>
    internal static string Key = "../../data/ElectionPublicKey.key";

    /// <summary>
    ///   What is the suffix of the name of the Log test DB?
    /// </summary>
    internal static string LogTestDb = "Log.sqlite";

    /// <summary>
    ///   What port should the test Manager use?
    /// </summary>
    internal static int ManagerPort = 62001;

    /// <summary>
    ///   What is the test database password?
    /// </summary>
    internal static string Password = "foobar";

    /// <summary>
    ///   What port should the test Station use?
    /// </summary>
    internal static int StationPort = ManagerPort + 1;

    /// <summary>
    ///   What is the suffix of the name of the primary Station's test DB?
    /// </summary>
    internal static string StationTestDb = "StationVoters.sqlite";

    /// <summary>
    ///   What port should the test peer Station use?
    /// </summary>
    internal static int PeerPort = StationPort + 1;

    /// <summary>
    ///   What is the suffix of the name of the secondary Station's test DB?
    /// </summary>
    internal static string PeerTestDb = "NewPeerVoters.sqlite";

    #endregion
  }
}
