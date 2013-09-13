#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="UtilTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Net;

  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  using Org.BouncyCastle.Crypto;

  /// <summary>
  /// The util tests.
  /// </summary>
  [TestFixture] public class UtilTests {
    #region Public Properties

    /// <summary>
    /// Gets the test byte array.
    /// </summary>
    public byte[] TestByteArray { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   Test all the various byte and enumerable utility methods
    /// </summary>
    [Test] public void BytesAndEnumerableTest() {
      Assert.That(this.TestByteArray.AsBase64().Equals("AQIDBAU="));
      Assert.That(this.TestByteArray.AsString().Equals("[1] [2] [3] [4] [5] "));
      int sum = 0;
      this.TestByteArray.ForEach(x => sum += x);
      Assert.That(sum == 15);
      Assert.That(this.TestByteArray.IsIdenticalTo(this.TestByteArray));

      byte[] mergeArray = this.TestByteArray.Merge(this.TestByteArray);
      Assert.That(mergeArray.Length == 10);

      byte[] xorArray = this.TestByteArray.Xor(new byte[] { 5, 4, 3 });
      Assert.That(!xorArray.IsIdenticalTo(this.TestByteArray));

      const string str = "Test string";
      byte[] strBytes = Bytes.From(str);
      Assert.That(strBytes.To<string>().Equals(str));

      using (var stream = new MemoryStream(this.TestByteArray)) {
        byte[] bytes = Bytes.FromStream(stream);
        Assert.That(bytes.IsIdenticalTo(this.TestByteArray));

        Assert.That(bytes.IsIdenticalTo(Bytes.From(bytes)));
      }
    }

    /// <summary>
    /// The ip end point comparer test.
    /// </summary>
    [Test] public void IPEndPointComparerTest() {
      var ip1 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 56721);
      var ip2 = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 56722);
      var comparer = new IPEndPointComparer();
      Assert.That(comparer.Compare(ip1, ip2) == -1);
      Assert.That(comparer.Compare(ip2, ip1) == 1);
      Assert.That(comparer.Compare(ip1, ip1) == 0);
      Assert.That(comparer.Compare(ip2, ip2) == 0);
    }

    /// <summary>
    /// The key util test.
    /// </summary>
    [Test] public void KeyUtilTest() {
      var crypto = new Crypto(new AsymmetricKey());
      AsymmetricKeyParameter key = crypto.KeyPair.Item1.Value;

      Assert.That(key.ToBytes().ToKey().Equals(key));
    }

    /// <summary>
    /// The serialize size test.
    /// </summary>
    [Test] public void SerializeSizeTest() {
      const string ip = "192.68.0.1";
      const int port = 62000;
      string merged = ip + ":" + port;
      var ipendpoint = new IPEndPoint(IPAddress.Parse(ip), port);
      Console.WriteLine("Class-size: {0}", Bytes.From(ipendpoint).Length);
      Console.WriteLine("Primitive-size: {0}", Bytes.From(ip).Length + Bytes.From(port).Length);
      Console.WriteLine("Merged primitives-size: {0}", Bytes.From(merged).Length);

      var list = new List<string> { merged, merged, merged };
      var arr = new[] { merged, merged, merged };
      Console.WriteLine("List-size of merged primities: {0}", Bytes.From(list).Length);
      Console.WriteLine("Array-size of merged primities: {0}", Bytes.From(arr).Length);
    }

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp] public void SetUp() { this.TestByteArray = new byte[] { 1, 2, 3, 4, 5 }; }

    #endregion
  }
}
