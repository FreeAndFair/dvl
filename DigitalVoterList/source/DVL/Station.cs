#region Copyright and License

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
    /// The _listener thread.
    /// </summary>
    private Thread _listenerThread;

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
    /// Are we currently using a master password at all?
    /// </summary>
    internal bool IsMasterPasswordInUse = false;

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
    /// <param name="dbName">
    /// The name of the voter database. Defaults to Voters.sqlite.
    /// </param>
    /// <param name="logName">
    /// The name of the logging database. Defaults to Log.sqlite.
    /// </param>
    public Station(IDvlUi ui, 
                   AsymmetricKey voterDataEncryptionKey, 
                   string masterPassword, 
                   int port = 62000, 
                   string dbName = "ElectionData.sqlite", 
                   string logName = "Log.sqlite")
      : this(ui, port, dbName) {
      Contract.Requires(ui != null);
      Contract.Requires(masterPassword != null);
      Contract.Requires(dbName != null);
      Contract.Requires(logName != null);

      this.Crypto = new Crypto(voterDataEncryptionKey);
      this.MasterPassword =
        this.Crypto.Hash(Bytes.From(masterPassword));
      this.Logger = new Logger(this, logName);
      this.Manager = this.Address;
      this.Logger.Log("Manager initialized", Level.Info);
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
    /// <param name="voterDbName">
    /// The name of the voter database. Defaults to Voters.sqlite.
    /// </param>
    /// <param name="logName">
    /// The name of the logging database. Defaults to Log.sqlite.
    /// </param>
    
    public Station(IDvlUi ui, 
                   string keyPath, 
                   string masterPassword, 
                   int port = 62000, 
                   string voterDbName = "Voters.sqlite", 
                   string logName = "Log.sqlite")
      : this(ui, 
        new AsymmetricKey(Bytes.FromFile(keyPath).ToKey()), 
        masterPassword, 
        port, 
        voterDbName, 
        logName) {
      Contract.Requires(ui != null);
      Contract.Requires(keyPath != null);
      Contract.Requires(File.Exists(keyPath));
      Contract.Requires(voterDbName != null);
      Contract.Requires(logName != null);
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
    /// <param name="databaseFile">
    /// The name of the database file.
    /// </param>
    public Station(IDvlUi ui, 
                   int port = 62000, 
                   string databaseFile = "Voters.sqlite") {
      Contract.Requires(ui != null);
      Contract.Requires(databaseFile != null);

      this.Peers = new SortedDictionary<IPEndPoint, AsymmetricKey>(new IPEndPointComparer());
      this.ElectionInProgress = false;
      this.AllStationsAvailable = true;
      this.Address =
        new IPEndPoint(
          Dns.GetHostEntry(Dns.GetHostName()).AddressList.First(ip => 
            ip.AddressFamily == AddressFamily.InterNetwork), 
          port);
      this.Database = new VoterListDatabase(this, databaseFile);
      this.Communicator = new Communicator(this);
      this.UI = ui;
      this.Crypto = new Crypto();
      this.StartListening();
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Station"/> class. 
    /// </summary>
    ~Station() { this.Dispose(false); }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What is my ip address?
    /// </summary>
    public IPEndPoint Address { [Pure] get; private set; }

    /// <summary>
    /// Gets or sets a value indicating whether all stations available.
    /// </summary>
    public bool AllStationsAvailable { [Pure] get; set; }

    /// <summary>
    ///   How can I communicate with my group?
    /// </summary>
    public ICommunicator Communicator { [Pure] get; private set; }

    /// <summary>
    ///   How can I encrypt messages?
    ///   This is how you encrypt messages!
    /// </summary>
    public ICrypto Crypto {
      [Pure] get { return this._crypto; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(Equals(this.Crypto, value));
        this._crypto = value;
      }
    }

    /// <summary>
    ///   How can I manipulate my database?
    /// </summary>
    public IDatabase Database { [Pure] get; private set; }

    /// <summary>
    ///   What is the status of the election?
    /// </summary>
    public bool ElectionInProgress { [Pure] get; private set; }

    /// <summary>
    ///   Is there enough active stations in the group to continue operations?
    /// </summary>
    public bool EnoughStations { [Pure] get {
      return this.Peers.Keys.Count(this.StationActive) >= 0; 
      // TODO: Correct to '>' when not testing
    }}

    /// <summary>
    ///   Am I the manager?
    /// </summary>
    public bool IsManager { [Pure] get {
      return this.Address.Equals(this.Manager);
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
      [Pure] get { return this._logger; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(Equals(this.Logger, value));
        this._logger = value;
      }
    }

    /// <summary>
    ///   Who is the manager?
    ///   This station is now the manager!
    /// </summary>
    public IPEndPoint Manager {
      [Pure] get { return this._manager; }
      set {
        Contract.Requires(value != null);
        Contract.Ensures(this.Manager.Equals(value));
        this._manager = value;
      }
    }

    /// <summary>
    ///   What is the master password?
    ///   The master password is this!
    /// </summary>
    public byte[] MasterPassword {
      get { return this._masterPassword; }
      set {
        Contract.Requires(value != null);
        Contract.Requires(this.MasterPassword == null);
        Contract.Ensures(Equals(this.MasterPassword, value));
        this._masterPassword = value;
        value.ToFile("Master.pw");
      }
    }

    /// <summary>
    ///   Who are my peers?
    /// </summary>
    public SortedDictionary<IPEndPoint, AsymmetricKey> Peers {
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
      Contract.Requires(!this.Peers.ContainsKey(peer));
      Contract.Ensures(this.Peers.ContainsKey(peer));
      this.Peers.Add(peer, peerPublicAsymmetricKey);
      if (this.EnoughStations) this.UI.EnoughPeers();
      if (this.Logger != null) this.Logger.Log("Peer added: " + peer, Level.Info);
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
      Contract.Requires(this.IsManager);
      Contract.Requires(newPeer != null);
      this.Peers.Keys.Where(peer => !peer.Equals(newPeer))
          .ForEach(peer => 
            this.Communicator.Send(new AddPeerCommand(this.Address, newPeer, newPeerKey), peer));
      if (this.Logger != null) 
        this.Logger.Log("Announcing that this peer should be added to the peerlist: " + 
          newPeer, Level.Info);
    }

    /// <summary>
    ///   Announce to all stations that the election has ended!
    /// </summary>
    public void AnnounceEndElection() {
      Contract.Requires(this.ElectionInProgress);
      Contract.Requires(this.IsManager);
      Contract.Ensures(!this.ElectionInProgress);
      this.EndElection();
      this.Peers.Keys.ForEach(peer => 
        this.Communicator.Send(new EndElectionCommand(this.Address), peer));
      if (this.Logger != null) 
        this.Logger.Log("Announcing that the election should be ended.", 
          Level.Info);
    }

    /// <summary>
    /// Tell the group to remove this station as a peer!
    /// </summary>
    /// <param name="peerToRemove">
    /// The address of the station to remove.
    /// </param>
    public void AnnounceRemovePeer(IPEndPoint peerToRemove) {
      Contract.Requires(this.IsManager);
      Contract.Requires(peerToRemove != null);
      this.RemovePeer(peerToRemove);
      this.Peers.Keys.ForEach(peer => 
        this.Communicator.Send(new RemovePeerCommand(this.Address, peerToRemove), peer));
      if (this.Logger != null) 
        this.Logger.Log("Announcing that this peer should be removed from the peerlist: " + 
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
    public void AnnounceRevokeBallot(VoterNumber voterNumber) {
      Contract.Requires(this.IsManager);
      Contract.Requires(this.Database[voterNumber] == BallotStatus.Received);
      var cmd = new RevokeBallotCommand(this.Address, voterNumber);
      this.Peers.Keys.ForEach(peer => this.Communicator.Send(cmd, peer));
      cmd.Execute(this);
      if (this.Logger != null) 
        this.Logger.Log("Announcing that this ballot should be revoked for voter number " + 
          voterNumber, Level.Warn);
    }
      
    /*
    /// <summary>
    /// Announce to all that they should revoke this update!
    /// </summary>
    /// <param name="cpr">
    /// The CPR number to revoke a ballot for.
    /// </param>
    /// <param name="masterPassword">
    /// The master password that only the election secretary should know.
    /// </param>
    public void AnnounceRevokeBallot(CPR cpr, string masterPassword) {
      Contract.Requires(masterPassword != null);
      Contract.Requires(this.ValidMasterPassword(masterPassword));
      Contract.Requires(this.Database[cpr, masterPassword] == 
        BallotStatus.Received);
      Contract.Requires(this.IsManager);
      var cmd = new RevokeBallotCPROnlyCommand(this.Address, cpr, masterPassword);
      this.Peers.Keys.ForEach(peer => this.Communicator.Send(cmd, peer));
      cmd.Execute(this);
      if (this.Logger != null)
        this.Logger.Log("Announcing that this ballot should be revoked " +
          "with master password: CPR " + cpr, Level.Warn);
    }
    */

    /// <summary>
    ///   Announce to all stations that the election has started!
    /// </summary>
    public void AnnounceStartElection() {
      Contract.Requires(!this.ElectionInProgress);
      Contract.Requires(this.IsManager);
      Contract.Ensures(this.ElectionInProgress);
      this.StartElection();
      this.Peers.Keys.ForEach(peer => this.Communicator.Send(new StartElectionCommand(this.Address), peer));
      if (this.Logger != null) 
        this.Logger.Log("Announcing that the election should be started.", 
          Level.Info);
    }

    /// <summary>
    /// This voter received a ballot!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public void BallotReceived(VoterNumber voterNumber) {
      Contract.Requires(this.Database[voterNumber] == BallotStatus.NotReceived);
      Contract.Ensures(this.Database[voterNumber] == BallotStatus.Received);
      this.Database[voterNumber] = BallotStatus.Received;
      if (this.Logger != null) 
        this.Logger.Log("Marking voternumber=" + voterNumber + 
          " as having received a ballot.", Level.Info);
    }

    /*
    /// <summary>
    /// This voter received a ballot!
    /// </summary>
    /// <param name="cpr">
    /// The CPR number to request a ballot for.
    /// </param>
    /// <param name="password">
    /// The master password.
    /// </param>
    public void BallotReceived(CPR cpr, string password) {
      Contract.Requires(password != null);
      Contract.Requires(this.ValidMasterPassword(password));
      Contract.Requires(this.Database[cpr, password] == BallotStatus.NotReceived);
      Contract.Ensures(this.Database[cpr, password] == BallotStatus.Received);
      this.Database[cpr, password] = BallotStatus.Received;
      if (this.Logger != null) this.Logger.Log("Marking CPR " + cpr + 
        " with master password as having received a ballot.", Level.Info);
    }
    */

    /// <summary>
    /// What machines on the network respond that they have the digital voter list software running?
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    [Pure] public IEnumerable<IPEndPoint> DiscoverPeers() {
      Contract.Ensures(Contract.Result<IEnumerable<IPEndPoint>>() != null);
      Contract.Ensures(Contract.Result<IEnumerable<IPEndPoint>>().
        All(x => x != null));
      return this.Communicator.DiscoverNetworkMachines().Where(address => 
        !address.Equals(this.Address));
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() {
      if (!this._isDisposed) this.Dispose(true);
      GC.SuppressFinalize(this);
    }

    /// <summary>
    ///   Elect a new manager!
    /// </summary>
    public void ElectNewManager() {
      Contract.Requires(!this.StationActive(this.Manager));
      Contract.Ensures(!this.Manager.Equals(Contract.OldValue(this.Manager)));
      this.RemovePeer(this.Manager);
      var candidates = new SortedSet<IPEndPoint>(new IPEndPointComparer()) { this.Address };
      this.Peers.Keys.Where(this.StationActive).ForEach(candidate => 
        candidates.Add(candidate));
      this.Manager = candidates.First();
      if (this.IsManager) this.UI.IsNowManager();
      if (this.Logger != null) 
        this.Logger.Log("Elected new manager: " + this.Manager, Level.Warn);
    }

    /// <summary>
    ///   End the election
    /// </summary>
    public void EndElection() {
      Contract.Requires(this.ElectionInProgress);
      Contract.Ensures(!this.ElectionInProgress);
      this.ElectionInProgress = false;
      if (this.Logger != null) this.Logger.Log("Election ended", Level.Info);
    }

    /// <summary>
    /// Exchange public keys with this machine!
    /// </summary>
    /// <param name="target">
    /// The address of the machine to exchange public keys with.
    /// </param>
    public void ExchangePublicKeys(IPEndPoint target) {
      Contract.Requires(target != null);
      Contract.Requires(this.StationActive(target));
      this.Communicator.Send(new PublicKeyExchangeCommand(this), target);
      if (this.Logger != null) 
        this.Logger.Log("Exchanging public keys with " + target, Level.Info);
    }

    /// <summary>
    /// Make this station the new manager!
    /// </summary>
    /// <param name="newManager">
    /// The station who is to be the new manager.
    /// </param>
    public void PromoteNewManager(IPEndPoint newManager) {
      Contract.Requires(this.IsManager);
      Contract.Requires(newManager != null);
      this.Peers.Keys.ForEach(
        peer => this.Communicator.Send(new PromoteNewManagerCommand(this.Address, newManager), peer));
      this.Manager = newManager;
      if (this.Logger != null) 
        this.Logger.Log("Promoting " + newManager + " to be the manager", 
          Level.Warn);
    }

    /// <summary>
    /// Remove this station from the group!
    /// </summary>
    /// <param name="peer">
    /// The address of the station to remove.
    /// </param>
    public void RemovePeer(IPEndPoint peer) {
      Contract.Requires(peer != null);
      Contract.Requires(this.Peers.ContainsKey(peer));
      Contract.Ensures(!this.Peers.ContainsKey(peer));
      this.Communicator.Send(new DisconnectStationCommand(new IPEndPoint(this.Manager.Address, 62000), peer), peer);
      this.Peers.Remove(peer);
      if (!this.EnoughStations) this.UI.NotEnoughPeers();
      if (this.Logger != null) this.Logger.Log("Peer removed: " + peer, Level.Info);
    }

    /// <summary>
    /// Request a ballot for this voter!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public void RequestBallot(VoterNumber voterNumber) {
      Contract.Requires(this.Database[voterNumber] == BallotStatus.NotReceived);
      this.Communicator.Send(new RequestBallotCommand(this.Address, voterNumber), this.Manager);
      if (this.Logger != null) 
        this.Logger.Log("Requesting ballot for: voter number " + 
          voterNumber, Level.Info);
    }

    /*
    /// <summary>
    /// Request a ballot for this voter!
    /// </summary>
    /// <param name="cpr">
    /// The CPR number to request a ballot for.
    /// </param>
    /// <param name="password">
    /// The master password.
    /// </param>
    public void RequestBallot(CPR cpr, string password) {
      Contract.Requires(password != null);
      Contract.Requires(this.ValidMasterPassword(password));
      Contract.Requires(this.Database[cpr, password] == BallotStatus.NotReceived);
      this.Communicator.Send(new RequestBallotCPROnlyCommand(this.Address, cpr,
        password), this.Manager);
      if (this.Logger != null) 
        this.Logger.Log("Requesting ballot with master password for CPR " + 
          cpr, Level.Info);
    }
    */

    /// <summary>
    /// Revoke this ballot!
    /// </summary>
    /// <param name="voterNumber">
    /// The voternumber to revoke a ballot for.
    /// </param>
    public void RevokeBallot(VoterNumber voterNumber) {
      Contract.Requires(this.Database[voterNumber] == BallotStatus.Received);
      Contract.Ensures(this.Database[voterNumber] == BallotStatus.NotReceived);
      this.Database[voterNumber] = BallotStatus.NotReceived;
      if (this.Logger != null) 
        this.Logger.Log("Revoking ballot for voter with voter number " + 
          voterNumber, Level.Warn);
    }

    /// <summary>
    /// Revoke this ballot!
    /// </summary>
    /// <param name="cpr">
    /// The CPR number to revoke a ballot for.
    /// </param>
    /// <param name="masterPassword">
    /// The master password that only the election secretary should know.
    /// </param>
    /*
    public void RevokeBallot(CPR cpr, string masterPassword) {
      Contract.Requires(masterPassword != null);
      Contract.Requires(this.ValidMasterPassword(masterPassword));
      Contract.Requires(this.Database[cpr, masterPassword] == BallotStatus.Received);
      Contract.Ensures(this.Database[cpr, masterPassword] == BallotStatus.NotReceived);
      this.Database[cpr, masterPassword] = BallotStatus.NotReceived;
      if (this.Logger != null) 
        this.Logger.Log("Revoking ballot with master password for voter with CPR " + 
          cpr, Level.Warn);
    }
    */
    /// <summary>
    ///   The system is compromised, notify everyone and shut down the election!
    /// </summary>
    public void ShutDownElection() {
      if (this.Logger != null) 
        this.Logger.Log("Compromised system---shutting down election.", 
          Level.Fatal);
      this.Peers.Keys.ForEach(peer => 
        this.Communicator.Send(new ShutDownElectionCommand(this.Address), peer));
      this.UI.Shutdown();
      throw new TheOnlyException();
    }

    /// <summary>
    ///   Start the election!
    /// </summary>
    public void StartElection() {
      Contract.Requires(!this.ElectionInProgress);
      Contract.Ensures(this.ElectionInProgress);
      this.ElectionInProgress = true;
      if (this.Logger != null) this.Logger.Log("Election started", Level.Info);
    }

    /// <summary>
    ///   Start listening to other stations!
    /// </summary>
    public void StartListening() {
      Contract.Requires(!this.Listening);
      Contract.Ensures(this.Listening);
      this.Listening = true;
      this._listenerThread = new Thread(this.LoopListen);
      this._listenerThread.SetApartmentState(ApartmentState.STA);
      this._listenerThread.Start();
      while (!this.StationActive(this.Address)) ;
    }

    /// <summary>
    ///   Start election of a new manager!
    /// </summary>
    public void StartNewManagerElection() {
      this.ElectNewManager();
      this.Peers.Keys.ForEach(peer => 
        this.Communicator.Send(new ElectNewManagerCommand(this.Address), peer));
      if (this.Logger != null) 
        this.Logger.Log("Announced that a new manager should be elected", 
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
      return this.Communicator.IsListening(target);
    }

    /// <summary>
    ///   Stop listening to other stations!
    /// </summary>
    public void StopListening() {
      Contract.Requires(this.Listening);
      Contract.Ensures(!this.Listening);
      this.Listening = false;
      while (this.StationActive(this.Address)) ;
      if (this._listenerThread != null) this._listenerThread.Abort();
      this._listenerThread = null;
      if (this.Logger != null) 
        this.Logger.Log("Stopped listening", Level.Info);
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
      return this._masterPassword != null &&
             this._masterPassword.SequenceEqual(
               this.Crypto.Hash(Bytes.From(password)));
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
      this._isDisposed = true;
      if (!disposing) return;
      //if (this.Crypto != null) this.Crypto.Dispose();
      if (this.Listening) this.StopListening();
      this.Database.Dispose();
      if (this.Logger != null) {
        this.Logger.Log("Disposing self", Level.Info);
        this.Logger.Dispose();
      }

      this._logger = null;
      this._crypto = null;
      this.Database = null;
    }

    /// <summary>
    /// The loop listen.
    /// </summary>
    private void LoopListen() { while (this.Listening) this.Communicator.ReceiveAndHandle(); }

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() {
      Contract.Invariant(this.Address != null);
      Contract.Invariant(this.Peers != null);
    }

    #endregion
  }
}
