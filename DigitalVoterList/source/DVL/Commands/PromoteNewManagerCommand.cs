#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="PromoteNewManagerCommand.cs" company="DemTech">
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
  /// The promote new manager command.
  /// </summary>
  [Serializable] public class PromoteNewManagerCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _new manager.
    /// </summary>
    private readonly IPEndPoint _newManager;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="PromoteNewManagerCommand"/> class. 
    /// May I have a new command that promotes another machine to be the new manager?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="newManager">
    /// The address of the station that should be the new manager.
    /// </param>
    public PromoteNewManagerCommand(IPEndPoint sender, IPEndPoint newManager) {
      Contract.Requires(sender != null);
      Contract.Requires(newManager != null);
      _newManager = newManager;
      Sender = sender;
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
      if (!receiver.Manager.Equals(Sender)) return;
      receiver.Manager = _newManager;
      if (_newManager.Equals(receiver.Address)) {
        receiver.UI.IsNowManager();
        receiver.Communicator.Send(new AcceptedPromotionCommand(receiver.Address), Sender);
      } else {
        receiver.UI.ResetBallotRequestPage();
      }
    }

    #endregion
  }
}
