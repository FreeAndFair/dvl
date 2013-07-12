#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="RequestBallotCPROnlyCommand.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Commands {
  using System;
  using System.Diagnostics.Contracts;
  using System.Linq;
  using System.Net;
  using System.Net.Sockets;

  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  /// <summary>
  /// The request ballot cpr only command.
  /// </summary>
  [Serializable] public class RequestBallotCPROnlyCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _cpr.
    /// </summary>
    private readonly CPR _cpr;

    /// <summary>
    /// The _password.
    /// </summary>
    private readonly string _password;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestBallotCPROnlyCommand"/> class. 
    /// May I have a new command that requests a ballot at the target?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="cpr">
    /// The CPR-number to request a ballot for.
    /// </param>
    /// <param name="password">
    /// The master-password that only the election secretary should know.
    /// </param>
    public RequestBallotCPROnlyCommand(IPEndPoint sender, CPR cpr, string password) {
      Contract.Requires(sender != null);
      Contract.Requires(password != null);
      this._cpr = cpr;
      this._password = password;
      this.Sender = sender;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public IPEndPoint Sender { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The execute.
    /// </summary>
    /// <param name="receiver">
    /// The receiver.
    /// </param>
    public void Execute(Station receiver) {
      if (!receiver.IsManager) return;
      if (receiver.Database[this._cpr, this._password] != BallotStatus.NotReceived) {
        receiver.Communicator.Send(new BallotRequestDeniedCommand(receiver.Address), this.Sender);
        receiver.Logger.Log(
          "Attempted to request a ballot that had status " + receiver.Database[this._cpr, this._password], Level.Info);
        return;
      }

      receiver.BallotReceived(this._cpr, this._password);
      receiver.Peers.Keys.Where(peer => !peer.Equals(this.Sender))
              .ForEach(
                peer =>
                receiver.Communicator.Send(
                  new BallotReceivedCPROnlyCommand(receiver.Address, this.Sender, this._cpr, this._password), peer));
      if (this.Sender.Equals(receiver.Manager)) {
        receiver.UI.BallotRequestReply(true);
        return;
      }

      try {
        receiver.Communicator.Send(
          new BallotReceivedCPROnlyCommand(receiver.Address, this.Sender, this._cpr, this._password), this.Sender);
      } catch (SocketException) {
        receiver.AnnounceRevokeBallot(this._cpr, this._password);
      }
    }

    #endregion
  }
}
