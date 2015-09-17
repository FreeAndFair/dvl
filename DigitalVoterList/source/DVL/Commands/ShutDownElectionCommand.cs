#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ShutDownElectionCommand.cs" company="DemTech">
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
  /// The shut down election command.
  /// </summary>
  [Serializable] public class ShutDownElectionCommand : ICommand {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="ShutDownElectionCommand"/> class. 
    /// May I have a new command that shuts down the election at the target?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    public ShutDownElectionCommand(IPEndPoint sender) {
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
    /// <exception cref="TheOnlyException">
    /// </exception>
    public void Execute(Station receiver) {
      receiver.UI.Shutdown();
      throw new TheOnlyException();
    }

    #endregion
  }
}
