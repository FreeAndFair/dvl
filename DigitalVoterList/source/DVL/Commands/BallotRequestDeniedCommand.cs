#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="BallotRequestDeniedCommand.cs" company="DemTech">
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

  /// <summary>
  /// The ballot request denied command.
  /// </summary>
  [Serializable] public class BallotRequestDeniedCommand : ICommand {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="BallotRequestDeniedCommand"/> class. 
    /// May I have a new command that lets the recipient know that a ballot request was denied?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    public BallotRequestDeniedCommand(IPEndPoint sender) {
      Contract.Requires(sender != null);
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
      if (!this.Sender.Equals(receiver.Manager)) return;
      receiver.UI.BallotRequestReply(false);
    }

    #endregion
  }
}
