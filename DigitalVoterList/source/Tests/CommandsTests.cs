#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="CommandsTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests
{
  using System.Collections.Generic;
  using System.IO;

  using Aegis_DVL;
  using Aegis_DVL.Commands;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  /// <summary>
  ///   The commands tests.
  /// </summary>
  [TestFixture]
  public class CommandsTests
  {
    #region Public Properties

    /// <summary>
    ///   Gets the manager.
    /// </summary>
    public Station Manager { get; private set; }

    /// <summary>
    ///   Gets the new peer.
    /// </summary>
    public Station NewPeer { get; private set; }

    /// <summary>
    ///   Gets the station.
    /// </summary>
    public Station Station { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   The add peer command test.
    /// </summary>
    [Test]
    public void AddPeerCommandTest()
    {
      var cmd = new AddPeerCommand(this.Manager.Address, 
        this.Station.Address, new AsymmetricKey(this.Station.Crypto.KeyPair.Public));
      Assert.That(cmd.Sender == this.Manager.Address);

      // Manager sending to station, should work
      Assert.That(!this.NewPeer.Peers.ContainsKey(this.Station.Address));
      cmd.Execute(this.NewPeer);
      Assert.That(this.NewPeer.Peers.ContainsKey(this.Station.Address));

      // Station sending to manager, shouldn't work.
      cmd = new AddPeerCommand(this.NewPeer.Address, 
        this.NewPeer.Address, new AsymmetricKey(this.NewPeer.Crypto.KeyPair.Public));
      Assert.That(!this.Manager.Peers.ContainsKey(this.NewPeer.Address));
      cmd.Execute(this.Manager);
      Assert.That(!this.Manager.Peers.ContainsKey(this.NewPeer.Address));
    }

    /// <summary>
    ///   The ballot received command and revoke ballot command test.
    /// </summary>
    [Test]
    public void BallotReceivedCommandAndRevokeBallotCommandTest()
    {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);

      Assert.That(this.Station.Database[vn] == BallotStatus.Unavailable);

      this.Station.Database.Import(
        new List<EncryptedVoterData>
          {
            new EncryptedVoterData(
              new CipherText(
                this.Station.Crypto.AsymmetricEncrypt(
                  Bytes.From(vn.Value), 
                  this.Station.Crypto.VoterDataEncryptionKey)), 
              new CipherText(
                this.Station.Crypto.AsymmetricEncrypt(
                  Bytes.From(cpr.Value), 
                  this.Station.Crypto.VoterDataEncryptionKey)),
              new CipherText(
                this.Station.Crypto.AsymmetricEncrypt(
                  Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), 
                  this.Station.Crypto.VoterDataEncryptionKey)))
          });

      var cmd = new BallotReceivedCommand(this.Manager.Address, 
        this.Station.Address, vn, cpr);
      Assert.That(cmd.Sender == this.Manager.Address);
      Assert.That(this.Station.Database[vn] == BallotStatus.NotReceived);
      cmd.Execute(this.Station);
      Assert.That(this.Station.Database[vn] == BallotStatus.Received);

      var revoke = new RevokeBallotCommand(this.Manager.Address, vn, cpr);
      revoke.Execute(this.Station);
      Assert.That(this.Station.Database[vn] == BallotStatus.NotReceived);
    }

    /// <summary>
    ///   The ballot request denied test.
    /// </summary>
    [Test]
    public void BallotRequestDeniedTest()
    {
      var cmd = new BallotRequestDeniedCommand(this.Manager.Address);
      Assert.That(cmd.Sender.Equals(this.Manager.Address));
      cmd.Execute(this.Station);
      Assert.That(!((TestUi)this.Manager.UI).HandOutBallot);
      cmd = new BallotRequestDeniedCommand(this.Station.Address);
      cmd.Execute(this.Station);
    }

    /// <summary>
    ///   The crypto command test.
    /// </summary>
    [Test]
    public void CryptoCommandTest()
    {
      var cmd = new CryptoCommand(this.Manager, 
        this.Station.Address, new StartElectionCommand(this.Manager.Address));
      Assert.That(cmd.Sender == this.Manager.Address);
      Assert.That(!this.Station.ElectionInProgress);
      cmd.Execute(this.Station);
      Assert.That(this.Station.ElectionInProgress);

      // Station sending to NewPeer
      this.NewPeer.RemovePeer(this.Manager.Address);
      cmd = new CryptoCommand(this.Station, 
        this.NewPeer.Address, new StartElectionCommand(this.Station.Address));
      Assert.That(!this.NewPeer.ElectionInProgress);
      Assert.Throws<TheOnlyException>(() => cmd.Execute(this.NewPeer));
      Assert.That(!this.NewPeer.ElectionInProgress);
    }

    /// <summary>
    ///   The elect new manager command test.
    /// </summary>
    [Test]
    public void ElectNewManagerCommandTest()
    {
      var cmd = new ElectNewManagerCommand(this.Station.Address);
      this.NewPeer.AddPeer(this.Station.Address, 
        new AsymmetricKey(this.Station.Crypto.KeyPair.Public));
      this.Manager.StopListening();
      cmd.Execute(this.NewPeer);
      Assert.That(this.NewPeer.Manager.Equals(this.Station.Address));
    }

    /// <summary>
    ///   The is alive command test.
    /// </summary>
    [Test]
    public void IsAliveCommandTest()
    {
      var cmd = new IsAliveCommand(this.Station.Address);
      Assert.That(cmd.Sender.Equals(this.Station.Address));
    }

    /// <summary>
    ///   The manager requirement check test.
    /// </summary>
    [Test]
    public void ManagerRequirementCheckTest()
    {
      var start = new StartElectionCommand(this.NewPeer.Address);
      var end = new EndElectionCommand(this.NewPeer.Address);
      Assert.That(!((TestUi)this.Station.UI).OngoingElection);
      start.Execute(this.Station);
      Assert.That(!((TestUi)this.Station.UI).OngoingElection);
      this.Station.StartElection();
      Assert.That(this.Station.ElectionInProgress);
      end.Execute(this.Station);
      Assert.That(this.Station.ElectionInProgress);

      var vn = new VoterNumber(5);
      var cpr = new CPR(5);
      var req = new RequestBallotCommand(this.NewPeer.Address, vn);
      var reqCprOnly = 
        new RequestBallotCPROnlyCommand(this.NewPeer.Address, cpr, 
          SystemTestData.Password);
      var revoke = new RevokeBallotCommand(this.NewPeer.Address, vn, cpr);
      var revokeCprOnly = 
        new RevokeBallotCPROnlyCommand(this.NewPeer.Address, cpr, 
          SystemTestData.Password);
      req.Execute(this.Station);
      reqCprOnly.Execute(this.Station);
      revoke.Execute(this.Station);
      revokeCprOnly.Execute(this.Station);
      Assert.That(this.Station.Database[vn] == BallotStatus.Unavailable);

      this.NewPeer.Crypto.VoterDataEncryptionKey = 
        this.Manager.Crypto.VoterDataEncryptionKey;
      var sync = new SyncCommand(this.NewPeer);
      sync.Execute(this.Station);

      var promoteManager = 
        new PromoteNewManagerCommand(this.NewPeer.Address, this.NewPeer.Address);
      promoteManager.Execute(this.Station);
      Assert.That(!this.Station.Manager.Equals(this.NewPeer.Address));
    }

    /// <summary>
    ///   The public key exchange command test.
    /// </summary>
    [Test]
    public void PublicKeyExchangeCommandTest()
    {
      var ui = new TestUi();
      using (
        var manager = new Station(
          ui, 
          SystemTestData.Key, 
          SystemTestData.Password, 
          SystemTestData.ManagerPort, 
          "CommandsTestPublicKeyExchangeCommandTestManagerVoters.sqlite", 
          "CommandsTestPublicKeyExchangeCommandTestManagerLog.sqlite"))
      using (
        var receiver = new Station(
          ui, SystemTestData.StationPort, 
          "CommandsTestPublicKeyExchangeCommandTestReceiverVoters.sqlite")) {
        var cmd = new PublicKeyExchangeCommand(manager);
        Assert.That(cmd.Sender.Equals(manager.Address));
        Assert.That(!receiver.Peers.ContainsKey(manager.Address));
        Assert.Null(receiver.Manager);
        cmd.Execute(receiver);
        Assert.That(receiver.Peers.ContainsKey(manager.Address));
        Assert.That(receiver.Manager.Equals(manager.Address));
      }

      File.Delete("CommandsTestPublicKeyExchangeCommandTestManagerVoters.sqlite");
      File.Delete("CommandsTestPublicKeyExchangeCommandTestManagerLog.sqlite");
      File.Delete("CommandsTestPublicKeyExchangeCommandTestReceiverVoters.sqlite");
    }

    /// <summary>
    ///   The request ballot commands test.
    /// </summary>
    [Test]
    public void RequestBallotCommandsTest()
    {
      var ui = (TestUi)this.Manager.UI;
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      var cmd = new RequestBallotCommand(this.Station.Address, vn);
      var pswdcmd = new RequestBallotCPROnlyCommand(
        this.Station.Address, cpr, SystemTestData.Password);
      var data =
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(this.Station.Crypto.AsymmetricEncrypt(
                             Bytes.From(vn.Value), 
                             this.Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(this.Station.Crypto.AsymmetricEncrypt(
                           Bytes.From(cpr.Value), 
                           this.Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(this.Station.Crypto.AsymmetricEncrypt(
                           Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), 
                           this.Station.Crypto.VoterDataEncryptionKey)))
        };
      this.Manager.Database.Import(data);
      this.Station.Database.Import(data);
      this.Station.MasterPassword = this.Manager.MasterPassword;
      this.Manager.BallotReceived(vn);
      cmd.Execute(this.Manager);
      Assert.That(!ui.HandOutBallot);
      pswdcmd.Execute(this.Manager);
      Assert.That(!ui.HandOutBallot);

      this.Manager.RevokeBallot(vn);

      cmd = new RequestBallotCommand(this.Manager.Address, vn);
      pswdcmd = new RequestBallotCPROnlyCommand(this.Manager.Address, 
        cpr, SystemTestData.Password);
      cmd.Execute(this.Manager);
      Assert.That(ui.HandOutBallot);
      this.Manager.RevokeBallot(vn);
      pswdcmd.Execute(this.Manager);
      Assert.That(ui.HandOutBallot);
    }

    /// <summary>
    ///   Create a new UI, Manager, Station, and joining peer Station along 
    /// with their respective DBs.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
      var ui = new TestUi();
      this.Manager = new Station(
        ui, SystemTestData.Key, 
        SystemTestData.Password, 
        SystemTestData.ManagerPort, 
        "CommandsTestsManagerTestVoters.sqlite",  
        "CommandsTestsManagerLogTestDb.sqlite");
      this.NewPeer = new Station(
        ui, SystemTestData.PeerPort, 
        "CommandsTestsManagerPeerTestDb.sqlite");

      this.Manager.Manager = this.Manager.Address;
      this.Station.Manager = this.Manager.Address;
      this.NewPeer.Manager = this.Manager.Address;

      this.Manager.AddPeer(this.Station.Address, 
        new AsymmetricKey(this.Station.Crypto.KeyPair.Public));

      this.Station.AddPeer(this.Manager.Address, 
        new AsymmetricKey(this.Manager.Crypto.KeyPair.Public));
      this.Station.AddPeer(this.NewPeer.Address, 
        new AsymmetricKey(this.NewPeer.Crypto.KeyPair.Public));
      this.Station.Crypto.VoterDataEncryptionKey = 
        this.Manager.Crypto.VoterDataEncryptionKey;

      this.NewPeer.AddPeer(this.Manager.Address, 
        new AsymmetricKey(this.Manager.Crypto.KeyPair.Public));
    }

    /// <summary>
    ///   The shut down election command test.
    /// </summary>
    [Test]
    public void ShutDownElectionCommandTest()
    {
      var cmd = new ShutDownElectionCommand(this.Manager.Address);
      Assert.That(cmd.Sender == this.Manager.Address);
      Assert.Throws<TheOnlyException>(() => cmd.Execute(this.Station));
    }

    /// <summary>
    ///   Clean up the test Manager, Station, and Peer and their DBs.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
      if (this.Manager != null) this.Manager.Dispose();
      if (this.Station != null) this.Station.Dispose();
      if (this.NewPeer != null) this.NewPeer.Dispose();
      this.Manager = null;
      this.Station = null;
      this.NewPeer = null;
      File.Delete("CommandsTestsManagerTestVoters.sqlite");
      File.Delete("CommandsTestsManagerLogTestDb.sqlite");
      File.Delete("CommandsTestsManagerPeerTestDb.sqlite");
    }

    #endregion
  }
}