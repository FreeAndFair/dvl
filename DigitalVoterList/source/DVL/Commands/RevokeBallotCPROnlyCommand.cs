#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="RevokeBallotCPROnlyCommand.cs" company="DemTech">
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
  /// The revoke ballot cpr only command.
  /// </summary>
  [Serializable] public class RevokeBallotCPROnlyCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _cpr.
    /// </summary>
    private readonly CPR _cpr;

    /// <summary>
    /// The _master password.
    /// </summary>
    private readonly string _masterPassword;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RevokeBallotCPROnlyCommand"/> class. 
    /// May I have a a command that revokes a ballot at the target?
    /// </summary>
    /// <param name="sender">
    /// The one sending the command.
    /// </param>
    /// <param name="cpr">
    /// The CPR-number to revoke a ballot for.
    /// </param>
    /// <param name="masterPassword">
    /// The master password only the election secretary should know.
    /// </param>
    public RevokeBallotCPROnlyCommand(IPEndPoint sender, CPR cpr, string masterPassword) {
      Contract.Requires(sender != null);
      this._cpr = cpr;
      this._masterPassword = masterPassword;
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
      if (!receiver.ValidMasterPassword(this._masterPassword) ||
          !receiver.Manager.Equals(this.Sender) ||
          receiver.Database[this._cpr, this._masterPassword] != BallotStatus.Received) return;
      receiver.Database[this._cpr, this._masterPassword] = BallotStatus.NotReceived;
    }

    #endregion
  }
}
