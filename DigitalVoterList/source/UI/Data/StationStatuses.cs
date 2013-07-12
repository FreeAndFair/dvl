#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StationStatuses.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace UI.Data {
  using System.Collections.Generic;

  /// <summary>
  /// The station statuses.
  /// </summary>
  public class StationStatuses : List<StationStatus> {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StationStatuses"/> class.
    /// </summary>
    public StationStatuses() { this.AddRange(new DesignTimeStationStatuses()); }

    #endregion
  }
}
