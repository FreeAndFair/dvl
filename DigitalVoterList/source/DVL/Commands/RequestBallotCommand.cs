#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="RequestBallotCommand.cs" company="DemTech">
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
  /// The request ballot command.
  /// </summary>
  [Serializable] public class RequestBallotCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _voternumber.
    /// </summary>
    private readonly VoterNumber _voternumber;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestBallotCommand"/> class. 
    /// May I have a new command that requests a ballot of the target?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="voternumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public RequestBallotCommand(IPEndPoint sender, VoterNumber voternumber) {
      Contract.Requires(sender != null);
      this.Sender = sender;
      this._voternumber = voternumber;
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
      if (receiver.Database[this._voternumber] != BallotStatus.NotReceived) {
        receiver.Communicator.Send(new BallotRequestDeniedCommand(receiver.Address), this.Sender);
        receiver.Logger.Log(
          "Attempted to request a ballot that had status " + receiver.Database[this._voternumber], Level.Info);
        return;
      }

      receiver.BallotReceived(this._voternumber);

      // Send to all but requester
      receiver.Peers.Keys.Where(peer => !peer.Equals(this.Sender))
              .ForEach(
                peer =>
                receiver.Communicator.Send(
                  new BallotReceivedCommand(receiver.Address, this.Sender, this._voternumber), peer));

      if (this.Sender.Equals(receiver.Manager)) {
        receiver.UI.BallotRequestReply(true);
        return;
      }

      // Send to requester last.
      try {
        receiver.Communicator.Send(
          new BallotReceivedCommand(receiver.Address, this.Sender, this._voternumber), this.Sender);
      } catch (SocketException) {
        receiver.AnnounceRevokeBallot(this._voternumber);
      }
    }

    #endregion
  }
}
