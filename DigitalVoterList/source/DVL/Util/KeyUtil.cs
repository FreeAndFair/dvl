﻿#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="KeyUtil.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Util {
  using System.Diagnostics.Contracts;

  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Security;
  using Org.BouncyCastle.X509;

  /// <summary>
  /// The key util.
  /// </summary>
  public static class KeyUtil {
    #region Public Methods and Operators

    /// <summary>
    /// What does this asymmetric key look like as a byte array?
    /// </summary>
    /// <param name="key">
    /// The asymmetric key to transform into a byte array.
    /// </param>
    /// <returns>
    /// A byte array representation of the asymmetric key
    /// </returns>
    [Pure] public static byte[] ToBytes(this AsymmetricKeyParameter key) { return SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(key).GetDerEncoded(); }

    /// <summary>
    /// What does this byte array representation of an asymmetric key look like when transformed back into an asymmetric key?
    /// </summary>
    /// <param name="bytes">
    /// The byte array to transform.
    /// </param>
    /// <returns>
    /// The asymmetric key representation of the byte array.
    /// </returns>
    [Pure] public static AsymmetricKeyParameter ToKey(this byte[] bytes) { return PublicKeyFactory.CreateKey(bytes); }

    #endregion
  }
}
