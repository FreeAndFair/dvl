#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Crypto.cs" company="DemTech">
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
  using System.Security.Cryptography;

  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Engines;
  using Org.BouncyCastle.Crypto.Generators;
  using Org.BouncyCastle.Crypto.Modes;
  using Org.BouncyCastle.Crypto.Paddings;
  using Org.BouncyCastle.Crypto.Parameters;
  using Org.BouncyCastle.Security;

  /// <summary>
  /// The crypto.
  /// </summary>
  public class Crypto : ICrypto {
    #region Fields

    /// <summary>
    /// The _asym engine.
    /// </summary>
    private readonly RsaEngine _asymEngine;

    /// <summary>
    /// The _hasher.
    /// </summary>
    private readonly SHA256Managed _hasher;

    /// <summary>
    /// The _random.
    /// </summary>
    private readonly SecureRandom _random;

    /// <summary>
    /// The _sym engine.
    /// </summary>
    private readonly PaddedBufferedBlockCipher _symEngine;

    /// <summary>
    /// The _is disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// The _iv.
    /// </summary>
    private byte[] _iv;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Crypto"/> class. 
    /// May I have a new Crypto where the VoterDataEncryptionKey is set to this?
    /// </summary>
    /// <param name="encryptionAsymmetricKey">
    /// The VoterDataEncryptionKey.
    /// </param>
    public Crypto(AsymmetricKey encryptionAsymmetricKey)
      : this() {
      Contract.Ensures(this.VoterDataEncryptionKey.Equals(encryptionAsymmetricKey));
      this.VoterDataEncryptionKey = encryptionAsymmetricKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Crypto"/> class. 
    ///   May I have a new Crypto?
    /// </summary>
    public Crypto() {
      this._random = new SecureRandom();

      // RSA initialization
      this._asymEngine = new RsaEngine();
      var rsaKeyPairGnr = new RsaKeyPairGenerator();

      // TODO: It should be 3072 instead of 128 in order to improve safety
      rsaKeyPairGnr.Init(new KeyGenerationParameters(new SecureRandom(), 1028));
      AsymmetricCipherKeyPair keys = rsaKeyPairGnr.GenerateKeyPair();

      // AES initialization
      // TODO: Should probably use CCM-mode instead of CBC (since bouncy 
      // doesn't have CWC..), but we couldn't get it to work because 
      // we're stupid as hell.
      this._symEngine = new PaddedBufferedBlockCipher(
        new CbcBlockCipher(new AesEngine()));
      this.NewIv();

      // SHA256 initilization
      this._hasher = new SHA256Managed();

      this.Keys = new Tuple<AsymmetricKey, AsymmetricKey>(
        new AsymmetricKey(keys.Public), new AsymmetricKey(keys.Private));
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Crypto"/> class. 
    /// </summary>
    ~Crypto() { this.Dispose(false); }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the iv.
    /// </summary>
    public byte[] Iv { get { return this._iv; } set { this._iv = value; } }

    /// <summary>
    /// Gets the keys.
    /// </summary>
    public Tuple<AsymmetricKey, AsymmetricKey> Keys { get; private set; }

    /// <summary>
    /// Gets or sets the voter data encryption key.
    /// </summary>
    [Pure] public AsymmetricKey VoterDataEncryptionKey { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   May I have a new randomly generated human-readable password?
    /// </summary>
    /// <returns>A randomly generated password.</returns>
    [Pure] public static string GeneratePassword() {
      Contract.Ensures(
        Contract.Result<string>() != null &&
        Contract.Result<string>().Length > 0);
      var random = new SecureRandom();
      var bytes = new byte[128];
      random.NextBytes(bytes);

      var badChars =
        new[] { 'L', 'l', 'i', 'I', '1', '5', 'S', 'Z', '2', 'o', '0', 'O' };
      var goodChars =
        new[] {
          'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 
          'N', 'P', 'Q', 'R', 'T', 'U', 'V', 'W', 'X', 'Y'
        };
      var pwd = new string(bytes.AsBase64().Take(10).ToArray());
      foreach (char c in pwd) {
        if (!badChars.Contains(c)) continue;
        var rdm = new Random();
        int number = rdm.Next(20);
        pwd = pwd.Replace(c, goodChars[number]);
      }

      return pwd;
    }

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
      this._asymEngine.Init(false, asymmetricKey.Value);
      return this._asymEngine.ProcessBlock(cipher, 0, cipher.Value.Length).Skip(1).ToArray();
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
      this._asymEngine.Init(true, asymmetricKey.Value);
      return new CipherText(
        this._asymEngine.ProcessBlock(
          new byte[] { 1 }.Merge(bytes), 0, bytes.Length + 1));
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() {
      if (!this._isDisposed) this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    /// The generate symmetric key.
    /// </summary>
    /// <returns>
    /// The <see cref="byte[]"/>.
    /// </returns>
    public byte[] GenerateSymmetricKey() {
      var key = new byte[32];
      this._random.NextBytes(key);
      return key;
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
    public byte[] Hash(byte[] bytes) { return this._hasher.ComputeHash(bytes); }

    /// <summary>
    /// The new iv.
    /// </summary>
    public void NewIv() {
      byte[] oldIv = this._iv;
      do {
        this._iv = new byte[this._symEngine.GetBlockSize()];
        this._random.NextBytes(this.Iv);
        oldIv = oldIv ?? this._iv;
      }
 while (oldIv.SequenceEqual(this._iv));
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
      this._symEngine.Init(
        false, 
        new ParametersWithIV(
          new KeyParameter(symmetricKey), this.Iv));
      return this._symEngine.DoFinal(cipher);
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
      this._symEngine.Init(
        true, 
        new ParametersWithIV(
          new KeyParameter(symmetricKey), this.Iv));
      return new CipherText(this._symEngine.DoFinal(bytes));
    }

    #endregion

    #region Methods

    /// <summary>
    /// The dispose.
    /// </summary>
    /// <param name="disposing">
    /// The disposing.
    /// </param>
    private void Dispose(bool disposing) {
      this._isDisposed = true;
      if (disposing) this._hasher.Dispose();
    }

    #endregion
  }
}
