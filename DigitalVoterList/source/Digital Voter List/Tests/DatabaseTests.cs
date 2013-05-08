using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Aegis_DVL;
using Aegis_DVL.Cryptography;
using Aegis_DVL.Data_Types;
using Aegis_DVL.Util;
using NUnit.Framework;

namespace Tests {
    [TestFixture]
    public class DatabaseTests {
        public ICrypto Crypto { get; private set; }
        /// <summary>
        /// SetUp test helper properties.
        /// </summary>
        [SetUp]
        public void SetUp() {
            Station = new Station(new TestUi(), "dataEncryption.key", "yo boii", 62000, "DatabaseTestVoters.sqlite");
            Assert.That(Station.ValidMasterPassword("yo boii"));
        }

        [TearDown]
        public void TearDown() {
            Station.Dispose();
            Station = null;
            File.Delete("DatabaseTestVoters.sqlite");
        }

        public Station Station { get; private set; }

        [Test]
        public void TestCreateDatafile()
        {
            const string fileName = "TEST_VOTERDATA.data";
            var db = Station.Database;
            
            var vn0 = new VoterNumber(000000);
            var cpr0 = new CPR(1111222200);

            var vn1 = new VoterNumber(000001);
            var cpr1 = new CPR(1111222201);

            var vn2 = new VoterNumber(000002);
            var cpr2 = new CPR(1111222202);

            var vn3 = new VoterNumber(000003);
            var cpr3 = new CPR(1111222203);

            var vn4 = new VoterNumber(000004);
            var cpr4 = new CPR(1111222204);

            var vn5 = new VoterNumber(000005);
            var cpr5 = new CPR(1111222205);

            var vn6 = new VoterNumber(000006);
            var cpr6 = new CPR(1111222206);

            var vn7 = new VoterNumber(000007);
            var cpr7 = new CPR(1111222207);

            var vn8 = new VoterNumber(000008);
            var cpr8 = new CPR(1111222208);

            var vn9 = new VoterNumber(000009);
            var cpr9 = new CPR(1111222209);
            /*
            db.Import(new List<EncryptedVoterData>
                          {
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn0.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr0.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr0.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn1.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr1.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr1.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn2.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr2.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr2.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn3.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr3.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr3.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn4.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr4.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr4.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn5.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr5.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr5.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn6.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr6.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr6.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn7.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr7.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr7.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn8.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr8.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr8.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))),
                              new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn9.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr9.Value), Station.Crypto.VoterDataEncryptionKey)), 
                                  new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr9.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey)))
                          });
            */

            var encData = new List<EncryptedVoterData>
                              {
                                  new EncryptedVoterData(
                                      new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn0.Value),
                                                                                      Station.Crypto.
                                                                                          VoterDataEncryptionKey)),
                                      new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr0.Value),
                                                                                      Station.Crypto.
                                                                                          VoterDataEncryptionKey)),
                                      new CipherText(
                                          Station.Crypto.AsymmetricEncrypt(
                                              Bytes.From(cpr0.Value + (uint) BallotStatus.NotReceived),
                                              Station.Crypto.VoterDataEncryptionKey)))
                              };
            db.Import(encData);
            //Create file
            Bytes.From(db.AllData).ToFile(fileName);

            //Check if file is created
            Assert.That(File.Exists(fileName));

            var decVData = Station.Crypto.AsymmetricDecrypt(new CipherText(Bytes.From(db.AllData)), Station.Crypto.VoterDataEncryptionKey);
            Debug.WriteLine("Decrypted voter data: " + decVData);

            Bytes.From(db.AllData.ToString()).ToFile("DECRYPTED_VOTERDATA");
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Test() {
            var db = Station.Database;
            var vn = new VoterNumber(250000);
            var cpr = new CPR(2312881234);

            Assert.That(db[vn] == BallotStatus.Unavailable);
            //Contracts do not allow us to do this, but they can be disabled.... 
            //Assert.Throws<ArgumentException>(() => db[vn] = BallotStatus.NotReceived);

            db.Import(new List<EncryptedVoterData> { new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))) });

            Assert.That(db.AllData != null);
            Assert.That(db.AllData.All(data => !Equals(data.BallotStatus, null) && !Equals(data.CPR, null) && !Equals(data.VoterNumber, null)));
            
            //CPR is in DB, but the voternumber doesn't match.
            Assert.That(db[new VoterNumber(123)] == BallotStatus.Unavailable);
            
            Assert.That(db[vn] == BallotStatus.NotReceived);
            db[vn] = BallotStatus.Received;
            Assert.That(db[vn] == BallotStatus.Received);
            db[vn] = BallotStatus.NotReceived;
            Assert.That(db[vn] == BallotStatus.NotReceived);

            var notNull = true;
            db.AllData.ForEach(row => notNull = notNull & !Equals(row.VoterNumber, null));
            Assert.That(notNull);
        }

        [Test]
        public void TestMasterPasswordDb() {
            var db = Station.Database;
            var vn = new VoterNumber(250000);
            var cpr = new CPR(2312881234);

            Assert.That(db[cpr, "yo boii"] == BallotStatus.Unavailable);

            db.Import(new List<EncryptedVoterData> { new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), Station.Crypto.VoterDataEncryptionKey))), new EncryptedVoterData(new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value + 5), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value + 5), Station.Crypto.VoterDataEncryptionKey)), new CipherText(Station.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived + 5), Station.Crypto.VoterDataEncryptionKey))) });

            Assert.That(db.AllData != null);
            Assert.That(db.AllData.All(data => !Equals(data.BallotStatus, null) && !Equals(data.CPR, null) && !Equals(data.VoterNumber, null)));
            Assert.That(db[cpr, "yo boii"] == BallotStatus.NotReceived);

            //Contracts do not allow us to do this, but they can be disabled.... 
            //Assert.Throws<ArgumentException>(() => status = db[cpr, "yo boii"]);
            //Assert.Throws<ArgumentException>(() => db[new CPR(123), "yo boii"] = BallotStatus.NotReceived);

            db[cpr, "yo boii"] = BallotStatus.Received;
            Assert.That(db[cpr, "yo boii"] == BallotStatus.Received);
            db[cpr, "yo boii"] = BallotStatus.NotReceived;
            Assert.That(db[cpr, "yo boii"] == BallotStatus.NotReceived);

            var notNull = true;
            db.AllData.ForEach(row => notNull = notNull & !Equals(row.VoterNumber, null));
            Assert.That(notNull);
        }
        
    }
}
