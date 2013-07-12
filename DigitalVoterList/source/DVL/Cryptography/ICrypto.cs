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
  using System.Linq;

  using Aegis_DVL.Data_Types;

  /// <summary>
  ///   Crypto is responsible for cryptographic functions such as 
  /// public-key encryption.
  /// </summary>
  [ContractClass(typeof(CryptoContract))] public interface ICrypto : IDisposable {
    #region Public Properties

    /// <summary>
    ///   What is the current initilization vector?
    ///   The initialization vector is the following!
    /// </summary>
    byte[] Iv { [Pure] get; set; }

    /// <summary>
    ///   What are the keys for my public key infrastructure?
    ///   Item1 = Public key
    ///   Item2 = Private key
    /// </summary>
    Tuple<AsymmetricKey, AsymmetricKey> Keys { get; }

    /// <summary>
    ///   What is the asymmetric key used for encrypting voterdata at 
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

    /// <summary>
    /// May I have a new randomly generated symmetric key?
    /// </summary>
    /// <returns>
    /// The <see cref="byte[]"/>.
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
    ///   Generate a new initialization vector to be used for symmetric encryption!
    /// </summary>
    void NewIv();

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

    #endregion
  }

  /// <summary>
  /// The crypto contract.
  /// </summary>
  [ContractClassFor(typeof(ICrypto))] public abstract class CryptoContract : ICrypto {
    #region Public Properties

    /// <summary>
    /// Gets or sets the iv.
    /// </summary>
    public byte[] Iv {
      get {
        Contract.Ensures(Contract.Result<byte[]>() != null);
        return default(byte[]);
      }

      set {
        Contract.Requires(value != null);
        Contract.Ensures(this.Iv.SequenceEqual(value));
      }
    }

    /// <summary>
    /// Gets the keys.
    /// </summary>
    public Tuple<AsymmetricKey, AsymmetricKey> Keys {
      get {
        var res = Contract.Result<Tuple<AsymmetricKey, AsymmetricKey>>();
        Contract.Ensures(res != null && !Equals(res.Item1, null) && !Equals(res.Item2, null));
        return default(Tuple<AsymmetricKey, AsymmetricKey>);
      }
    }

    /// <summary>
    /// Gets or sets the voter data encryption key.
    /// </summary>
    public AsymmetricKey VoterDataEncryptionKey { get { return default(AsymmetricKey); } set { } }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The asymmetric decrypt.
    /// </summary>
    /// <param name="cipher">
    /// The cipher.
    /// </param>
    /// <param name="asymmetricKey">
    /// The asymmetric key.
    /// </param>
    /// <returns>
    /// The <see cref="byte[]"/>.
    /// </returns>
    public byte[] AsymmetricDecrypt(CipherText cipher, AsymmetricKey asymmetricKey) {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    /// <summary>
    /// The asymmetric encrypt.
    /// </summary>
    /// <param name="bytes">
    /// The bytes.
    /// </param>
    /// <param name="asymmetricKey">
    /// The asymmetric key.
    /// </param>
    /// <returns>
    /// The <see cref="CipherText"/>.
    /// </returns>
    public CipherText AsymmetricEncrypt(byte[] bytes, AsymmetricKey asymmetricKey) {
      Contract.Requires(bytes != null);
      return default(CipherText);
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// The generate symmetric key.
    /// </summary>
    /// <returns>
    /// The <see cref="byte[]"/>.
    /// </returns>
    public byte[] GenerateSymmetricKey() {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    /// <summary>
    /// The hash.
    /// </summary>
    /// <param name="bytes">
    /// The bytes.
    /// </param>
    /// <returns>
    /// The <see cref="byte[]"/>.
    /// </returns>
    public byte[] Hash(byte[] bytes) {
      Contract.Requires(bytes != null);
      return default(byte[]);
    }

    /// <summary>
    /// The new iv.
    /// </summary>
    public void NewIv() {
      Contract.Ensures(this.Iv != null);
      Contract.Ensures(
        Contract.OldValue(this.Iv) == null ||
        Contract.OldValue(this.Iv) != this.Iv);
    }

    /// <summary>
    /// The symmetric decrypt.
    /// </summary>
    /// <param name="cipher">
    /// The cipher.
    /// </param>
    /// <param name="symmetricKey">
    /// The symmetric key.
    /// </param>
    /// <returns>
    /// The <see cref="byte[]"/>.
    /// </returns>
    public byte[] SymmetricDecrypt(CipherText cipher, SymmetricKey symmetricKey) {
      Contract.Ensures(Contract.Result<byte[]>() != null);
      return default(byte[]);
    }

    /// <summary>
    /// The symmetric encrypt.
    /// </summary>
    /// <param name="bytes">
    /// The bytes.
    /// </param>
    /// <param name="symmetricKey">
    /// The symmetric key.
    /// </param>
    /// <returns>
    /// The <see cref="CipherText"/>.
    /// </returns>
    public CipherText SymmetricEncrypt(byte[] bytes, SymmetricKey symmetricKey) {
      Contract.Requires(bytes != null);
      return default(CipherText);
    }

    #endregion
  }
}
