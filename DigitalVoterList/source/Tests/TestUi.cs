#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="TestUi.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Tests {
  using System.Net;

  using Aegis_DVL;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Database;

  /// <summary>
  ///   The test ui.
  /// </summary>
  public class TestUi : IDvlUi {
    #region Public Properties

    /// <summary>
    ///   Gets a value indicating whether hand out ballot.
    /// </summary>
    public bool HandOutBallot { get; private set; }

    /// <summary>
    ///   Gets a value indicating whether is manager.
    /// </summary>
    public bool IsManager { get; private set; }

    /// <summary>
    ///   Gets a value indicating whether ongoing election.
    /// </summary>
    public bool OngoingElection { get; private set; }

    /// <summary>
    ///   Gets the password from manager.
    /// </summary>
    public string PasswordFromManager { get; private set; }

    /// <summary>
    ///   Gets the password from station.
    /// </summary>
    public string PasswordFromStation { get; private set; }

    /// <summary>
    ///   Gets a value indicating whether ui remove peer.
    /// </summary>
    public bool UIRemovePeer { get; private set; }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether ui has enough peers.
    /// </summary>
    protected bool UIHasEnoughPeers { get; set; }

    /// <summary>
    ///   Gets or sets a value indicating whether ui shut down.
    /// </summary>
    protected bool UIShutDown { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The ballot request reply.
    /// </summary>
    /// <param name="handOutBallot">
    /// The hand out ballot.
    /// </param>
    public void BallotRequestReply(VoterNumber vn, bool handOutBallot, VoterStatus oldStatus, VoterStatus newStatus) { this.HandOutBallot = handOutBallot; }
    public void BallotRequestReply(Voter v, bool handOutBallot, VoterStatus oldStatus, VoterStatus newStatus) { this.HandOutBallot = handOutBallot; }
    public void ConvertToStation() { }
    public void RefreshStatistics() { }
    public void RefreshPeers() { }

    /// <summary>
    ///   The election ended.
    /// </summary>
    public void ElectionEnded() { this.OngoingElection = false; }

    /// <summary>
    ///   The election started.
    /// </summary>
    public void ElectionStarted() { this.OngoingElection = true; }

    /// <summary>
    ///   The enough peers.
    /// </summary>
    public void EnoughPeers() { this.UIHasEnoughPeers = true; }

    /// <summary>
    ///   The is now manager.
    /// </summary>
    public void IsNowManager() { this.IsManager = true; }

    /// <summary>
    /// The manager exchanging key.
    /// </summary>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string ManagerExchangingKey(IPEndPoint ip) { return this.PasswordFromManager; }

    /// <summary>
    ///   The not enough peers.
    /// </summary>
    public void NotEnoughPeers() { this.UIHasEnoughPeers = false; }

    /// <summary>
    /// The show password on manager.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnManager(string password, IPEndPoint station) { this.PasswordFromManager = password; }

    /// <summary>
    /// The show password on station.
    /// </summary>
    /// <param name="password">
    /// The password.
    /// </param>
    public void ShowPasswordOnStation(string password, IPEndPoint manager) { this.PasswordFromStation = password; }

    /// <summary>
    ///   The shutdown.
    /// </summary>
    public void Shutdown() { this.UIShutDown = true; }

    /// <summary>
    /// The station exchanging key.
    /// </summary>
    /// <param name="ip">
    /// The ip.
    /// </param>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public string StationExchangingKey(IPEndPoint ip) { return this.PasswordFromStation; }

    /// <summary>
    ///   The station removed.
    /// </summary>
    public void StationRemoved() { this.UIRemovePeer = true; }

    public void SyncComplete() { }

    public void Synchronizing(IPEndPoint ip) { }

    public void DoneSynchronizing(IPEndPoint ip) { }

    public void ResetBallotRequestPage() { }

    #endregion
  }
}
