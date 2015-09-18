#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StationTests.cs" company="DemTech">
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
  using System.Linq;
  using System.Net;
  using System.Threading;

  using Aegis_DVL;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Logging;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  /// <summary>
  /// The station tests.
  /// </summary>
  [TestFixture] public class StationTests {
    #region Delegates

    /// <summary>
    /// The listener.
    /// </summary>
    public delegate void Listener();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the manager.
    /// </summary>
    public Station Manager { get; private set; }

    /// <summary>
    /// Gets the manager listener.
    /// </summary>
    public Listener ManagerListener { get; private set; }

    /// <summary>
    /// Gets the peer 1.
    /// </summary>
    public Station Peer1 { get; private set; }

    /// <summary>
    /// Gets the peer 1 listener.
    /// </summary>
    public Listener Peer1Listener { get; private set; }

    /// <summary>
    /// Gets the peer 2.
    /// </summary>
    public Station Peer2 { get; private set; }

    /// <summary>
    /// Gets the peer 2 listener.
    /// </summary>
    public Listener Peer2Listener { get; private set; }

    /// <summary>
    /// Gets the peer 3.
    /// </summary>
    public Station Peer3 { get; private set; }

    /// <summary>
    /// Gets the peer 3 listener.
    /// </summary>
    public Listener Peer3Listener { get; private set; }

    /// <summary>
    /// Gets the peer 4.
    /// </summary>
    public Station Peer4 { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The add and remove peer test.
    /// </summary>
    [Test] public void AddAndRemovePeerTest() {
      Assert.That(!Manager.Peers.ContainsKey(Peer4.Address));
      Manager.AddPeer(Peer4.Address, new AsymmetricKey(Peer4.Crypto.KeyPair.Public));
      Assert.That(Manager.Peers.ContainsKey(Peer4.Address));
      Manager.RemovePeer(Peer4.Address, true);
      Assert.That(!Manager.Peers.ContainsKey(Peer4.Address));
    }

    /// <summary>
    /// The announce add and remove peer test.
    /// </summary>
    [Test] public void AnnounceAddAndRemovePeerTest() {
      Manager.StartListening();
      Peer1.StartListening();
      Peer2.StartListening();
      Peer3.StartListening();
      Peer4.StartListening();
      Assert.That(
        !Manager.Peers.ContainsKey(Peer4.Address) && !Peer1.Peers.ContainsKey(Peer4.Address) &&
        !Peer2.Peers.ContainsKey(Peer4.Address) && !Peer3.Peers.ContainsKey(Peer4.Address));
      Manager.AnnounceAddPeer(Peer4.Address, 
        new AsymmetricKey(Peer4.Crypto.KeyPair.Public));
      Manager.AddPeer(Peer4.Address, 
        new AsymmetricKey(Peer4.Crypto.KeyPair.Public));
      Thread.Sleep(3000);
      Assert.That(
        Manager.Peers.ContainsKey(Peer4.Address) && Peer1.Peers.ContainsKey(Peer4.Address) &&
        Peer2.Peers.ContainsKey(Peer4.Address) && Peer3.Peers.ContainsKey(Peer4.Address));
      Manager.AnnounceRemovePeer(Peer4.Address);
      Thread.Sleep(3000);
      Assert.That(
        !Manager.Peers.ContainsKey(Peer4.Address) && !Peer1.Peers.ContainsKey(Peer4.Address) &&
        !Peer2.Peers.ContainsKey(Peer4.Address) && !Peer3.Peers.ContainsKey(Peer4.Address));
    }

    /// <summary>
    /// The announce start and end election test.
    /// </summary>
    [Test] public void AnnounceStartAndEndElectionTest() {
      Assert.That(
        !(Manager.ElectionInProgress && Peer1.ElectionInProgress && Peer2.ElectionInProgress &&
          Peer3.ElectionInProgress));
      AsyncManagerAnnounce(() => Manager.AnnounceStartElection());
      Assert.That(
        Manager.ElectionInProgress && Peer1.ElectionInProgress && Peer2.ElectionInProgress &&
        Peer3.ElectionInProgress);
      AsyncManagerAnnounce(() => Manager.AnnounceEndElection());
      Assert.That(
        !(Manager.ElectionInProgress && Peer1.ElectionInProgress && Peer2.ElectionInProgress &&
          Peer3.ElectionInProgress));
    }

    /// <summary>
    /// The ballot received and revoked.
    /// </summary>
    /*
    [Test] public void BallotReceivedAndRevoked() {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      Assert.That(Peer1.Database[vn] == BallotStatus.Unavailable);
      Peer1.Database.Import(
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(
              Peer1.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), Peer1.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Peer1.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), Peer1.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              Peer1.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), Peer1.Crypto.VoterDataEncryptionKey)))
        });

      Assert.That(Peer1.Database[vn] == BallotStatus.NotReceived);
      Peer1.BallotReceived(vn);
      Assert.That(Peer1.Database[vn] == BallotStatus.Received);
      Peer1.RevokeBallot(vn);
      Assert.That(Peer1.Database[vn] == BallotStatus.NotReceived);

      Assert.That(Peer1.Database[cpr, "yo boii"] == BallotStatus.NotReceived);
      Peer1.BallotReceived(cpr, "yo boii");
      Assert.That(Peer1.Database[cpr, "yo boii"] == BallotStatus.Received);
      Peer1.RevokeBallot(cpr, "yo boii");
      Assert.That(Peer1.Database[cpr, "yo boii"] == BallotStatus.NotReceived);
    }
    */

    /// <summary>
    /// The elect new manager.
    /// </summary>
    [Test] public void ElectNewManager() {
      Assert.That(Peer1.Manager.Equals(Manager.Address));
      AsyncManagerAnnounce(
        () => {
          // "Have" to send bogus command to kill the listener.
          // ReSharper disable ReturnValueOfPureMethodIsNotUsed
          Manager.StationActive(Peer1.Address);

          // ReSharper restore ReturnValueOfPureMethodIsNotUsed
          Peer1.ElectNewManager();
        });
      Assert.That(!Peer1.Manager.Equals(Manager.Address));
    }

    /// <summary>
    /// The enough stations test.
    /// </summary>
    [Test]
    public void EnoughStationsTest() {
      AsyncManagerAnnounce(() => Assert.That(Manager.EnoughStations));
    }

    /// <summary>
    /// The exchange public keys test.
    /// </summary>
    [Test] public void ExchangePublicKeysTest() {
      var ui = new TestUi();
      using (
        var manager = new Station(
          ui, 
          SystemTestData.Key, 
          SystemTestData.Password, 
          SystemTestData.ManagerPort, 
          "ExchangePublicKeysTestManagerVoters.sqlite", 
          "ExchangePublicKeysTestManagerLog.sqlite"))
      using (var station = new Station(ui, SystemTestData.StationPort, 
                                       "ExchangePublicKeysTestStationVoters.sqlite")) {
        Assert.That(!manager.Peers.ContainsKey(station.Address));
        Assert.That(!station.Peers.ContainsKey(manager.Address));
        manager.ExchangePublicKeys(station.Address);

        // Wait some time while they synchronize.
        Thread.Sleep(3000);
        Assert.That(manager.Peers.ContainsKey(station.Address));
        Assert.That(station.Peers.ContainsKey(manager.Address));
      }

      File.Delete("ExchangePublicKeysTestManagerVoters.sqlite");
      File.Delete("ExchangePublicKeysTestStationVoters.sqlite");
      File.Delete("ExchangePublicKeysTestManagerLog.sqlite");
    }

    /// <summary>
    /// The listener test.
    /// </summary>
    [Test] public void ListenerTest() {
      Manager.StartListening();

      // Waste some CPU time while the thread hopefully starts...
      int c = 0;
      while (c < 500000) c++;
      Console.WriteLine(c);
      Assert.That(Peer1.StationActive(Manager.Address));
      Assert.That(Peer1.StationActive(Manager.Address));
      Manager.StopListening();
      Assert.That(!Peer1.StationActive(Manager.Address));
    }

    /// <summary>
    /// The promote new manager test.
    /// </summary>
    [Test] public void PromoteNewManagerTest() {
      IPEndPoint oldManager = Manager.Address;
      IPEndPoint newManager = Peer1.Address;
      Assert.That(
        Manager.Manager.Equals(oldManager) && Peer1.Manager.Equals(oldManager) &&
        Peer2.Manager.Equals(oldManager) && Peer3.Manager.Equals(oldManager));
      Assert.That(Manager.IsManager && !Peer1.IsManager && !Peer2.IsManager && !Peer3.IsManager);
      AsyncManagerAnnounce(() => Manager.PromoteNewManager(newManager));
      Assert.That(!Manager.IsManager && Peer1.IsManager && !Peer2.IsManager && !Peer3.IsManager);
      Assert.That(
        Manager.Manager.Equals(newManager) && Peer1.Manager.Equals(newManager) &&
        Peer2.Manager.Equals(newManager) && Peer3.Manager.Equals(newManager));
    }

    /// <summary>
    /// The request ballot and announce ballot received and revoked test.
    /// </summary>
    /*
    [Test] public void RequestBallotAndAnnounceBallotReceivedAndRevokedTest() {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      Assert.That(Peer1.Database[vn] == BallotStatus.Unavailable);
      Assert.That(Peer2.Database[vn] == BallotStatus.Unavailable);
      Assert.That(Peer3.Database[vn] == BallotStatus.Unavailable);
      Assert.That(Manager.Database[vn] == BallotStatus.Unavailable);
      var data = new List<EncryptedVoterData> {
        new EncryptedVoterData(
          new CipherText(
            Peer1.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), Peer1.Crypto.VoterDataEncryptionKey)), 
          new CipherText(
            Peer1.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), Peer1.Crypto.VoterDataEncryptionKey)), 
          new CipherText(
            Peer1.Crypto.AsymmetricEncrypt(
              Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), Peer1.Crypto.VoterDataEncryptionKey)))
      };
      Peer1.Database.Import(data);
      Peer2.Database.Import(data);
      Peer3.Database.Import(data);
      Manager.Database.Import(data);

      Assert.That(Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Manager.Database[vn] == BallotStatus.NotReceived);
      IAsyncResult managerListenerResult = ManagerListener.BeginInvoke(null, null);
      AsyncManagerAnnounce(() => Peer1.RequestBallot(vn));
      ManagerListener.EndInvoke(managerListenerResult);
      Assert.That(Peer1.Database[vn] == BallotStatus.Received);
      Assert.That(Peer2.Database[vn] == BallotStatus.Received);
      Assert.That(Peer3.Database[vn] == BallotStatus.Received);
      Assert.That(Manager.Database[vn] == BallotStatus.Received);
      AsyncManagerAnnounce(() => Manager.AnnounceRevokeBallot(vn, cpr));
      Assert.That(Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Manager.Database[vn] == BallotStatus.NotReceived);

      managerListenerResult = ManagerListener.BeginInvoke(null, null);
      AsyncManagerAnnounce(() => Peer1.RequestBallot(cpr, "yo boii"));
      ManagerListener.EndInvoke(managerListenerResult);
      Assert.That(Peer1.Database[vn] == BallotStatus.Received);
      Assert.That(Peer2.Database[vn] == BallotStatus.Received);
      Assert.That(Peer3.Database[vn] == BallotStatus.Received);
      Assert.That(Manager.Database[vn] == BallotStatus.Received);

      AsyncManagerAnnounce(() => Manager.AnnounceRevokeBallot(cpr, "yo boii"));
      Assert.That(Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(Manager.Database[vn] == BallotStatus.NotReceived);
    }
    */

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp] public void SetUp() {
      var ui = new TestUi();
      Manager = new Station(
        ui, SystemTestData.Key, SystemTestData.Password, SystemTestData.ManagerPort, 
        "StationTestsManagerVoters.sqlite", "StationTestsManagerLog.sqlite");
      byte[] pswd =
        Manager.Crypto.Hash(
          Bytes.From(SystemTestData.Password));
      Peer1 = new Station(ui, SystemTestData.StationPort, "StationTestsPeer1Voters.sqlite") {
        Manager = Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = Manager.Crypto.VoterDataEncryptionKey }
      };
      Peer2 = new Station(ui, SystemTestData.StationPort+1, "StationTestsPeer2Voters.sqlite")
      {
        Manager = Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = Manager.Crypto.VoterDataEncryptionKey }
      };
      Peer3 = new Station(ui, SystemTestData.StationPort+2, "StationTestsPeer3Voters.sqlite")
      {
        Manager = Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = Manager.Crypto.VoterDataEncryptionKey }
      };
      Peer4 = new Station(ui, SystemTestData.StationPort+3, "StationTestsPeer4Voters.sqlite")
      {
        Manager = Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = Manager.Crypto.VoterDataEncryptionKey }
      };

      Manager.StopListening();
      Peer1.StopListening();
      Peer2.StopListening();
      Peer3.StopListening();
      Peer4.StopListening();

      Peer1.Logger = new Logger(Peer1, "StationsTestsPeer1Log.sqlite");
      Peer2.Logger = new Logger(Peer2, "StationsTestsPeer2Log.sqlite");
      Peer3.Logger = new Logger(Peer3, "StationsTestsPeer3Log.sqlite");
      Peer4.Logger = new Logger(Peer4, "StationsTestsPeer4Log.sqlite");

      Manager.AddPeer(Peer1.Address, 
        new AsymmetricKey(Peer1.Crypto.KeyPair.Public));
      Manager.AddPeer(Peer2.Address, new AsymmetricKey(Peer2.Crypto.KeyPair.Public));
      Manager.AddPeer(Peer3.Address, new AsymmetricKey(Peer3.Crypto.KeyPair.Public));

      Peer1.AddPeer(Manager.Address, new AsymmetricKey(Manager.Crypto.KeyPair.Public));
      Peer1.AddPeer(Peer2.Address, new AsymmetricKey(Peer2.Crypto.KeyPair.Public));
      Peer1.AddPeer(Peer3.Address, new AsymmetricKey(Peer3.Crypto.KeyPair.Public));

      Peer2.AddPeer(Manager.Address, new AsymmetricKey(Manager.Crypto.KeyPair.Public));
      Peer2.AddPeer(Peer1.Address, new AsymmetricKey(Peer1.Crypto.KeyPair.Public));
      Peer2.AddPeer(Peer3.Address, new AsymmetricKey(Peer3.Crypto.KeyPair.Public));

      Peer3.AddPeer(Manager.Address, new AsymmetricKey(Manager.Crypto.KeyPair.Public));
      Peer3.AddPeer(Peer1.Address, new AsymmetricKey(Peer1.Crypto.KeyPair.Public));
      Peer3.AddPeer(Peer2.Address, new AsymmetricKey(Peer2.Crypto.KeyPair.Public));
      /*
      ManagerListener = Manager.Communicator.NetworkReceiveThread;
      Peer1Listener = Peer1.Communicator.NetworkReceiveThread;
      Peer2Listener = Peer2.Communicator.NetworkReceiveThread;
      Peer3Listener = Peer3.Communicator.NetworkReceiveThread;
       * */
    }

    /// <summary>
    /// The start and end election test.
    /// </summary>
    [Test] public void StartAndEndElectionTest() {
      Assert.That(!Manager.ElectionInProgress);
      Manager.StartElection();
      Assert.That(Manager.ElectionInProgress);
      Manager.EndElection();
      Assert.That(!Manager.ElectionInProgress);
    }

    /// <summary>
    /// The start new manager election test.
    /// </summary>
    [Test] public void StartNewManagerElectionTest() {
      var ui = new TestUi();
      using (
        var manager = new Station(
          ui, 
          SystemTestData.Key, 
          SystemTestData.Password, 
          SystemTestData.ManagerPort, 
          "ExchangePublicKeysTestManagerVoters.sqlite", 
          "ExchangePublicKeysTestManagerLog.sqlite")) {
        AsymmetricKey pswd = manager.Crypto.VoterDataEncryptionKey;
        using (
          var station = new Station(ui, SystemTestData.StationPort, 
                                    "ExchangePublicKeysTestStationVoters.sqlite") {
            Manager = manager.Address, 
            Crypto = { VoterDataEncryptionKey = pswd }, 
            MasterPassword = manager.MasterPassword
          })
        using (
          var station2 = new Station(ui, SystemTestData.PeerPort, 
                                     "ExchangePublicKeysTestStation2Voters.sqlite") {
            Manager = manager.Address, 
            Crypto = { VoterDataEncryptionKey = pswd }, 
            MasterPassword = manager.MasterPassword
          }) {
          station.Logger = new Logger(station, "ExchangePublicKeysTestStationLog.sqlite");
          station2.Logger = new Logger(station2, "ExchangePublicKeysTestStation2Log.sqlite");
          Assert.That(station.Manager.Equals(manager.Address));
          Assert.That(station2.Manager.Equals(manager.Address));
          Assert.That(station2.Manager.Equals(station.Manager));

          station.AddPeer(manager.Address, new AsymmetricKey(manager.Crypto.KeyPair.Public));
          station.AddPeer(station2.Address, new AsymmetricKey(station2.Crypto.KeyPair.Public));
          station2.AddPeer(manager.Address, new AsymmetricKey(manager.Crypto.KeyPair.Public));
          station2.AddPeer(station.Address, new AsymmetricKey(station.Crypto.KeyPair.Public));

          manager.StopListening();
          station.StartNewManagerElection();
          Thread.Sleep(5000);
          Assert.That(!station.Manager.Equals(manager.Address));
          Assert.That(!station2.Manager.Equals(manager.Address));
          Assert.That(station2.Manager.Equals(station.Manager));
        }
      }

      File.Delete("ExchangePublicKeysTestManagerVoters.sqlite");
      File.Delete("ExchangePublicKeysTestStationVoters.sqlite");
      File.Delete("ExchangePublicKeysTestStation2Voters.sqlite");
      File.Delete("ExchangePublicKeysTestManagerLog.sqlite");
      File.Delete("ExchangePublicKeysTestStationLog.sqlite");
      File.Delete("ExchangePublicKeysTestStation2Log.sqlite");
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      Manager.Dispose();
      Peer1.Dispose();
      Peer2.Dispose();
      Peer3.Dispose();
      Peer4.Dispose();
      Manager = null;
      Peer1 = null;
      Peer2 = null;
      Peer3 = null;
      Peer4 = null;
      File.Delete("StationTestsManagerVoters.sqlite");
      File.Delete("StationTestsPeer1Voters.sqlite");
      File.Delete("StationTestsPeer2Voters.sqlite");
      File.Delete("StationTestsPeer3Voters.sqlite");
      File.Delete("StationTestsPeer4Voters.sqlite");

      File.Delete("StationsTestsManagerLog.sqlite");
      File.Delete("StationsTestsPeer1Log.sqlite");
      File.Delete("StationsTestsPeer2Log.sqlite");
      File.Delete("StationsTestsPeer3Log.sqlite");
      File.Delete("StationsTestsPeer4Log.sqlite");
    }

    /// <summary>
    /// The valid master password test.
    /// </summary>
    [Test] public void ValidMasterPasswordTest() {
      Assert.That(Manager.ValidMasterPassword(SystemTestData.Password));
      Assert.That(!Manager.ValidMasterPassword(SystemTestData.Password + "foo"));
    }

    #endregion

    #region Methods

    /// <summary>
    /// The async manager announce.
    /// </summary>
    /// <param name="invoke">
    /// The invoke.
    /// </param>
    private void AsyncManagerAnnounce(Action invoke) {
      IAsyncResult peer1ListenerResult = Peer1Listener.BeginInvoke(null, null);
      IAsyncResult peer2ListenerResult = Peer2Listener.BeginInvoke(null, null);
      IAsyncResult peer3ListenerResult = Peer3Listener.BeginInvoke(null, null);

      // Waste some CPU time while the thread hopefully starts...
      // TODO: ICK!
      int c = 0;
      while (c < 5000000) c++;
      Console.WriteLine(c);
      invoke();

      Peer1Listener.EndInvoke(peer1ListenerResult);
      Peer2Listener.EndInvoke(peer2ListenerResult);
      Peer3Listener.EndInvoke(peer3ListenerResult);
    }

    #endregion
  }
}
