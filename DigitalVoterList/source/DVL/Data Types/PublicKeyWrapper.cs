#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="PublicKeyWrapper.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Data_Types {
  using System;
  using System.Diagnostics.Contracts;

  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Util;

  /// <summary>
  /// The public key wrapper.
  /// </summary>
  [Serializable] public class PublicKeyWrapper {
    #region Fields

    /// <summary>
    /// The _key bytes.
    /// </summary>
    private readonly byte[] _keyBytes;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PublicKeyWrapper"/> class. 
    /// May I have a new wrapper for my this public key?
    /// </summary>
    /// <param name="crypto">
    /// The crypto service used.
    /// </param>
    /// <param name="physicalSecret">
    /// A password that obscures the public key.
    /// </param>
    public PublicKeyWrapper(ICrypto crypto, string physicalSecret) {
      Contract.Requires(crypto != null);
      Contract.Requires(physicalSecret != null);
      _keyBytes = Bytes.Obfuscate(crypto, 
        crypto.KeyPair.Public.ToBytes(), physicalSecret);
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// What is the key, when deobfuscated with this string?
    /// </summary>
    /// <param name="crypto">
    /// Cryptography service used for hashing.
    /// </param>
    /// <param name="physicalSecret">
    /// The string used to deobfuscate the key.
    /// </param>
    /// <returns>
    /// The original key, assuming the string was identical to the one used to obfuscate it.
    /// </returns>
    public AsymmetricKey GetKey(ICrypto crypto, string physicalSecret) {
      Contract.Requires(physicalSecret != null);
      return new AsymmetricKey(Bytes.Obfuscate(crypto, _keyBytes, physicalSecret).ToKey());
    }

    #endregion
  }
}
