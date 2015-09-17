#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="DatabaseTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;

  using Aegis_DVL;
  using Aegis_DVL.Database;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  /// <summary>
  /// The database tests.
  /// </summary>
  [TestFixture] public class DatabaseTests {
    #region Public Properties

    /// <summary>
    /// Gets the station.
    /// </summary>
    public Station Station { get; private set; }

    #endregion

    // public ICrypto Crypto { get; private set; }
    #region Public Methods and Operators

    /// <summary>
    ///   Create the test database and station for database testing.
    /// </summary>
    [SetUp] public void SetUp() {
      Station = new Station(
        new TestUi(), 
        SystemTestData.Key, 
        SystemTestData.Password, 
        SystemTestData.ManagerPort,
        "DatabaseTestsManagerVoters.sqlite");
      Assert.That(Station.ValidMasterPassword(SystemTestData.Password));
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      if (Station != null) Station.Dispose();
      Station = null;
      File.Delete("DatabaseTestsManagerVoters.sqlite");
    }

    /// <summary>
    ///   Create some voter card data, insert it into the test DB,
    /// and check that various operations on said data hold.
    /// </summary>
    /*
    [Test] public void Test() {
      IDatabase db = Station.Database;
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);

      Assert.That(db[vn] == BallotStatus.Unavailable);

      // Contracts do not allow us to do this, but they can be disabled.... 
      // Assert.Throws<ArgumentException>(() => db[vn] = BallotStatus.NotReceived);
      db.Import(
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), 
                  Station.Crypto.VoterDataEncryptionKey)))
        });

      Assert.That(db.AllData != null);
      Assert.That(
        db.AllData.All(
          data => !Equals(data.BallotStatus, null) && 
                  !Equals(data.CPR, null) && 
                  !Equals(data.VoterNumber, null)));

      // CPR is in DB, but the voternumber doesn't match.
      Assert.That(db[new VoterNumber(123)] == BallotStatus.Unavailable);

      Assert.That(db[vn] == BallotStatus.NotReceived);
      db[vn] = BallotStatus.Received;
      Assert.That(db[vn] == BallotStatus.Received);
      db[vn] = BallotStatus.NotReceived;
      Assert.That(db[vn] == BallotStatus.NotReceived);

      bool notNull = true;
      db.AllData.ForEach(row => notNull = notNull & !Equals(row.VoterNumber, null));
      Assert.That(notNull);
    }

    /// <summary>
    ///   Create some voter card data, insert it into the test DB,
    /// </summary>
    [Test] public void TestCreateDatafile() {
      const string FileName = "TEST_VOTERDATA.data";
      IDatabase db = Station.Database;

      var vn0 = new VoterNumber(000000);
      var cpr0 = new CPR(1111222200);

      // var vn1 = new VoterNumber(000001);
      // var cpr1 = new CPR(1111222201);

      // var vn2 = new VoterNumber(000002);
      // var cpr2 = new CPR(1111222202);

      // var vn3 = new VoterNumber(000003);
      // var cpr3 = new CPR(1111222203);

      // var vn4 = new VoterNumber(000004);
      // var cpr4 = new CPR(1111222204);

      // var vn5 = new VoterNumber(000005);
      // var cpr5 = new CPR(1111222205);

      // var vn6 = new VoterNumber(000006);
      // var cpr6 = new CPR(1111222206);

      // var vn7 = new VoterNumber(000007);
      // var cpr7 = new CPR(1111222207);

      // var vn8 = new VoterNumber(000008);
      // var cpr8 = new CPR(1111222208);

      // var vn9 = new VoterNumber(000009);
      // var cpr9 = new CPR(1111222209);

      // db.Import(new List<EncryptedVoterData>
      // {
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn0.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr0.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr0.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn1.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr1.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr1.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn2.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr2.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr2.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn3.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr3.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr3.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn4.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr4.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr4.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn5.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr5.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr5.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn6.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr6.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr6.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn7.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr7.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr7.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn8.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr8.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr8.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
      // new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn9.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr9.Value), Station.Crypto.VoterDataEncryptionKey)), 
      // new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr9.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey)))
      // });
      var encData =
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(vn0.Value), 
                Station.Crypto.VoterDataEncryptionKey)),
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr0.Value), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr0.Value + (uint)BallotStatus.NotReceived), 
                Station.Crypto.VoterDataEncryptionKey)))
        };
      db.Import(encData);

      // Create file
      Bytes.From(db.AllData).ToFile(FileName);

      // Check if file is created
      Assert.That(File.Exists(FileName));

      byte[] decVData = Station.Crypto.AsymmetricDecrypt(
        new CipherText(Bytes.From(db.AllData)), 
        Station.Crypto.VoterDataEncryptionKey);
      Debug.WriteLine("Decrypted voter data: " + decVData);

      Bytes.From(db.AllData.ToString()).ToFile("DECRYPTED_VOTERDATA");
    }
     */

    /// <summary>
    /// The test master password db.
    /// </summary>
    /*
    [Test] public void TestMasterPasswordDb() {
      IDatabase db = Station.Database;
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);

      Assert.That(db[cpr, SystemTestData.Password] == BallotStatus.Unavailable);

      db.Import(
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(vn.Value), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), 
                Station.Crypto.VoterDataEncryptionKey))), 
          new EncryptedVoterData(
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(vn.Value + 5), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + 5), 
                Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Station.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived + 5), 
                Station.Crypto.VoterDataEncryptionKey)))
        });

      Assert.That(db.AllData != null);
      Assert.That(
        db.AllData.All(
          data => !Equals(data.BallotStatus, null) &&
                  !Equals(data.CPR, null) &&
                  !Equals(data.VoterNumber, null)));
      Assert.That(db[cpr, SystemTestData.Password] == BallotStatus.NotReceived);

      // Contracts do not allow us to do this, but they can be disabled.... 
      // Assert.Throws<ArgumentException>(() => status = db[cpr, SystemTestData.password]);
      // Assert.Throws<ArgumentException>(() => db[new CPR(123), SystemTestData.password] = BallotStatus.NotReceived);
      db[cpr, SystemTestData.Password] = BallotStatus.Received;
      Assert.That(db[cpr, SystemTestData.Password] == BallotStatus.Received);
      db[cpr, SystemTestData.Password] = BallotStatus.NotReceived;
      Assert.That(db[cpr, SystemTestData.Password] == BallotStatus.NotReceived);

      bool notNull = true;
      db.AllData.ForEach(row => notNull = notNull & !Equals(row.VoterNumber, null));
      Assert.That(notNull);
    }
      */
    #endregion
  }
}
