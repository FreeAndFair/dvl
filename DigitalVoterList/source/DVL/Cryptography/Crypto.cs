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

  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using Org.BouncyCastle.Crypto;
  using Org.BouncyCastle.Crypto.Engines;
  using Org.BouncyCastle.Crypto.Digests;
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
    // Our asymmetric cipher (currently ElGamal).
    private readonly IBufferedCipher _aCipher;
    // Our hasher (currently SHA256).
    private readonly IDigest _hasher;
    // Our pseudo-random number generator.
    private readonly SecureRandom _random;
    // Our symmetric cipher (currently AES).
    private readonly IBufferedCipher _cipher;
    // Have we been disposed?
    private bool _isDisposed;
    // Our initialization vector.
    // TODO Can/should we make the IV readonly?
    private byte[] _iv;
    // Our key pair generator (for ElGamal).
    private readonly IAsymmetricCipherKeyPairGenerator _keyGen;
    // Our key pair (for ElGamal).
    private readonly AsymmetricCipherKeyPair _keys;
    #endregion

    #region Constructors and Destructors

    /// <summary> 
    /// May I have a new Crypto for this VoterDataEncryptionKey?
    /// </summary>
    public Crypto(AsymmetricKey encryptionAsymmetricKey) : this() {
      Contract.Ensures(VoterDataEncryptionKey.Equals(encryptionAsymmetricKey));
      this.VoterDataEncryptionKey = encryptionAsymmetricKey;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Crypto"/> class. 
    ///   May I have a new Crypto?
    /// </summary>
    public Crypto() {
      _random = new SecureRandom();

      _keyGen = GeneratorUtilities.GetKeyPairGenerator("ElGamal");
      var parameterGenerator = new ElGamalParametersGenerator();
      parameterGenerator.Init(512, 10, _random);
      var parameters = parameterGenerator.GenerateParameters();
      _keyGen.Init(new ElGamalKeyGenerationParameters(_random, parameters));
      _keys = _keyGen.GenerateKeyPair();
      _aCipher = CipherUtilities.GetCipher("ElGamal/NONE/PKCS1Padding");

      _cipher = CipherUtilities.GetCipher("AES/CBC/PKCS7Padding");
      //new PaddedBufferedBlockCipher(new CbcBlockCipher(new AesEngine()));

      Iv = new byte[_cipher.GetBlockSize()];
      _random.NextBytes(this.Iv);

      _hasher = new Sha256Digest();

      KeyPair = new AsymmetricCipherKeyPair(
        new AsymmetricKey(_keys.Public), new AsymmetricKey(_keys.Private));
    }

    #endregion

    #region Public Properties

    public byte[] Iv { get { return _iv; } set { _iv = value; } }

    public AsymmetricCipherKeyPair KeyPair { get; private set; }

    [Pure] public AsymmetricKey VoterDataEncryptionKey { get; set; }

    #endregion

    #region Public Methods and Operators

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
    // TODO refactor block processing
    public byte[] AsymmetricDecrypt(CipherText cipher, 
                                    AsymmetricKey asymmetricKey) {
      //Contract.Requires(cipher.Value.Length <= _aCipher.GetBlockSize());
      _aCipher.Init(false, asymmetricKey.Value);
      byte[] plainText = new byte[_aCipher.GetOutputSize(cipher.Value.Length)];
      /*
      int blockSize = _aCipher.GetBlockSize();
      int bytesProcessed = 0;
      byte[] input = cipher.Value;

      if (input.Length < blockSize) {
        bytesProcessed = _aCipher.ProcessBytes(input, plainText, 0);
      } else {
        for (int i = 0; i < input.Length / blockSize; i++) {
          int offset = i * blockSize;
          int length = Math.Min(blockSize, input.Length - offset);
          bytesProcessed +=
            _aCipher.ProcessBytes(
                                  input,
                                  offset,
                                  length,
                                  plainText,
                                  bytesProcessed);
        }
      }
      bytesProcessed += _aCipher.DoFinal(input, plainText, bytesProcessed);
       * */
      int outputLength = 
        _aCipher.ProcessBytes(cipher.Value, 0, cipher.Value.Length, plainText, 0);
      outputLength += _aCipher.DoFinal(plainText, outputLength);
      byte[] result = new byte[outputLength];
      Array.Copy(plainText, result, outputLength);
      return new CipherText(result);
    }

    public CipherText AsymmetricEncrypt(byte[] input, AsymmetricKey asymmetricKey) {
      //Contract.Requires(input.Length <= _aCipher.GetBlockSize());
      _aCipher.Init(true, asymmetricKey.Value);
      byte[] cipherText = new byte[_aCipher.GetOutputSize(input.Length)];
      /*
      int blockSize = _aCipher.GetBlockSize();
      int bytesProcessed = 0;

      if (input.Length < blockSize) {
          bytesProcessed = _aCipher.ProcessBytes(input, cipherText, 0);
      } else {
        for (int i = 0; i < input.Length / blockSize; i++) {
          int offset = i * blockSize;
          int length = Math.Min(blockSize, input.Length - offset);
          bytesProcessed +=
            _aCipher.ProcessBytes(
                                  input,
                                  offset,
                                  length,
                                  cipherText,
                                  bytesProcessed);
        }
      }
      bytesProcessed += _aCipher.DoFinal(input, cipherText, bytesProcessed);
       * */
      int outputLength = 
        _aCipher.ProcessBytes(input, 0, input.Length, cipherText, 0);
      outputLength += _aCipher.DoFinal(cipherText, outputLength);
      byte[] result = new byte[outputLength];
      Array.Copy(cipherText, result, outputLength);
      return new CipherText(result);
    }

    public byte[] GenerateSymmetricKey() {
      var key = new byte[32];
      this._random.NextBytes(key);
      return key;
    }

    public byte[] Hash(byte[] bytes) {
      _hasher.Reset();
      _hasher.BlockUpdate(bytes, 0, bytes.Length);
      byte[] result = new byte[_hasher.GetDigestSize()];
      _hasher.DoFinal(result, 0);
      return result;
    }

    public byte[] SymmetricDecrypt(CipherText cipher, SymmetricKey symmetricKey) {
      this._cipher.Init(
        false, 
        new ParametersWithIV(
          new KeyParameter(symmetricKey), this.Iv));
      return this._cipher.DoFinal(cipher);
    }

    public CipherText SymmetricEncrypt(byte[] bytes, SymmetricKey symmetricKey) {
      this._cipher.Init(
        true, 
        new ParametersWithIV(
          new KeyParameter(symmetricKey), this.Iv));
      return new CipherText(this._cipher.DoFinal(bytes));
    }

    public void NewIv() {
      var oldIv = _iv;
      do {
        _iv = new byte[this._cipher.GetBlockSize()];
        _random.NextBytes(Iv);
        oldIv = oldIv ?? _iv;
      } while (oldIv.SequenceEqual(_iv));
    }

    #endregion

    #region Methods

    [ContractInvariantMethod]
    private void ObjectInvariant() {
      Contract.Invariant(_aCipher != null);
      Contract.Invariant(_cipher != null);
      Contract.Invariant(_hasher != null);
      Contract.Invariant(_iv != null);
      Contract.Invariant(0 < _iv.Length);
      Contract.Invariant(_keyGen != null);
      Contract.Invariant(_keys != null);
      Contract.Invariant(_random != null);
    }

    #endregion
  }
}
