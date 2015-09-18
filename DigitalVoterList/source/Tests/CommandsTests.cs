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
      var cmd = new AddPeerCommand(Manager.Address, 
        Station.Address, new AsymmetricKey(Station.Crypto.KeyPair.Public));
      Assert.That(cmd.Sender == Manager.Address);

      // Manager sending to station, should work
      Assert.That(!NewPeer.Peers.ContainsKey(Station.Address));
      cmd.Execute(NewPeer);
      Assert.That(NewPeer.Peers.ContainsKey(Station.Address));

      // Station sending to manager, shouldn't work.
      cmd = new AddPeerCommand(NewPeer.Address, 
        NewPeer.Address, new AsymmetricKey(NewPeer.Crypto.KeyPair.Public));
      Assert.That(!Manager.Peers.ContainsKey(NewPeer.Address));
      cmd.Execute(Manager);
      Assert.That(!Manager.Peers.ContainsKey(NewPeer.Address));
    }

    /// <summary>
    ///   The ballot received command and revoke ballot command test.
    /// </summary>
    /*
    [Test]
    public void BallotReceivedCommandAndRevokeBallotCommandTest()
    {
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);

      Assert.That(Station.Database[vn] == BallotStatus.Unavailable);

      Station.Database.Import(
        new List<EncryptedVoterData>
          {
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
                  Station.Crypto.VoterDataEncryptionKey)))
          });

      var cmd = new BallotReceivedCommand(Manager.Address, 
        Station.Address, vn, cpr);
      Assert.That(cmd.Sender == Manager.Address);
      Assert.That(Station.Database[vn] == BallotStatus.NotReceived);
      cmd.Execute(Station);
      Assert.That(Station.Database[vn] == BallotStatus.Received);

      var revoke = new RevokeBallotCommand(Manager.Address, vn, cpr);
      revoke.Execute(Station);
      Assert.That(Station.Database[vn] == BallotStatus.NotReceived);
    }
      */
    /// <summary>
    ///   The ballot request denied test.
    /// </summary>
    [Test]
    public void BallotRequestDeniedTest()
    {
      var cmd = new BallotRequestDeniedCommand(Manager.Address, new VoterNumber(0), VoterStatus.NotSeenToday, VoterStatus.ActiveVoter);
      Assert.That(cmd.Sender.Equals(Manager.Address));
      cmd.Execute(Station);
      Assert.That(!((TestUi)Manager.UI).HandOutBallot);
      cmd = new BallotRequestDeniedCommand(Station.Address, new VoterNumber(0), VoterStatus.NotSeenToday, VoterStatus.ActiveVoter);
      cmd.Execute(Station);
    }

    /// <summary>
    ///   The crypto command test.
    /// </summary>
    [Test]
    public void CryptoCommandTest()
    {
      var cmd = new CryptoCommand(Manager, 
        Station.Address, new StartElectionCommand(Manager.Address));
      Assert.That(cmd.Sender == Manager.Address);
      Assert.That(!Station.ElectionInProgress);
      cmd.Execute(Station);
      Assert.That(Station.ElectionInProgress);

      // Station sending to NewPeer
      NewPeer.RemovePeer(Manager.Address, false);
      cmd = new CryptoCommand(Station, 
        NewPeer.Address, new StartElectionCommand(Station.Address));
      Assert.That(!NewPeer.ElectionInProgress);
      Assert.Throws<TheOnlyException>(() => cmd.Execute(NewPeer));
      Assert.That(!NewPeer.ElectionInProgress);
    }

    /// <summary>
    ///   The elect new manager command test.
    /// </summary>
    [Test]
    public void ElectNewManagerCommandTest()
    {
      var cmd = new ElectNewManagerCommand(Station.Address);
      NewPeer.AddPeer(Station.Address, 
        new AsymmetricKey(Station.Crypto.KeyPair.Public));
      Manager.StopListening();
      cmd.Execute(NewPeer);
      Assert.That(NewPeer.Manager.Equals(Station.Address));
    }

    /// <summary>
    ///   The is alive command test.
    /// </summary>
    [Test]
    public void IsAliveCommandTest()
    {
      var cmd = new IsAliveCommand(Station.Address);
      Assert.That(cmd.Sender.Equals(Station.Address));
    }

    /// <summary>
    ///   The manager requirement check test.
    /// </summary>
    /*
    [Test]
    public void ManagerRequirementCheckTest()
    {
      var start = new StartElectionCommand(NewPeer.Address);
      var end = new EndElectionCommand(NewPeer.Address);
      Assert.That(!((TestUi)Station.UI).OngoingElection);
      start.Execute(Station);
      Assert.That(!((TestUi)Station.UI).OngoingElection);
      Station.StartElection();
      Assert.That(Station.ElectionInProgress);
      end.Execute(Station);
      Assert.That(Station.ElectionInProgress);

      var vn = new VoterNumber(5);
      var cpr = new CPR(5);
      var req = new RequestBallotCommand(NewPeer.Address, vn);
      var reqCprOnly = 
        new RequestBallotCPROnlyCommand(NewPeer.Address, cpr, 
          SystemTestData.Password);
      var revoke = new RevokeBallotCommand(NewPeer.Address, vn, cpr);
      var revokeCprOnly = 
        new RevokeBallotCPROnlyCommand(NewPeer.Address, cpr, 
          SystemTestData.Password);
      req.Execute(Station);
      reqCprOnly.Execute(Station);
      revoke.Execute(Station);
      revokeCprOnly.Execute(Station);
      Assert.That(Station.Database[vn] == BallotStatus.Unavailable);

      NewPeer.Crypto.VoterDataEncryptionKey = 
        Manager.Crypto.VoterDataEncryptionKey;
      var sync = new SyncCommand(NewPeer);
      sync.Execute(Station);

      var promoteManager = 
        new PromoteNewManagerCommand(NewPeer.Address, NewPeer.Address);
      promoteManager.Execute(Station);
      Assert.That(!Station.Manager.Equals(NewPeer.Address));
    }
      */

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
        var cmd = new PublicKeyExchangeCommand(manager, receiver.Address);
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
    /*
    [Test]
    public void RequestBallotCommandsTest()
    {
      var ui = (TestUi)Manager.UI;
      var vn = new VoterNumber(250000);
      var cpr = new CPR(2312881234);
      var cmd = new RequestBallotCommand(Station.Address, vn);
      var pswdcmd = new RequestBallotCPROnlyCommand(
        Station.Address, cpr, SystemTestData.Password);
      var data =
        new List<EncryptedVoterData> {
          new EncryptedVoterData(
            new CipherText(Station.Crypto.AsymmetricEncrypt(
                             Bytes.From(vn.Value), 
                             Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(Station.Crypto.AsymmetricEncrypt(
                           Bytes.From(cpr.Value), 
                           Station.Crypto.VoterDataEncryptionKey)), 
            new CipherText(Station.Crypto.AsymmetricEncrypt(
                           Bytes.From(cpr.Value + (uint)BallotStatus.NotReceived), 
                           Station.Crypto.VoterDataEncryptionKey)))
        };
      Manager.Database.Import(data);
      Station.Database.Import(data);
      Station.MasterPassword = Manager.MasterPassword;
      Manager.BallotReceived(vn);
      cmd.Execute(Manager);
      Assert.That(!ui.HandOutBallot);
      pswdcmd.Execute(Manager);
      Assert.That(!ui.HandOutBallot);

      Manager.RevokeBallot(vn);

      cmd = new RequestBallotCommand(Manager.Address, vn);
      pswdcmd = new RequestBallotCPROnlyCommand(Manager.Address, 
        cpr, SystemTestData.Password);
      cmd.Execute(Manager);
      Assert.That(ui.HandOutBallot);
      Manager.RevokeBallot(vn);
      pswdcmd.Execute(Manager);
      Assert.That(ui.HandOutBallot);
    }
      */

    /// <summary>
    ///   Create a new UI, Manager, Station, and joining peer Station along 
    /// with their respective DBs.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
      var ui = new TestUi();
      Manager = new Station(
        ui, SystemTestData.Key, 
        SystemTestData.Password, 
        SystemTestData.ManagerPort, 
        "CommandsTestsManagerTestVoters.sqlite",  
        "CommandsTestsManagerLogTestDb.sqlite");
      NewPeer = new Station(
        ui, SystemTestData.PeerPort, 
        "CommandsTestsManagerPeerTestDb.sqlite");

      Manager.Manager = Manager.Address;
      Station.Manager = Manager.Address;
      NewPeer.Manager = Manager.Address;

      Manager.AddPeer(Station.Address, 
        new AsymmetricKey(Station.Crypto.KeyPair.Public));

      Station.AddPeer(Manager.Address, 
        new AsymmetricKey(Manager.Crypto.KeyPair.Public));
      Station.AddPeer(NewPeer.Address, 
        new AsymmetricKey(NewPeer.Crypto.KeyPair.Public));
      Station.Crypto.VoterDataEncryptionKey = 
        Manager.Crypto.VoterDataEncryptionKey;

      NewPeer.AddPeer(Manager.Address, 
        new AsymmetricKey(Manager.Crypto.KeyPair.Public));
    }

    /// <summary>
    ///   The shut down election command test.
    /// </summary>
    [Test]
    public void ShutDownElectionCommandTest()
    {
      var cmd = new ShutDownElectionCommand(Manager.Address);
      Assert.That(cmd.Sender == Manager.Address);
      Assert.Throws<TheOnlyException>(() => cmd.Execute(Station));
    }

    /// <summary>
    ///   Clean up the test Manager, Station, and Peer and their DBs.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
      if (Manager != null) Manager.Dispose();
      if (Station != null) Station.Dispose();
      if (NewPeer != null) NewPeer.Dispose();
      Manager = null;
      Station = null;
      NewPeer = null;
      File.Delete("CommandsTestsManagerTestVoters.sqlite");
      File.Delete("CommandsTestsManagerLogTestDb.sqlite");
      File.Delete("CommandsTestsManagerPeerTestDb.sqlite");
    }

    #endregion
  }
}