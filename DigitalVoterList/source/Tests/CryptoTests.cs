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

  using Org.BouncyCastle.Crypto;

  using NUnit.Framework;

  /// <summary>
  /// The crypto tests.
  /// </summary>
  [TestFixture] public class CryptoTests {
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
    /// Test asymmetric crypto features of Crypto subsystem.
    /// </summary>
    [Test] public void AsymmetricTest() {
      // Encrypt/decrypt
      const string testString = "Howdy there, partner!";
      byte[] bytes = Bytes.From(testString);
      CipherText ciphertext = Crypto.AsymmetricEncrypt(bytes, 
        new AsymmetricKey(Crypto.KeyPair.Public));
      Assert.That(!bytes.IsIdenticalTo(ciphertext));
      byte[] decryptedBytes = Crypto.AsymmetricDecrypt(ciphertext, 
        new AsymmetricKey(Crypto.KeyPair.Private));
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));
      Assert.That(decryptedBytes.To<string>().Equals(testString));

      // Encrypt/decrypt using reversed keys
      ciphertext = Crypto.AsymmetricEncrypt(bytes, 
        new AsymmetricKey(Crypto.KeyPair.Private));
      decryptedBytes = Crypto.AsymmetricDecrypt(ciphertext, 
        new AsymmetricKey(Crypto.KeyPair.Public));
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));

      // Test that the same content/key give the same result
      ciphertext = Crypto.AsymmetricEncrypt(bytes, 
        new AsymmetricKey(Crypto.KeyPair.Public));
      Assert.That(ciphertext.Value.IsIdenticalTo(Crypto.AsymmetricEncrypt(bytes,
        new AsymmetricKey(Crypto.KeyPair.Public))));
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
      const string Str1 = "hello";
      const string Str2 = "hello";
      Assert.That(Crypto.Hash(Bytes.From(Str1)).
        IsIdenticalTo(Crypto.Hash(Bytes.From(Str2))));
    }

    /// <summary>
    /// The iv test.
    /// </summary>
    [Test] public void IvTest() {
      byte[] oldIv = Crypto.Iv;
      Crypto.NewIv();
      Assert.That(!oldIv.IsIdenticalTo(Crypto.Iv));
      Crypto.Iv = oldIv;
      Assert.That(oldIv.IsIdenticalTo(Crypto.Iv));
    }

    /// <summary>
    /// The symmetric test.
    /// </summary>
    [Test] public void SymmetricTest() {
      var key = new SymmetricKey(Crypto.GenerateSymmetricKey());
      const string TestString = "Howdy there, partner!";
      byte[] bytes = Bytes.From(TestString);
      CipherText ciphertext = Crypto.SymmetricEncrypt(bytes, key);
      Assert.That(!bytes.IsIdenticalTo(ciphertext));
      byte[] decryptedBytes = Crypto.SymmetricDecrypt(ciphertext, key);
      Assert.That(bytes.IsIdenticalTo(decryptedBytes));
      Assert.That(decryptedBytes.To<string>().Equals(TestString));
    }

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp]
    public void SetUp() {
      _station = new Station(new TestUi(), SystemTestData.Key, SystemTestData.Password,
        SystemTestData.StationPort, "CryptoTestVoters.sqlite");
      Crypto = _station.Crypto;
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      _station.Dispose();
      _station = null;
      File.Delete("CryptoTestVoters.sqlite");
    }

    #endregion
  }
}
