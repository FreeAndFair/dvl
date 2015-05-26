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
  using Aegis_DVL.Data_Types;

  /// <summary>
  /// The design time station statuses.
  /// </summary>
  public class DesignTimeStationStatuses : List<StationStatus> {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignTimeStationStatuses"/> class.
    /// </summary>
    public DesignTimeStationStatuses() {
      this.Add(new StationStatus("127.0.0.1", "Ready"));
      this.Add(new StationStatus("127.0.0.2", "Ready"));
      this.Add(new StationStatus("127.0.0.3", "Not Connected"));
      this.Add(new StationStatus("127.0.0.4", "Not Connected"));
    }

    #endregion
  }
}
