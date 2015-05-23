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
  [Serializable] public class DemandStatusChangeCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _voternumber.
    /// </summary>
    private readonly VoterNumber _voternumber;
    private readonly VoterStatus _newstatus;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="RequestStatusChangeCommand"/> class. 
    /// May I have a new command that requests a ballot of the target?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="voternumber">
    /// The voternumber to request a ballot for.
    /// </param>
    public DemandStatusChangeCommand(IPEndPoint sender, VoterNumber voternumber, VoterStatus newstatus) {
      Contract.Requires(sender != null);
      this.Sender = sender;
      this._voternumber = voternumber;
      this._newstatus = newstatus;
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
      if (receiver.IsManager || !receiver.Manager.Equals(Sender)) return;
      receiver.Database.GetVoterByVoterNumber(_voternumber).PollbookStatus = (int) _newstatus;
    }

    #endregion
  }
}
