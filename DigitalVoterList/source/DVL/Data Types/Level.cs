#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Level.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Data_Types {
  using System;

  /// <summary>
  /// The level.
  /// </summary>
  [Serializable] public enum Level {
    /// <summary>
    /// The debug.
    /// </summary>
    Debug, 

    /// <summary>
    /// The info.
    /// </summary>
    Info, 

    /// <summary>
    /// The warn.
    /// </summary>
    Warn, 

    /// <summary>
    /// The error.
    /// </summary>
    Error, 

    /// <summary>
    /// The fatal.
    /// </summary>
    Fatal
  }
}
