#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StationStatus.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Data_Types {
  using System.Collections.Generic;
  using System.Net;

  /// <summary>
  /// The station status.
  /// </summary>
  public class StationStatus {
    #region Public Properties

    public IPEndPoint Address { get; private set;  }

    /// <summary>
    /// Gets or sets a value indicating whether connected.
    /// </summary>
    public string ConnectionState { get; set; }

    /// <summary>
    /// Gets or sets the ip adress.
    /// </summary>
    public string DisplayName { get; private set; }

    #endregion

    public StationStatus(IPEndPoint address, string displayName, string status) {
      Address = address;
      DisplayName = displayName;
      ConnectionState = status;
    }

    public bool Connected() {
      return !ConnectionState.Equals("Not Connected");
    }
  }
}
