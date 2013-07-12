#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="DesignTimeStationStatuses.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.Data {
  using System.Collections.Generic;

  /// <summary>
  /// The design time station statuses.
  /// </summary>
  public class DesignTimeStationStatuses : List<StationStatus> {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignTimeStationStatuses"/> class.
    /// </summary>
    public DesignTimeStationStatuses() {
      this.Add(new StationStatus { IpAdress = "127.0.0.1", Connected = true });
      this.Add(new StationStatus { IpAdress = "127.0.0.2", Connected = true });
      this.Add(new StationStatus { IpAdress = "127.0.0.3", Connected = false });
      this.Add(new StationStatus { IpAdress = "127.0.0.4", Connected = false });
    }

    #endregion
  }
}
