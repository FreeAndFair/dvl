#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="IScanner.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Devices.Scanner {
  using Aegis_DVL.Data_Types;

  /// <summary>
  /// A scanner can read a physical voter card and extract required 
  /// information from it.
  /// </summary>
  public interface IScanner {
    #region Public Methods and Operators

    /// <summary>
    /// What is the voter number from this voter card?
    /// </summary>
    /// <returns>the voternumber</returns>
    VoterNumber Scan();

    #endregion
  }
}
