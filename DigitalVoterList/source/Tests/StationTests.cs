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
      Assert.That(!this.Manager.Peers.ContainsKey(this.Peer4.Address));
      this.Manager.AddPeer(this.Peer4.Address, new AsymmetricKey(this.Peer4.Crypto.KeyPair.Public));
      Assert.That(this.Manager.Peers.ContainsKey(this.Peer4.Address));
      this.Manager.RemovePeer(this.Peer4.Address);
      Assert.That(!this.Manager.Peers.ContainsKey(this.Peer4.Address));
    }

    /// <summary>
    /// The announce add and remove peer test.
    /// </summary>
    [Test] public void AnnounceAddAndRemovePeerTest() {
      this.Manager.StartListening();
      this.Peer1.StartListening();
      this.Peer2.StartListening();
      this.Peer3.StartListening();
      this.Peer4.StartListening();
      Assert.That(
        !this.Manager.Peers.ContainsKey(this.Peer4.Address) && !this.Peer1.Peers.ContainsKey(this.Peer4.Address) &&
        !this.Peer2.Peers.ContainsKey(this.Peer4.Address) && !this.Peer3.Peers.ContainsKey(this.Peer4.Address));
      this.Manager.AnnounceAddPeer(this.Peer4.Address, 
        new AsymmetricKey(this.Peer4.Crypto.KeyPair.Public));
      this.Manager.AddPeer(this.Peer4.Address, 
        new AsymmetricKey(this.Peer4.Crypto.KeyPair.Public));
      Thread.Sleep(3000);
      Assert.That(
        this.Manager.Peers.ContainsKey(this.Peer4.Address) && this.Peer1.Peers.ContainsKey(this.Peer4.Address) &&
        this.Peer2.Peers.ContainsKey(this.Peer4.Address) && this.Peer3.Peers.ContainsKey(this.Peer4.Address));
      this.Manager.AnnounceRemovePeer(this.Peer4.Address);
      Thread.Sleep(3000);
      Assert.That(
        !this.Manager.Peers.ContainsKey(this.Peer4.Address) && !this.Peer1.Peers.ContainsKey(this.Peer4.Address) &&
        !this.Peer2.Peers.ContainsKey(this.Peer4.Address) && !this.Peer3.Peers.ContainsKey(this.Peer4.Address));
    }

    /// <summary>
    /// The announce start and end election test.
    /// </summary>
    [Test] public void AnnounceStartAndEndElectionTest() {
      Assert.That(
        !(this.Manager.ElectionInProgress && this.Peer1.ElectionInProgress && this.Peer2.ElectionInProgress &&
          this.Peer3.ElectionInProgress));
      this.AsyncManagerAnnounce(() => this.Manager.AnnounceStartElection());
      Assert.That(
        this.Manager.ElectionInProgress && this.Peer1.ElectionInProgress && this.Peer2.ElectionInProgress &&
        this.Peer3.ElectionInProgress);
      this.AsyncManagerAnnounce(() => this.Manager.AnnounceEndElection());
      Assert.That(
        !(this.Manager.ElectionInProgress && this.Peer1.ElectionInProgress && this.Peer2.ElectionInProgress &&
          this.Peer3.ElectionInProgress));
    }

    /// <summary>
    /// The ballot received and revoked.
    /// </summary>
    /*
    [Test] public void BallotReceivedAndRevoked() {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.Unavailable);
      this.Peer1.Database.Import(
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(
              this.Peer1.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), this.Peer1.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              this.Peer1.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), this.Peer1.Crypto.VoterDataEncryptionKey)), 
            new CipherText(
              this.Peer1.Crypto.AsymmetricEncrypt(
                Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), this.Peer1.Crypto.VoterDataEncryptionKey)))
        });

      Assert.That(this.Peer1.Database[vn] == BallotStatus.NotReceived);
      this.Peer1.BallotReceived(vn);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.Received);
      this.Peer1.RevokeBallot(vn);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.NotReceived);

      Assert.That(this.Peer1.Database[cpr, "yo boii"] == BallotStatus.NotReceived);
      this.Peer1.BallotReceived(cpr, "yo boii");
      Assert.That(this.Peer1.Database[cpr, "yo boii"] == BallotStatus.Received);
      this.Peer1.RevokeBallot(cpr, "yo boii");
      Assert.That(this.Peer1.Database[cpr, "yo boii"] == BallotStatus.NotReceived);
    }
    */

    /// <summary>
    /// The discover peers test.
    /// </summary>
    [Test] public void DiscoverPeersTest() {
      IPEndPoint[] peers = this.Manager.DiscoverPeers().ToArray();
      Assert.That(peers.Count() >= 0);
      foreach (IPEndPoint peer in peers) Console.WriteLine(peer);
    }

    /// <summary>
    /// The elect new manager.
    /// </summary>
    [Test] public void ElectNewManager() {
      Assert.That(this.Peer1.Manager.Equals(this.Manager.Address));
      this.AsyncManagerAnnounce(
        () => {
          // "Have" to send bogus command to kill the listener.
          // ReSharper disable ReturnValueOfPureMethodIsNotUsed
          this.Manager.StationActive(this.Peer1.Address);

          // ReSharper restore ReturnValueOfPureMethodIsNotUsed
          this.Peer1.ElectNewManager();
        });
      Assert.That(!this.Peer1.Manager.Equals(this.Manager.Address));
    }

    /// <summary>
    /// The enough stations test.
    /// </summary>
    [Test]
    public void EnoughStationsTest() {
      this.AsyncManagerAnnounce(() => Assert.That(this.Manager.EnoughStations));
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
      this.Manager.StartListening();

      // Waste some CPU time while the thread hopefully starts...
      int c = 0;
      while (c < 500000) c++;
      Console.WriteLine(c);
      Assert.That(this.Peer1.StationActive(this.Manager.Address));
      Assert.That(this.Peer1.StationActive(this.Manager.Address));
      this.Manager.StopListening();
      Assert.That(!this.Peer1.StationActive(this.Manager.Address));
    }

    /// <summary>
    /// The promote new manager test.
    /// </summary>
    [Test] public void PromoteNewManagerTest() {
      IPEndPoint oldManager = this.Manager.Address;
      IPEndPoint newManager = this.Peer1.Address;
      Assert.That(
        this.Manager.Manager.Equals(oldManager) && this.Peer1.Manager.Equals(oldManager) &&
        this.Peer2.Manager.Equals(oldManager) && this.Peer3.Manager.Equals(oldManager));
      Assert.That(this.Manager.IsManager && !this.Peer1.IsManager && !this.Peer2.IsManager && !this.Peer3.IsManager);
      this.AsyncManagerAnnounce(() => this.Manager.PromoteNewManager(newManager));
      Assert.That(!this.Manager.IsManager && this.Peer1.IsManager && !this.Peer2.IsManager && !this.Peer3.IsManager);
      Assert.That(
        this.Manager.Manager.Equals(newManager) && this.Peer1.Manager.Equals(newManager) &&
        this.Peer2.Manager.Equals(newManager) && this.Peer3.Manager.Equals(newManager));
    }

    /// <summary>
    /// The request ballot and announce ballot received and revoked test.
    /// </summary>
    /*
    [Test] public void RequestBallotAndAnnounceBallotReceivedAndRevokedTest() {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.Unavailable);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.Unavailable);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.Unavailable);
      Assert.That(this.Manager.Database[vn] == BallotStatus.Unavailable);
      var data = new List<EncryptedVoterData> {
        new EncryptedVoterData(
          new CipherText(
            this.Peer1.Crypto.AsymmetricEncrypt(Bytes.From(vn.Value), this.Peer1.Crypto.VoterDataEncryptionKey)), 
          new CipherText(
            this.Peer1.Crypto.AsymmetricEncrypt(Bytes.From(cpr.Value), this.Peer1.Crypto.VoterDataEncryptionKey)), 
          new CipherText(
            this.Peer1.Crypto.AsymmetricEncrypt(
              Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), this.Peer1.Crypto.VoterDataEncryptionKey)))
      };
      this.Peer1.Database.Import(data);
      this.Peer2.Database.Import(data);
      this.Peer3.Database.Import(data);
      this.Manager.Database.Import(data);

      Assert.That(this.Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Manager.Database[vn] == BallotStatus.NotReceived);
      IAsyncResult managerListenerResult = this.ManagerListener.BeginInvoke(null, null);
      this.AsyncManagerAnnounce(() => this.Peer1.RequestBallot(vn));
      this.ManagerListener.EndInvoke(managerListenerResult);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.Received);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.Received);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.Received);
      Assert.That(this.Manager.Database[vn] == BallotStatus.Received);
      this.AsyncManagerAnnounce(() => this.Manager.AnnounceRevokeBallot(vn, cpr));
      Assert.That(this.Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Manager.Database[vn] == BallotStatus.NotReceived);

      managerListenerResult = this.ManagerListener.BeginInvoke(null, null);
      this.AsyncManagerAnnounce(() => this.Peer1.RequestBallot(cpr, "yo boii"));
      this.ManagerListener.EndInvoke(managerListenerResult);
      Assert.That(this.Peer1.Database[vn] == BallotStatus.Received);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.Received);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.Received);
      Assert.That(this.Manager.Database[vn] == BallotStatus.Received);

      this.AsyncManagerAnnounce(() => this.Manager.AnnounceRevokeBallot(cpr, "yo boii"));
      Assert.That(this.Peer1.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer2.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Peer3.Database[vn] == BallotStatus.NotReceived);
      Assert.That(this.Manager.Database[vn] == BallotStatus.NotReceived);
    }
    */

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp] public void SetUp() {
      var ui = new TestUi();
      this.Manager = new Station(
        ui, SystemTestData.Key, SystemTestData.Password, SystemTestData.ManagerPort, 
        "StationTestsManagerVoters.sqlite", "StationTestsManagerLog.sqlite");
      byte[] pswd =
        this.Manager.Crypto.Hash(
          Bytes.From(SystemTestData.Password));
      this.Peer1 = new Station(ui, SystemTestData.StationPort, "StationTestsPeer1Voters.sqlite") {
        Manager = this.Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = this.Manager.Crypto.VoterDataEncryptionKey }
      };
      this.Peer2 = new Station(ui, SystemTestData.StationPort+1, "StationTestsPeer2Voters.sqlite")
      {
        Manager = this.Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = this.Manager.Crypto.VoterDataEncryptionKey }
      };
      this.Peer3 = new Station(ui, SystemTestData.StationPort+2, "StationTestsPeer3Voters.sqlite")
      {
        Manager = this.Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = this.Manager.Crypto.VoterDataEncryptionKey }
      };
      this.Peer4 = new Station(ui, SystemTestData.StationPort+3, "StationTestsPeer4Voters.sqlite")
      {
        Manager = this.Manager.Address, 
        MasterPassword = pswd, 
        Crypto = { VoterDataEncryptionKey = this.Manager.Crypto.VoterDataEncryptionKey }
      };

      this.Manager.StopListening();
      this.Peer1.StopListening();
      this.Peer2.StopListening();
      this.Peer3.StopListening();
      this.Peer4.StopListening();

      this.Peer1.Logger = new Logger(this.Peer1, "StationsTestsPeer1Log.sqlite");
      this.Peer2.Logger = new Logger(this.Peer2, "StationsTestsPeer2Log.sqlite");
      this.Peer3.Logger = new Logger(this.Peer3, "StationsTestsPeer3Log.sqlite");
      this.Peer4.Logger = new Logger(this.Peer4, "StationsTestsPeer4Log.sqlite");

      this.Manager.AddPeer(this.Peer1.Address, 
        new AsymmetricKey(this.Peer1.Crypto.KeyPair.Public));
      this.Manager.AddPeer(this.Peer2.Address, new AsymmetricKey(this.Peer2.Crypto.KeyPair.Public));
      this.Manager.AddPeer(this.Peer3.Address, new AsymmetricKey(this.Peer3.Crypto.KeyPair.Public));

      this.Peer1.AddPeer(this.Manager.Address, new AsymmetricKey(this.Manager.Crypto.KeyPair.Public));
      this.Peer1.AddPeer(this.Peer2.Address, new AsymmetricKey(this.Peer2.Crypto.KeyPair.Public));
      this.Peer1.AddPeer(this.Peer3.Address, new AsymmetricKey(this.Peer3.Crypto.KeyPair.Public));

      this.Peer2.AddPeer(this.Manager.Address, new AsymmetricKey(this.Manager.Crypto.KeyPair.Public));
      this.Peer2.AddPeer(this.Peer1.Address, new AsymmetricKey(this.Peer1.Crypto.KeyPair.Public));
      this.Peer2.AddPeer(this.Peer3.Address, new AsymmetricKey(this.Peer3.Crypto.KeyPair.Public));

      this.Peer3.AddPeer(this.Manager.Address, new AsymmetricKey(this.Manager.Crypto.KeyPair.Public));
      this.Peer3.AddPeer(this.Peer1.Address, new AsymmetricKey(this.Peer1.Crypto.KeyPair.Public));
      this.Peer3.AddPeer(this.Peer2.Address, new AsymmetricKey(this.Peer2.Crypto.KeyPair.Public));
      /*
      this.ManagerListener = this.Manager.Communicator.NetworkReceiveThread;
      this.Peer1Listener = this.Peer1.Communicator.NetworkReceiveThread;
      this.Peer2Listener = this.Peer2.Communicator.NetworkReceiveThread;
      this.Peer3Listener = this.Peer3.Communicator.NetworkReceiveThread;
       * */
    }

    /// <summary>
    /// The start and end election test.
    /// </summary>
    [Test] public void StartAndEndElectionTest() {
      Assert.That(!this.Manager.ElectionInProgress);
      this.Manager.StartElection();
      Assert.That(this.Manager.ElectionInProgress);
      this.Manager.EndElection();
      Assert.That(!this.Manager.ElectionInProgress);
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
      this.Manager.Dispose();
      this.Peer1.Dispose();
      this.Peer2.Dispose();
      this.Peer3.Dispose();
      this.Peer4.Dispose();
      this.Manager = null;
      this.Peer1 = null;
      this.Peer2 = null;
      this.Peer3 = null;
      this.Peer4 = null;
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
      Assert.That(this.Manager.ValidMasterPassword(SystemTestData.Password));
      Assert.That(!this.Manager.ValidMasterPassword(SystemTestData.Password + "foo"));
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
      IAsyncResult peer1ListenerResult = this.Peer1Listener.BeginInvoke(null, null);
      IAsyncResult peer2ListenerResult = this.Peer2Listener.BeginInvoke(null, null);
      IAsyncResult peer3ListenerResult = this.Peer3Listener.BeginInvoke(null, null);

      // Waste some CPU time while the thread hopefully starts...
      // TODO: ICK!
      int c = 0;
      while (c < 5000000) c++;
      Console.WriteLine(c);
      invoke();

      this.Peer1Listener.EndInvoke(peer1ListenerResult);
      this.Peer2Listener.EndInvoke(peer2ListenerResult);
      this.Peer3Listener.EndInvoke(peer3ListenerResult);
    }

    #endregion
  }
}
