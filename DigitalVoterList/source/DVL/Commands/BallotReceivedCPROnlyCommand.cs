#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="BallotReceivedCPROnlyCommand.cs" company="DemTech">
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
  /// The ballot received cpr only command.
  /// </summary>
  [Serializable] public class BallotReceivedCPROnlyCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _cpr.
    /// </summary>
    private readonly CPR _cpr;

    /// <summary>
    /// The _password.
    /// </summary>
    private readonly string _password;

    /// <summary>
    /// The _requester.
    /// </summary>
    private readonly IPEndPoint _requester;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BallotReceivedCPROnlyCommand"/> class. 
    /// May I have a new command that requests a ballot with just the CPR 
    /// number and master password known?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="requester">
    /// The address of the station requesting the command.
    /// </param>
    /// <param name="cpr">
    /// The CPR-number of the ballot being requested.
    /// </param>
    /// <param name="password">
    /// The master password for the election, that only the election 
    /// secretary should know.
    /// </param>
    public BallotReceivedCPROnlyCommand(IPEndPoint sender, IPEndPoint requester, CPR cpr, string password) {
      Contract.Requires(sender != null);
      Contract.Requires(requester != null);
      Contract.Requires(password != null);

      this._requester = requester;
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
      if (!receiver.Manager.Equals(this.Sender) ||
          receiver.Database[this._cpr, this._password] != BallotStatus.NotReceived) return;
      receiver.BallotReceived(this._cpr, this._password);
      if (receiver.Address.Equals(this._requester)) receiver.UI.BallotRequestReply(true);
    }

    #endregion
  }
}
