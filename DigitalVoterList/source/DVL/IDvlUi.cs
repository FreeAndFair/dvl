#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="IDvlUi.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL {
  using System.Net;
  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Database;

  /// <summary>
  ///   A UI is used to interact with human beings. The UI must be able to support requirements to be able to interact with the Digital Voter List system.
  /// </summary>
  public interface IDvlUi {
    #region Public Methods and Operators

    /// <summary>
    /// Let the UI know whether or not the voter can receive a ballot!
    /// </summary>
    /// <param name="handOutBallot">
    /// Whether or not the ballot should be handed out to the voter.
    /// </param>
    void BallotRequestReply(VoterNumber vn, bool handOutBallot, VoterStatus oldStatus, VoterStatus newStatus);
    void BallotRequestReply(Voter v, bool handOutBallot, VoterStatus oldStatus, VoterStatus newStatus);

    /// <summary>
    ///   Let the UI know that the election has ended!
    /// </summary>
    // When the election ends, the UI is notified with this method.
    void ElectionEnded();

    /// <summary>
    ///   Let the UI know that the election has started!
    /// </summary>
    // When the election starts, the UI is notified with this method.
    void ElectionStarted();

    /// <summary>
    ///   Let the UI know that there are enough peers to continue execution!
    /// </summary>
    // Called when the number of peers during the election rises above the required amount.
    void EnoughPeers();

    /// <summary>
    ///   Let the UI know that this machine is now the manager!
    /// </summary>
    // Called on a station to signal that it has now become the new manager.
    void IsNowManager();

    void ResetBallotRequestPage();

    /// <summary>
    /// What is the password the user typed in to respond to the manager initiating a key-exchange?
    /// </summary>
    /// <param name="ip">
    /// The IP adress of the connecting manager.
    /// </param>
    /// <returns>
    /// The typed password.
    /// </returns>
    string ManagerExchangingKey(IPEndPoint ip);

    /// <summary>
    ///   Let the UI know that there are not enough peers to continue execution!
    /// </summary>
    // Called when the number of peers during the election falls below the required amount.
    void NotEnoughPeers();

    /// <summary>
    /// Show this password on the manager machine!
    /// </summary>
    /// <param name="password">
    /// The password to display.
    /// </param>
    void ShowPasswordOnManager(string password);

    /// <summary>
    /// Show this password on a station machine!
    /// </summary>
    /// <param name="password">
    /// The password to display.
    /// </param>
    void ShowPasswordOnStation(string password);

    /// <summary>
    ///   Let the UI know that it needs to shut down!
    /// </summary>
    // Called when the election is compromised and needs to shut down.
    void Shutdown();

    /// <summary>
    /// What is the password the user typed in when a station is replying to a key-exchange?
    /// </summary>
    /// <param name="ip">
    /// The IP adress of the replying station.
    /// </param>
    /// <returns>
    /// The typed password.
    /// </returns>
    string StationExchangingKey(IPEndPoint ip);

    /// <summary>
    ///   Let the peer know that it is removed
    /// </summary>
    /// Called when a peer has been removed by a Manager.
    void StationRemoved();

    void Synchronizing(IPEndPoint ip);
    void DoneSynchronizing(IPEndPoint ip);

    /// <summary>
    /// Called when a station is done processing a SyncCommand, handled contextually.
    /// </summary>
    void SyncComplete();

    #endregion
  }
}
