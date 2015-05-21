#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="BallotReceivedCommand.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Commands {
  using System;
  using System.Diagnostics.Contracts;
  using System.Net;

  using Aegis_DVL.Data_Types;

  /// <summary>
  /// The ballot received command.
  /// </summary>
  [Serializable] public class BallotReceivedCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _requester.
    /// </summary>
    private readonly IPEndPoint _requester;

    /// <summary>
    /// The _voter number.
    /// </summary>
    private readonly VoterNumber _voterNumber;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BallotReceivedCommand"/> class. 
    /// May I have a new command that tells the target that a ballot should be handed out and be marked as received?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="requester">
    /// The address of the station who initially requested the ballot.
    /// </param>
    /// <param name="voterNumber">
    /// The voternumber of the ballot to be handed out and to be marked as received.
    /// </param>
    /// <param name="cpr">
    /// The CPR-number of the ballot to be handed out and to be marked as received.
    /// </param>
    public BallotReceivedCommand(IPEndPoint sender, IPEndPoint requester, VoterNumber voterNumber) {
      Contract.Requires(sender != null);
      Contract.Requires(requester != null);

      this._requester = requester;
      this._voterNumber = voterNumber;
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
      if (!receiver.Manager.Equals(this.Sender) ||
          receiver.Database[_voterNumber] == BallotStatus.Received) return;
      receiver.Database[_voterNumber] = BallotStatus.Received;
      if (receiver.Address.Equals(this._requester)) receiver.UI.BallotRequestReply(true);
    }

    #endregion
  }
}
