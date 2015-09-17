#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ElectNewManagerCommand.cs" company="DemTech">
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
  /// The elect new manager command.
  /// </summary>
  [Serializable] public class ElectNewManagerCommand : ICommand {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ElectNewManagerCommand"/> class. 
    /// May I have a new command that asks the target to elect a new manager?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    public ElectNewManagerCommand(IPEndPoint sender) {
      Contract.Requires(sender != null);
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
    public void Execute(Station receiver) { if (!receiver.StationActive(receiver.Manager)) receiver.ElectNewManager(); }

    #endregion
  }
}
