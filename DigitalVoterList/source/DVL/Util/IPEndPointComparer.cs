#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="IPEndPointComparer.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Util {
  using System.Collections.Generic;
  using System.Net;

  /// <summary>
  ///   A IPEndPointComparer is used to compare two IPEndPoints with each other.
  /// </summary>
  public class IPEndPointComparer : IComparer<IPEndPoint> {
    #region Public Methods and Operators

    /// <summary>
    /// The compare.
    /// </summary>
    /// <param name="x">
    /// The x.
    /// </param>
    /// <param name="y">
    /// The y.
    /// </param>
    /// <returns>
    /// The <see cref="int"/>.
    /// </returns>
    public int Compare(IPEndPoint x, IPEndPoint y) { return string.CompareOrdinal(x.ToString(), y.ToString()); }

    #endregion
  }
}
