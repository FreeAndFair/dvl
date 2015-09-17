#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="CommunicatorTests.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Threading;

  using Aegis_DVL;
  using Aegis_DVL.Commands;
  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Logging;
  using Aegis_DVL.Util;

  using NUnit.Framework;

  using Enumerable = System.Linq.Enumerable;

  /// <summary>
  ///   Test the communicator.
  /// </summary>
  [TestFixture] public class CommunicatorTests {
    #region Fields

    /// <summary>
    /// The _timer.
    /// </summary>
    private Stopwatch _timer;

    #endregion

    #region Delegates

    /// <summary>
    /// The receiver listener.
    /// </summary>
    public delegate void ReceiverListener();

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the bad end point.
    /// </summary>
    public IPEndPoint BadEndPoint { get; private set; }

    /// <summary>
    /// Gets the receiver.
    /// </summary>
    public Station Receiver { get; private set; }

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public Station Sender { get; private set; }

    #endregion

    #region Public Methods and Operators

      /*
    /// <summary>
    /// The big command send and receive test.
    /// </summary>
    [Test] public void BigCommandSendAndReceiveTest() {
      IEnumerable<Tuple<string, string, uint, uint>> voters = Enumerable.Range(0, 500).Select(
        x =>
        new Tuple<string, string, uint, uint>(
          "Bob Bobbersen nummer " + x, "Køge Sportshal", (uint)x + 250000, (uint)x + 2312881234));

      ICrypto crypto = this.Sender.Crypto;
      AsymmetricKey key = crypto.VoterDataEncryptionKey;

      IEnumerable<EncryptedVoterData> encryptedVoters = voters.Select(
        v =>
        new EncryptedVoterData(
          new CipherText(crypto.AsymmetricEncrypt(Bytes.From(v.Item3), key)), 
          new CipherText(crypto.AsymmetricEncrypt(Bytes.From(v.Item4), key)), 
          new CipherText(crypto.AsymmetricEncrypt(Bytes.From((uint)BallotStatus.NotReceived), key))));
      this.Sender.Database.Import(encryptedVoters);

      using (
        var receiver = new Station(new TestUi(), 62347, "BigCommandSendAndReceiveTestVoters.sqlite") {
          Manager = this.Sender.Address
        }) {
        receiver.StopListening();
        this.Sender.AddPeer(receiver.Address, new AsymmetricKey(receiver.Crypto.KeyPair.Public));
        receiver.AddPeer(this.Sender.Address, new AsymmetricKey(this.Sender.Crypto.KeyPair.Public));
        var receiverListener = new ReceiverListener(receiver.Communicator.ReceiveAndHandle);
        IAsyncResult receiverResult = receiverListener.BeginInvoke(null, null);

        Assert.That(!receiver.Database.AllData.Any());
        var sync = new SyncCommand(this.Sender);
        Console.WriteLine(Bytes.From(sync).Length);
        this.Sender.Communicator.Send(sync, receiver.Address);
        receiverListener.EndInvoke(receiverResult);

        Assert.That(receiver.Database.AllData.Count() == 500);
      }

      File.Delete("BigCommandSendAndReceiveTestVoters.sqlite");
    }
      */

    /// <summary>
    /// The discover network machines test.
    /// </summary>
    [Test] public void DiscoverNetworkMachinesTest() {
      IEnumerable<IPEndPoint> machines = this.Sender.Communicator.DiscoverPeers();
      Assert.That(machines != null);
      machines = machines.ToArray();
      int count = 0;
      foreach (IPEndPoint machine in machines) {
        count++;
        Console.WriteLine(machine);
      }

      Assert.That(count >= 0);
    }

    /// <summary>
    /// The is listening test.
    /// </summary>
    [Test] public void IsListeningTest() {
      Assert.That(!this.Sender.Communicator.IsListening(this.Sender.Address));
      this.Sender.StartListening();

      // Waste some CPU time while the thread hopefully starts...
      int c = 0;
      while (c < 500000) c++;
      Console.WriteLine(c);
      Assert.That(this.Sender.Communicator.IsListening(this.Sender.Address));
    }

    /// <summary>
    /// The receiver failure test.
    /// </summary>
    [Test] public void ReceiverFailureTest() {
      var ui = new TestUi();
      using (
        var manager = new Station(
          ui, 
          SystemTestData.Key, 
          SystemTestData.Password, 
          SystemTestData.ManagerPort, 
          "CommunicatorTestsReceiverFailureTestManagerVoters.sqlite", 
          "CommunicatorTestsReceiverFailureTestManagerLog.sqlite")) {
        byte[] pswd =
          manager.Crypto.Hash(Bytes.From(SystemTestData.Password));
        using (var peer1 = new Station(ui, SystemTestData.StationPort, 
            "CommunicatorTestsReceiverFailureTestPeer1Voters.sqlite") {
          Manager = manager.Address, 
          MasterPassword = pswd, 
          Crypto = { VoterDataEncryptionKey = manager.Crypto.VoterDataEncryptionKey }
        })
        using (var peer2 = new Station(ui, SystemTestData.StationPort + 1, 
            "CommunicatorTestsReceiverFailureTestPeer2Voters.sqlite") {
          Manager = manager.Address, 
          MasterPassword = pswd, 
          Crypto = { VoterDataEncryptionKey = manager.Crypto.VoterDataEncryptionKey }
        })
        using (var peer3 = new Station(ui, SystemTestData.StationPort + 2, 
          "CommunicatorTestsReceiverFailureTestPeer3Voters.sqlite") {
          Manager = manager.Address, 
          MasterPassword = pswd, 
          Crypto = { VoterDataEncryptionKey = manager.Crypto.VoterDataEncryptionKey }
        }) {
          peer1.Logger = new Logger(peer1, "CommunicatorTestsReceiverFailureTestPeer1log.sqlite");
          peer2.Logger = new Logger(peer2, "CommunicatorTestsReceiverFailureTestPeer2log.sqlite");
          peer3.Logger = new Logger(peer3, "CommunicatorTestsReceiverFailureTestPeer3log.sqlite");
          peer1.StopListening();

          manager.AddPeer(peer1.Address, new AsymmetricKey(peer1.Crypto.KeyPair.Public));
          manager.AddPeer(peer2.Address, new AsymmetricKey(peer2.Crypto.KeyPair.Public));
          manager.AddPeer(peer3.Address, new AsymmetricKey(peer3.Crypto.KeyPair.Public));

          peer1.AddPeer(peer2.Address, new AsymmetricKey(peer2.Crypto.KeyPair.Public));
          peer1.AddPeer(peer3.Address, new AsymmetricKey(peer3.Crypto.KeyPair.Public));
          peer1.AddPeer(manager.Address, new AsymmetricKey(manager.Crypto.KeyPair.Public));

          peer2.AddPeer(peer1.Address, new AsymmetricKey(peer1.Crypto.KeyPair.Public));
          peer2.AddPeer(peer3.Address, new AsymmetricKey(peer3.Crypto.KeyPair.Public));
          peer2.AddPeer(manager.Address, new AsymmetricKey(manager.Crypto.KeyPair.Public));

          peer3.AddPeer(peer1.Address, new AsymmetricKey(peer1.Crypto.KeyPair.Public));
          peer3.AddPeer(peer2.Address, new AsymmetricKey(peer2.Crypto.KeyPair.Public));
          peer3.AddPeer(manager.Address, new AsymmetricKey(manager.Crypto.KeyPair.Public));

          Assert.That(manager.IsManager);
          Assert.That(!peer1.IsManager);
          Assert.That(!peer2.IsManager);
          Assert.That(!peer3.IsManager);

          Assert.That(peer2.Peers.ContainsKey(peer1.Address) && !peer2.ElectionInProgress);
          Assert.That(peer3.Peers.ContainsKey(peer1.Address) && !peer3.ElectionInProgress);
          Assert.That(manager.Peers.ContainsKey(peer1.Address) && !manager.ElectionInProgress);

          manager.Communicator.Send(new StartElectionCommand(manager.Address), peer1.Address);

          Thread.Sleep(5000);
          Assert.That(!peer2.Peers.ContainsKey(peer1.Address));
          Assert.That(!peer3.Peers.ContainsKey(peer1.Address));
          Assert.That(!manager.Peers.ContainsKey(peer1.Address));
        }
      }

      File.Delete("CommunicatorTestsReceiverFailureTestManagerVoters.sqlite");
      File.Delete("CommunicatorTestsReceiverFailureTestPeer1Voters.sqlite");
      File.Delete("CommunicatorTestsReceiverFailureTestPeer2Voters.sqlite");
      File.Delete("CommunicatorTestsReceiverFailureTestPeer3Voters.sqlite");
      File.Delete("CommunicatorTestsReceiverFailureTestManagerLog.sqlite");
    }

    /// <summary>
    ///   Test whether the Send and ReceiveAndHandle methods works.
    /// </summary>
    [Test] public void SendAndReceiveAndHandleTest() {
      this.Sender.AddPeer(this.Receiver.Address, 
        new AsymmetricKey(this.Receiver.Crypto.KeyPair.Public));
      /*
      var receiver = new ReceiverListener(this.Receiver.Communicator.NetworkReceiveThread);

      // Test whether the system is able to send and receive a basic command
      IAsyncResult receiverResult = receiver.BeginInvoke(null, null);

      // Waste some CPU time while the thread hopefully starts...
      int c = 0;
      while (c < 500000) c++;
      Console.WriteLine(c);
      Assert.That(this.Sender.StationActive(this.Receiver.Address));
      receiver.EndInvoke(receiverResult);

      receiverResult = receiver.BeginInvoke(null, null);

      this.Sender.Communicator.Send(new EndElectionCommand(this.Sender.Address), this.Receiver.Address);
      IAsyncResult result = receiverResult;
      Assert.Throws<TheOnlyException>(() => receiver.EndInvoke(result));

      receiverResult = receiver.BeginInvoke(null, null);
      using (var client = new TcpClient()) {
        client.Connect(this.Receiver.Address);
        using (NetworkStream stream = client.GetStream()) {
          byte[] msg = Bytes.From(new EndElectionCommand(this.Sender.Address));
          stream.Write(msg, 0, msg.Length);
        }
      }

      Assert.Throws<TheOnlyException>(() => receiver.EndInvoke(receiverResult));

      // Send a command that will be wrapped in a CryptoCommand that wont be received, and that the receiver is removed from the peer-list
      Assert.That(this.Sender.Peers.ContainsKey(this.Receiver.Address));
      this.Sender.Communicator.Send(new ElectNewManagerCommand(this.Sender.Address), this.Receiver.Address);
      Assert.That(!this.Sender.Peers.ContainsKey(this.Receiver.Address));

      // Test bad endpoint
      Assert.That(!this.Sender.StationActive(this.BadEndPoint));
       * */
    }

    /// <summary>
    ///   SetUp test helper properties.
    /// </summary>
    [SetUp] public void SetUp() {
      var ui = new TestUi();
      this.Sender = new Station(
        ui, SystemTestData.Key, SystemTestData.Password, SystemTestData.StationPort, 
        "CommunicatorTestsSenderVoters.sqlite", "CommunicatorTestsSenderLog.sqlite");
      this.Receiver = new Station(ui, SystemTestData.PeerPort, "CommunicatorTestsReceiverVoters.sqlite") {
        Manager = this.Sender.Address, 
        Crypto = { VoterDataEncryptionKey = this.Sender.Crypto.VoterDataEncryptionKey }, 
        MasterPassword =
          this.Sender.Crypto.Hash(Bytes.From(SystemTestData.Password))
      };

      // Receiver.Logger = new Logger(Receiver, "CommunicatorTestsReceiverLog.sqlite");
      this.Sender.StopListening();
      this.Receiver.StopListening();

      this.BadEndPoint = new IPEndPoint(this.Receiver.Address.Address, this.Receiver.Address.Port + 5);
      this._timer = new Stopwatch();
      this._timer.Start();
    }

    /// <summary>
    /// The tear down.
    /// </summary>
    [TearDown] public void TearDown() {
      Console.WriteLine("Test duration: {0}", this._timer.ElapsedMilliseconds);
      this._timer = null;
      this.Sender.Dispose();
      this.Receiver.Dispose();
      this.Sender = null;
      this.Receiver = null;
      try {
        File.Delete("CommunicatorTestsSenderVoters.sqlite");
        File.Delete("CommunicatorTestsReceiverVoters.sqlite");
        File.Delete("CommunicatorTestsSenderLog.sqlite");
        File.Delete("CommunicatorTestsReceiverLog.sqlite");
      } catch (IOException e) {
        Console.WriteLine(e.Message);
        Console.WriteLine(e.StackTrace);
      }
    }

    #endregion
  }
}
