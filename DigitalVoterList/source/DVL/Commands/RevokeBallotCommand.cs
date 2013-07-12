#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="RevokeBallotCommand.cs" company="DemTech">
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
  /// The revoke ballot command.
  /// </summary>
  [Serializable] public class RevokeBallotCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _cpr.
    /// </summary>
    private readonly CPR _cpr;

    /// <summary>
    /// The _voter number.
    /// </summary>
    private readonly VoterNumber _voterNumber;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeBallotCommand"/> class. 
    /// May I have a a command that revokes a ballot at the target?
    /// </summary>
    /// <param name="sender">
    /// The one sending the command.
    /// </param>
    /// <param name="voterNumber">
    /// The voternumber to revoke a ballot for.
    /// </param>
    /// <param name="cpr">
    /// The CPR-number to revoke a ballot for.
    /// </param>
    public RevokeBallotCommand(IPEndPoint sender, VoterNumber voterNumber, CPR cpr) {
      Contract.Requires(sender != null);
      this._voterNumber = voterNumber;
      this._cpr = cpr;
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
          receiver.Database[this._voterNumber] != BallotStatus.Received) return;
      receiver.Database[this._voterNumber] = BallotStatus.NotReceived;
    }

    #endregion
  }
}
