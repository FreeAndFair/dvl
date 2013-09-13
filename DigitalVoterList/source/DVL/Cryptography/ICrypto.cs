#region Copyright and License
// // -----------------------------------------------------------------------
// // <copyright file="ICrypto.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Cryptography {
  using System;
  using System.Diagnostics.Contracts;

  using Aegis_DVL.Data_Types;

  using Org.BouncyCastle.Crypto;

  /// <summary>
  /// ICrypto is the interface to all cryptographic features of the DVL system. 
  /// </summary>
  [ContractClass(typeof(CryptoContract))] public interface ICrypto {
    #region Public Properties

    /// <summary>
    /// Get and set the initialization vector.
    /// </summary>
    byte[] Iv { get; set; }

    /// <summary>
    /// What is our keypair for this election?
    /// </summary>
    /// TODO: The private key should not be here, in general.
    AsymmetricCipherKeyPair KeyPair { get; }

    /// <summary>
    /// What is the asymmetric key used for encrypting voterdata at 
    /// this election venue?
    /// </summary>
    [Pure] AsymmetricKey VoterDataEncryptionKey { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// What does this look like when it's asymmetrically decrypted with this key?
    /// </summary>
    /// <param name="cipher">
    /// the ciphertext to be decrypted
    /// </param>
    /// <param name="asymmetricKey">
    /// the AsymmetricKey to decrypt with
    /// </param>
    /// <returns>
    /// the decrypted ciphertext as a byte array
    /// </returns>
    [Pure] byte[] AsymmetricDecrypt(CipherText cipher, AsymmetricKey asymmetricKey);

    /// <summary>
    /// What does this look like when it's asymmetrically encrypted with this key?
    /// </summary>
    /// <param name="bytes">
    /// the bytes to be encrypted
    /// </param>
    /// <param name="asymmetricKey">
    /// the AsymmetricKey to ecnrypt with
    /// </param>
    /// <returns>
    /// the encrypted byte array as a ciphertext
    /// </returns>
    [Pure] CipherText AsymmetricEncrypt(byte[] bytes, AsymmetricKey asymmetricKey);

    /// <returns>
    /// A new randomly generated symmetric key?
    /// </returns>
    [Pure] byte[] GenerateSymmetricKey();

    /// <summary>
    /// What is the hashed value of this?
    /// </summary>
    /// <param name="bytes">
    /// The bytes to be hashed.
    /// </param>
    /// <returns>
    /// The hashed bytes.
    /// </returns>
    [Pure] byte[] Hash(byte[] bytes);

    /// <summary>
    /// What does this look like when it's symmetrically decrypted with this key?
    /// </summary>
    /// <param name="cipher">
    /// the ciphertext to be decrypted
    /// </param>
    /// <param name="symmetricKey">
    /// the AsymmetricKey to decrypt with
    /// </param>
    /// <returns>
    /// the decrypted ciphertext as a byte array
    /// </returns>
    [Pure] byte[] SymmetricDecrypt(CipherText cipher, SymmetricKey symmetricKey);

    /// <summary>
    /// What does this look like when it's symmetrically encrypted with this key?
    /// </summary>
    /// <param name="bytes">
    /// the bytes to be encrypted
    /// </param>
    /// <param name="symmetricKey">
    /// the AsymmetricKey to ecnrypt with
    /// </param>
    /// <returns>
    /// the encrypted byte array as a ciphertext
    /// </returns>
    [Pure] CipherText SymmetricEncrypt(byte[] bytes, SymmetricKey symmetricKey);

    /// <summary>
    /// Generate a new initialization vector.
    /// </summary>
    void NewIv();

    #endregion
  }

  /// <summary>
  /// The crypto contract.
  /// </summary>
  [ContractClassFor(typeof(ICrypto))] public abstract class CryptoContract : ICrypto {
    #region Public Properties

    /// <summary>
    /// Initialization vectors are of positive length.
    /// </summary>
    public byte[] Iv { 
      get { return default(byte[]); }
      set { }
    }

    /// <summary>
    /// Gets the keys.
    /// </summary>
    /// TODO Write a contract for KeyPair that includes invariants about non-nullness
    /// of Private and Public.
    public AsymmetricCipherKeyPair KeyPair {
      get { return default(AsymmetricCipherKeyPair); }
    }

    /// <summary>
    /// Gets or sets the voter data encryption key.
    /// </summary>
    public AsymmetricKey VoterDataEncryptionKey {
      get { return default(AsymmetricKey); } set { }
    }

    #endregion

    #region Public Methods and Operators

    public byte[] AsymmetricDecrypt(CipherText cipher, AsymmetricKey asymmetricKey) {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    public CipherText AsymmetricEncrypt(byte[] bytes, AsymmetricKey asymmetricKey) {
      Contract.Requires(bytes != null);
      return default(CipherText);
    }

    public void Dispose() { }

    public byte[] GenerateSymmetricKey() {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    public byte[] Hash(byte[] bytes) {
      Contract.Requires(bytes != null);
      return default(byte[]);
    }

    public byte[] SymmetricDecrypt(CipherText cipher, SymmetricKey symmetricKey) {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    public CipherText SymmetricEncrypt(byte[] bytes, SymmetricKey symmetricKey) {
      Contract.Requires(bytes != null);
      return default(CipherText);
    }

    public void NewIv() {
      Contract.Ensures(!this.Iv.Equals(Contract.OldValue(this.Iv)));
    }

    #endregion

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(this.Iv != null);
      Contract.Invariant(0 < this.Iv.Length);
      Contract.Invariant(KeyPair != null);
    }
  }
}
