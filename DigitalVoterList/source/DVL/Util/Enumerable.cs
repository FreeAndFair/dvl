#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Enumerable.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Util {
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// The enumerable.
  /// </summary>
  public static class Enumerable {
    #region Public Methods and Operators

    /// <summary>
    /// Perform this action for every item in the collection!
    /// </summary>
    /// <typeparam name="T">
    /// The type of the items the collection holds.
    /// </typeparam>
    /// <param name="self">
    /// The collection to perform the action on.
    /// </param>
    /// <param name="lambda">
    /// The action to perform.
    /// </param>
    public static void ForEach<T>(this IEnumerable<T> self, Action<T> lambda) {
      T[] collection = self.ToArray();
      foreach (T item in collection) lambda(item);
    }

    #endregion
  }
}
