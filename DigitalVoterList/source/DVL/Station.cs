﻿#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Station.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.Contracts;
  using System.IO;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;
  using System.Threading;

  using Aegis_DVL.Commands;
  using Aegis_DVL.Communication;
  using Aegis_DVL.Cryptography;
  using Aegis_DVL.Database;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Logging;
  using Aegis_DVL.Util;

  /// <summary>
  ///   A station is a client-machine that communicates with its manager, 
  /// and provides a graphical user interface for voters to use when 
  /// requesting a ballot.
  ///   A station can also be the manager. A manager manages the various 
  /// stations, and handles synchronization of the data.
  ///   It also has elevated rights compared to a station, and can for 
  /// example manually mark a voter as having been handed a ballot (in 
  /// case he lost his voter card, or the like).
  /// </summary>
  public class Station : IDisposable {
    #region Fields

    /// <summary>
    /// The _crypto.
    /// </summary>
    private ICrypto _crypto;

    /// <summary>
    /// The _is disposed.
    /// </summary>
    private bool _isDisposed;

    /// <summary>
    /// The _logger.
    /// </summary>
    private ILogger _logger;

    /// <summary>
    /// The _manager.
    /// </summary>
    private IPEndPoint _manager;

    /// <summary>
    /// The _master password.
    /// </summary>
    private byte[] _masterPassword;

    /// <summary>
    /// The database prefix.
    /// </summary>
    private string _dbPrefix;

    /// <summary>
    /// Are we currently using a master password at all?
    /// </summary>
    internal bool IsMasterPasswordInUse = false;

    private IPEndPoint PromotedStation = null;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Station"/> class. 
    /// Can I have a new Station that is the manager?
    /// </summary>
    /// <param name="ui">
    /// A reference to the UI.
    /// </param>
    /// <param name="voterDataEncryptionKey">
    /// The AsymmetricKey used for encrypting the data at this election venue.
    /// </param>
    /// <param name="masterPassword">
    /// The master password known only by the election secretary.
    /// </param>
    /// <param name="port">
    /// The network port the station is communicating over. Defaults to 62000.
    /// </param>
    /// <param name="dbPrefix">
    /// The prefix of the voter database prefix. Defaults to Voters.
    /// </param>
    /// <param name="logPrefix">
    /// The prefix of the logging database prefix. Defaults to Log.
    /// </param>
    public Station(IDvlUi ui, 
                   AsymmetricKey voterDataEncryptionKey, 
                   string masterPassword, 
                   bool local = false,
                   int port = 62000, 
                   string dbPrefix = "Voters", 
                   string logPrefix = "Log")
      : this(ui, local, port, dbPrefix) {
      Contract.Requires(ui != null);
      Contract.Requires(masterPassword != null);
      Contract.Requires(dbPrefix != null);
      Contract.Requires(logPrefix != null);

      Crypto = new Crypto(voterDataEncryptionKey);
      MasterPassword =
        Crypto.Hash(Bytes.From(masterPassword));
      Logger = new Logger(this, logPrefix);
      Database = new VoterListDatabase(this, _dbPrefix);
      Manager = Address;
      Logger.Log("Manager initialized", Level.Info);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Station"/> class. 
    /// Can I have a new Station that is the manager?
    /// </summary>
    /// <param name="ui">
    /// A reference to the UI.
    /// </param>
    /// <param name="keyPath">
    /// The path to the key-file.
    /// </param>
    /// <param name="masterPassword">
    /// The master password known only by the election secretary.
    /// </param>
    /// <param name="port">
    /// The network port the station is communicating over. Defaults to 62000.
    /// </param>
    /// <param name="dbPrefix">
    /// The prefix of the voter database prefix. Defaults to Voters.
    /// </param>
    /// <param name="logPrefix">
    /// The prefix of the logging database prefix. Defaults to Log.
    /// </param>
    
    public Station(IDvlUi ui, 
                   string keyPath, 
                   string masterPassword, 
                   bool local,
                   int port = 62000, 
                   string dbPrefix = "Voters", 
                   string logPrefix = "Log")
      : this(ui, 
        new AsymmetricKey(Bytes.FromFile(keyPath).ToKey()), 
        masterPassword, 
        local,
        port, 
        dbPrefix, 
        logPrefix) {
      Contract.Requires(ui != null);
      Contract.Requires(keyPath != null);
      Contract.Requires(File.Exists(keyPath));
      Contract.Requires(dbPrefix != null);
      Contract.Requires(logPrefix != null);
    }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="Station"/> class. 
    /// Can I have a new Station?
    /// </summary>
    /// <param name="ui">
    /// A reference to the UI.
    /// </param>
    /// <param name="port">
    /// The port the station should listen to. Defaults to 62000.
    /// </param>
    /// <param name="dbPrefix">
    /// The prefix of the voter database prefix.
    /// </param>
    public Station(IDvlUi ui, 
                   bool local = false,
                   int port = 62000, 
                   string dbPrefix = "Voters") {
      Contract.Requires(ui != null);
      Contract.Requires(dbPrefix != null);

      Peers = new SortedDictionary<IPEndPoint, AsymmetricKey>(new IPEndPointComparer());
      PeerStatuses = new SortedDictionary<IPEndPoint, StationStatus>(new IPEndPointComparer());
      ElectionInProgress = false;
      if (local) {
        Communicator = new LocalhostCommunicator(this);
      } else {
        Communicator = new InternetCommunicator(this);
      }
      Address = Communicator.GetLocalEndPoint(port);
      _dbPrefix = dbPrefix;
      UI = ui;
      Crypto = new Crypto();
      StartListening();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Station"/> class. 
    /// </summary>
    ~Station() { Dispose(false); }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What is my ip address?
    /// </summary>
    public IPEndPoint Address { [Pure] get; private set; }

    /// <summary>
    ///   How can I communicate with my group?
    /// </summary>
    public ICommunicator Communicator { [Pure] get; private set; }

    /// <summary>
    ///   How can I encrypt messages?
    ///   This is how you encrypt messages!
    /// </summary>
    public ICrypto Crypto {
      [Pure] get { return _crypto; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(Equals(Crypto, value));
        _crypto = value;
      }
    }

    public PollingPlace PollingPlace { [Pure] get; set; }

    /// <summary>
    ///   How can I manipulate my database?
    /// </summary>
    public IDatabase Database { [Pure] get; private set; }

    /// <summary>
    ///   What is the status of the election?
    /// </summary>
    public bool ElectionInProgress { [Pure] get; private set; }

    public bool PromotionInProgress { [Pure] get; private set; }

    /// <summary>
    ///   Is there enough active stations in the group to continue operations?
    /// </summary>
    public bool EnoughStations { [Pure] get {
      return Peers.Keys.Count(StationActive) > 0; 
      // TODO: Correct to '>' when not testing
    }}

    /// <summary>
    ///   Am I the manager?
    /// </summary>
    public bool IsManager { [Pure] get {
      return Address.Equals(Manager);
    }}

    /// <summary>
    /// Gets a value indicating whether listening.
    /// </summary>
    public bool Listening { get; private set; }

    /// <summary>
    ///   How can I log messages?
    ///   This is how you log messages!
    /// </summary>
    public ILogger Logger {
      [Pure] get { return _logger; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(Equals(Logger, value));
        _logger = value;
      }
    }

    /// <summary>
    ///   Who is the manager?
    ///   This station is now the manager!
    /// </summary>
    public IPEndPoint Manager {
      [Pure] get { return _manager; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(Manager.Equals(value));
        _manager = value;
      }
    }

    /// <summary>
    ///   What is the master password?
    ///   The master password is this!
    /// </summary>
    public byte[] MasterPassword {
      get { return _masterPassword; }
      set {
        Contract.Requires(value != null);
        Contract.Requires(MasterPassword == null);
        Contract.Ensures(Equals(MasterPassword, value));
        _masterPassword = value;
        value.ToFile("Master.pw");
      }
    }

    /// <summary>
    ///   Who are my peers?
    /// </summary>
    public SortedDictionary<IPEndPoint, AsymmetricKey> Peers {
      [Pure] get; private set;
    }

    public SortedDictionary<IPEndPoint, StationStatus> PeerStatuses {
      [Pure] get; private set;
    }

    /// <summary>
    ///   How can the user interact with me?
    /// </summary>
    public IDvlUi UI { [Pure] get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Add this station to the group!
    /// </summary>
    /// <param name="peer">
    /// The address of the station to add.
    /// </param>
    /// <param name="peerPublicAsymmetricKey">
    /// The public AsymmetricKey of the station to add.
    /// </param>
    public void AddPeer(IPEndPoint peer, AsymmetricKey peerPublicAsymmetricKey) {
      Contract.Requires(peer != null);
      Contract.Requires(!Peers.ContainsKey(peer));
      Contract.Ensures(Peers.ContainsKey(peer));
      Peers.Add(peer, peerPublicAsymmetricKey);
      if (PeerStatuses.ContainsKey(peer)) {
        PeerStatuses[peer].ConnectionState = "Connected";
      } else {
        PeerStatuses.Add(peer, new StationStatus(peer, Communicator.GetIdentifyingStringForStation(peer), "Connected"));
      }
      if (EnoughStations) UI.EnoughPeers();
      if (Logger != null) Logger.Log("Peer added: " + peer, Level.Info);
    }

    /// <summary>
    /// Tell the group to add this station as a peer!
    /// </summary>
    /// <param name="newPeer">
    /// The address of the station to add.
    /// </param>
    /// <param name="newPeerKey">
    /// The public AsymmetricKey of the station to add.
    /// </param>
    public void AnnounceAddPeer(IPEndPoint newPeer, AsymmetricKey newPeerKey) {
      Contract.Requires(IsManager);
      Contract.Requires(newPeer != null);
      Peers.Keys.Where(peer => !peer.Equals(newPeer))
          .ForEach(peer => 
            Communicator.Send(new AddPeerCommand(Address, newPeer, newPeerKey), peer));
      if (Logger != null) 
        Logger.Log("Announcing that this peer should be added to the peerlist: " + 
          newPeer, Level.Info);
    }

    /// <summary>
    ///   Announce to all stations that the election has ended!
    /// </summary>
    public void AnnounceEndElection() {
      Contract.Requires(ElectionInProgress);
      Contract.Requires(IsManager);
      Contract.Ensures(!ElectionInProgress);
      EndElection();
      Peers.Keys.ForEach(peer => 
        Communicator.Send(new EndElectionCommand(Address), peer));
      if (Logger != null) 
        Logger.Log("Announcing that the election should be ended.", 
          Level.Info);
    }

    /// <summary>
    /// Tell the group to remove this station as a peer!
    /// </summary>
    /// <param name="peerToRemove">
    /// The address of the station to remove.
    /// </param>
    public void AnnounceRemovePeer(IPEndPoint peerToRemove) {
      Contract.Requires(IsManager);
      Contract.Requires(peerToRemove != null);
      Contract.Requires(Peers.Keys.Contains(peerToRemove));
      foreach (IPEndPoint peer in Peers.Keys) {
        if (!Address.Equals(peer) && !peer.Equals(peerToRemove)) {
          Communicator.Send(new RemovePeerCommand(Address, peerToRemove), peer);
        }
      }
      if (Logger != null) 
        Logger.Log("Announcing that this peer should be removed from the peerlist: " + 
          peerToRemove, Level.Info);
    }

    /// <summary>
    /// Announce to all that they should revoke this update!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to revoke a ballot for.
    /// </param>
    /// <param name="cpr">
    /// The CPR number to revoke a ballot for.
    /// </param>
    public void AnnounceRevokeBallot(Voter voter, VoterStatus oldStatus) {
      Contract.Requires(IsManager);
      var cmd = new DemandStatusChangeCommand(Address, new VoterNumber(voter.VoterId), oldStatus);
      Peers.Keys.ForEach(peer => Communicator.Send(cmd, peer));
      cmd.Execute(this);
      if (Logger != null) 
        Logger.Log("Announcing that this status change should be revoked for voter number " + 
          voter.VoterId, Level.Warn);
    }

    /// <summary>
    ///   Announce to all stations that the election has started!
    /// </summary>
    public void AnnounceStartElection() {
      Contract.Requires(!ElectionInProgress);
      Contract.Requires(IsManager);
      Contract.Ensures(ElectionInProgress);
      StartElection();
      Peers.Keys.ForEach(peer => Communicator.Send(new StartElectionCommand(Address), peer));
      if (Logger != null) 
        Logger.Log("Announcing that the election should be started.", 
          Level.Info);
    }

    /// <summary>
    /// This voter received a ballot!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public void BallotReceived(VoterNumber voterNumber, VoterStatus voterStatus) {
      Contract.Ensures(Database[voterNumber] == voterStatus);
      Database[voterNumber] = voterStatus;
      if (Logger != null) 
        Logger.Log("Changed voter number " + voterNumber + 
          " status to " + voterStatus + ".", Level.Info);
      UI.RefreshStatistics();
    }

    /// <summary>
    /// What machines on the network respond that they have the digital voter list software running?
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    [Pure] public void DiscoverPeers() {
      HashSet<IPEndPoint> potentials = new HashSet<IPEndPoint>(Communicator.DiscoverPeers().Concat(Peers.Keys));

      foreach (IPEndPoint peer in potentials) {
        bool listening = Communicator.IsListening(peer);
        if (listening && !Peers.Keys.Contains(peer)) {
          if (PeerStatuses.Keys.Contains(peer)) {
            PeerStatuses[peer].ConnectionState = "Not Connected";
          } else {
            PeerStatuses[peer] = new StationStatus(peer, Communicator.GetIdentifyingStringForStation(peer), "Not Connected");
          }
        } else if (!listening) {
          RemovePeer(peer, false);
        }
      }

      UI.RefreshPeers();
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() {
      if (!_isDisposed) Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Elect a new manager!
    /// </summary>
    public void ElectNewManager() {
      Contract.Ensures(Manager != null);
      RemovePeer(Manager, false);
      var candidates = new SortedSet<IPEndPoint>(new IPEndPointComparer()) { Address };
      Peers.Keys.Where(StationActive).ForEach(candidate => 
        candidates.Add(candidate));
      Manager = candidates.First();
      if (IsManager) {
        UI.IsNowManager();
      } else {
        UI.ResetBallotRequestPage();
      }
      if (Logger != null) 
        Logger.Log("Elected new manager: " + Manager, Level.Warn);
    }

    /// <summary>
    ///   End the election
    /// </summary>
    public void EndElection() {
      Contract.Requires(ElectionInProgress);
      Contract.Ensures(!ElectionInProgress);
      ElectionInProgress = false;
      if (Logger != null) Logger.Log("Election ended", Level.Info);
    }

    /// <summary>
    /// Exchange public keys with this machine!
    /// </summary>
    /// <param name="target">
    /// The address of the machine to exchange public keys with.
    /// </param>
    public void ExchangePublicKeys(IPEndPoint target) {
      Contract.Requires(target != null);
      Communicator.Send(new PublicKeyExchangeCommand(this, target), target);
      if (Logger != null) 
        Logger.Log("Exchanging public keys with " + target, Level.Info);
    }

    public bool ImportData(IEnumerable<Voter> voterData, IEnumerable<Precinct> precinctData) {
      bool result = false;
      try {
        if (Database == null) {
          Console.WriteLine("null database!");
          Database = new VoterListDatabase(this, _dbPrefix);
        }
        result = Database.Import(voterData);
        result &= Database.Import(precinctData);
        if (!result) {
          Database.Dispose();
          Database = new VoterListDatabase(this, _dbPrefix);
        }
      } catch (Exception e) {
        Console.WriteLine("Exception when loading data: " + e);
      }
      return result;
    }

    /// <summary>
    /// Make this station the new manager!
    /// </summary>
    /// <param name="newManager">
    /// The station who is to be the new manager.
    /// </param>
    public void PromoteNewManager(IPEndPoint newManager) {
      Contract.Requires(!PromotionInProgress);
      Contract.Requires(IsManager);
      Contract.Requires(newManager != null);
      Communicator.Send(new PromoteNewManagerCommand(Address, newManager), newManager);
      PromotedStation = newManager;
      PromotionInProgress = true;
      if (Logger != null) 
        Logger.Log("Promoting " + newManager + " to be the manager", 
          Level.Warn);
    }

    public void FinishPromotion(IPEndPoint newManager) {
      if (PromotionInProgress && PromotedStation.Equals(newManager)) {
        Manager = newManager;
        PromotionInProgress = false;
        PromotedStation = null;
        var others = Peers.Keys.Where(peer => !peer.Equals(newManager));
        others.ForEach(
          peer => Communicator.Send(new PromoteNewManagerCommand(Address, newManager), peer));
        UI.ConvertToStation();
        if (Logger != null) {
          Logger.Log("Promotion of " + newManager + " complete", Level.Warn);
        }
      } else {
        if (Logger != null) {
          Logger.Log("Erroneous promotion claim by " + newManager + ", ending election", Level.Fatal);
          ShutDownElection();
        }
      }
    }

    /// <summary>
    /// Removes this station from the system
    /// </summary>
    public void RemoveSelf() {
      Contract.Requires(Manager != null);
      Contract.Requires(!Manager.Equals(Address));

      if (Logger != null) Logger.Log("Station " + Address + " notifying manager of departure.", Level.Info);
      Communicator.Send(new RemovePeerCommand(Address, Address), Manager);
    }

    /// <summary>
    /// Remove this station from the group!
    /// </summary>
    /// <param name="peer">
    /// The address of the station to remove.
    /// </param>
    public void RemovePeer(IPEndPoint peer, Boolean disconnect) {
      Contract.Requires(peer != null);
      Contract.Ensures(!Peers.ContainsKey(peer));
      if (Peers.Remove(peer)) {
        if (IsManager && disconnect) {
          // only the manager should send disconnect messages
          if (Logger != null) Logger.Log("Sending disconnect command to station " + peer, Level.Info);
          Communicator.Send(new DisconnectStationCommand(new IPEndPoint(Manager.Address, Manager.Port), peer), peer);
          AnnounceRemovePeer(peer);
        }
        PeerStatuses.Remove(peer);
        if (Logger != null) Logger.Log("Station " + peer + " removed from peer list.", Level.Info);
        if (!EnoughStations) UI.NotEnoughPeers();
        UI.RefreshPeers();
      } else if (peer.Equals(Address)) {
        // we are the peer being disconnected
        if (Logger != null) Logger.Log("This station has been disconnected by the manager.", Level.Info);
        UI.StationRemoved();
      } else {
        if (Logger != null) Logger.Log("Attempt to remove nonexistent peer " + peer, Level.Error);
      }
    }

    /// <summary>
    /// Request a ballot for this voter!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public void RequestStatusChange(Voter voter, VoterStatus voterStatus) {
      Communicator.Send(new RequestStatusChangeCommand(Address, new VoterNumber(voter.VoterId), (VoterStatus) voter.PollbookStatus, voterStatus), Manager);
      if (Logger != null) 
        Logger.Log("Requesting status change for voter number " + 
          voter.VoterId + " to " + voterStatus, Level.Info);
    }

    public void MakePeerAvailableAndRefresh(IPEndPoint peer) {
      UI.DoneSynchronizing(peer);
    }

    public void SetPeerStatus(IPEndPoint peer, string status) {
      if (PeerStatuses.ContainsKey(peer)) {
        PeerStatuses[peer].ConnectionState = status;
      }
    }

    /// <summary>
    ///   The system is compromised, notify everyone and shut down the election!
    /// </summary>
    public void ShutDownElection() {
      if (Logger != null) 
        Logger.Log("Compromised system---shutting down election.", 
          Level.Fatal);
      Peers.Keys.ForEach(peer => 
        Communicator.Send(new ShutDownElectionCommand(Address), peer));
      UI.Shutdown();
      throw new TheOnlyException();
    }

    /// <summary>
    ///   Start the election!
    /// </summary>
    public void StartElection() {
      Contract.Ensures(ElectionInProgress);
      ElectionInProgress = true;
      if (Logger != null) Logger.Log("Election started", Level.Info);
    }

    /// <summary>
    ///   Start listening to other stations!
    /// </summary>
    public void StartListening() {
      Contract.Requires(!Listening);
      Contract.Ensures(Listening);
      Listening = true;
      Communicator.StartThreads();
    }

    /// <summary>
    ///   Start election of a new manager!
    /// </summary>
    public void StartNewManagerElection() {
      ElectNewManager();
      Peers.Keys.ForEach(peer => 
        Communicator.Send(new ElectNewManagerCommand(Address), peer));
      if (Logger != null) 
        Logger.Log("Announced that a new manager should be elected", 
          Level.Warn);
    }

    /// <summary>
    /// Is this station active?
    /// </summary>
    /// <param name="target">
    /// The station to check.
    /// </param>
    /// <returns>
    /// True if the station is active, otherwise false.
    /// </returns>
    [Pure] public bool StationActive(IPEndPoint target) {
      Contract.Requires(target != null);
      Contract.Requires(Listening);
      return Communicator.IsListening(target);
    }

    /// <summary>
    ///   Stop listening to other stations!
    /// </summary>
    public void StopListening() {
      Contract.Requires(Listening);
      Contract.Ensures(!Listening);
      Communicator.StopThreads();
      Listening = false;
      if (Logger != null) 
        Logger.Log("Stopped listening", Level.Info);
    }

    /// <summary>
    /// Is this string the master password?
    /// </summary>
    /// <param name="password">
    /// The password to check.
    /// </param>
    /// <returns>
    /// True if the password is identical to the master password, false otherwise.
    /// </returns>
    [Pure] public bool ValidMasterPassword(string password) {
      Contract.Requires(password != null);
      return _masterPassword != null &&
             _masterPassword.SequenceEqual(
               Crypto.Hash(Bytes.From(password)));
    }

    #endregion

    // Methods only the manager should be able to invoke.

    // Methods related to announcing changes to the list of peers.
    #region Methods

    /// <summary>
    /// The dispose.
    /// </summary>
    /// <param name="disposing">
    /// The disposing.
    /// </param>
    private void Dispose(bool disposing) {
      _isDisposed = true;
      if (!disposing) return;
      //if (Crypto != null) Crypto.Dispose();
      if (Logger != null) {
        Logger.Log("Disposing self", Level.Info);
        Logger.Dispose();
      }
      if (Listening) StopListening();
      if (Database != null) Database.Dispose();

      _logger = null;
      _crypto = null;
      Database = null;
      Console.WriteLine("Stopping communicator threads...");
      Communicator.StopThreads();
      Communicator = null;
      Console.WriteLine("Station " + this + " disposed."); 
    }

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() {
      Contract.Invariant(Address != null);
      Contract.Invariant(Peers != null);
    }

    #endregion
  }
}
