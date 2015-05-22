#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StationStatus.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.Data {
  using System.Collections.Generic;

  /// <summary>
  /// The station status.
  /// </summary>
  public class StationStatus {
    #region Public Properties

    /// <summary>
    /// Gets or sets a value indicating whether connected.
    /// </summary>
    public string ConnectionState { get; set; }

    /// <summary>
    /// Gets or sets the ip adress.
    /// </summary>
    public string IpAddress { get; set; }

    #endregion

    public bool Connected() {
      return !ConnectionState.Equals("Not Connected");
    }

    public bool Ready() {
      return ConnectionState.Equals("Ready");
    }

  }
}
