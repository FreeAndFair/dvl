#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="CryptoTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System.IO;

  using Aegis_DVL;
  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  /// <summary>
  /// The crypto tests.
  /// </summary>
  [TestFixture] public class CryptoTests {
    #region Static Fields

    /// <summary>
    /// The key.
    /// </summary>
    public static string key = "../../data/ElectionPublicKey.key";

    #endregion

    #region Fields

    /// <summary>
    /// The _station.
    /// </summary>
    private Station _station;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the crypto.
    /// </summary>
    public ICrypto Crypto { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// </summary>
    [Test] public void AsymmetricTest() {
      // Encrypt/decrypt
      const string testString = "Howdy there, partner!";
      byte[] bytes = Bytes.From(testString);
      CipherText ciphertext = this.Crypto.AsymmetricEncrypt(bytes, this.Crypto.Keys.Item1);
      Assert.That(!bytes.IsIdenticalTo(ciphertext));
      byte[] decryptedBytes = this.Crypto.AsymmetricDecrypt(ciphertext, this.Crypto.Keys.Item2);
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));
      Assert.That(decryptedBytes.To<string>().Equals(testString));

      // Encrypt/decrypt using reversed keys
      ciphertext = this.Crypto.AsymmetricEncrypt(bytes, this.Crypto.Keys.Item2);
      decryptedBytes = this.Crypto.AsymmetricDecrypt(ciphertext, this.Crypto.Keys.Item1);
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));

      // Test that the same content/key give the same result
      ciphertext = this.Crypto.AsymmetricEncrypt(bytes, this.Crypto.Keys.Item1);
      Assert.That(ciphertext.Value.IsIdenticalTo(this.Crypto.AsymmetricEncrypt(bytes, this.Crypto.Keys.Item1)));
    }

    /// <summary>
    /// The generate password test.
    /// </summary>
    [Test] public void GeneratePasswordTest() {
      string pswd = Aegis_DVL.Cryptography.Crypto.GeneratePassword();
      Assert.That(!string.IsNullOrEmpty(pswd));
    }

    /// <summary>
    /// The hash test.
    /// </summary>
    [Test] public void HashTest() {
      const string str1 = "hello";
      const string str2 = "hello";
      Assert.That(this.Crypto.Hash(Bytes.From(str1)).IsIdenticalTo(this.Crypto.Hash(Bytes.From(str2))));
    }

    /// <summary>
    /// The iv test.
    /// </summary>
    [Test] public void IvTest() {
      byte[] oldIv = this.Crypto.Iv;
      this.Crypto.NewIv();
      Assert.That(!oldIv.IsIdenticalTo(this.Crypto.Iv));
      this.Crypto.Iv = oldIv;
      Assert.That(oldIv.IsIdenticalTo(this.Crypto.Iv));
    }

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp] public void SetUp() {
      this._station = new Station(new TestUi(), key, "yo boii", 62001, "CryptoTestVoters.sqlite");
      this.Crypto = this._station.Crypto;
    }

    /// <summary>
    /// The symmetric test.
    /// </summary>
    [Test] public void SymmetricTest() {
      var key = new SymmetricKey(this.Crypto.GenerateSymmetricKey());
      const string testString = "Howdy there, partner!";
      byte[] bytes = Bytes.From(testString);
      CipherText ciphertext = this.Crypto.SymmetricEncrypt(bytes, key);
      Assert.That(!bytes.IsIdenticalTo(ciphertext));
      byte[] decryptedBytes = this.Crypto.SymmetricDecrypt(ciphertext, key);
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));
      Assert.That(decryptedBytes.To<string>().Equals(testString));
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      this._station.Dispose();
      this._station = null;
      File.Delete("CryptoTestVoters.sqlite");
    }

    #endregion
  }
}
