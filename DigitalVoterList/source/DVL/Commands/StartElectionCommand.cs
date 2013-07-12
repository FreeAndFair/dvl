#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="StartElectionCommand.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Commands {
  using System;
  using System.Diagnostics;
  using System.Diagnostics.Contracts;
  using System.Net;

  /// <summary>
  /// The start election command.
  /// </summary>
  [Serializable] public class StartElectionCommand : ICommand {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StartElectionCommand"/> class. 
    /// May I have a new Command that starts the election at the target
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    public StartElectionCommand(IPEndPoint sender) {
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
      if (!receiver.Manager.Equals(this.Sender)) {
        Debug.WriteLine("StartElectionCommand.Execute REGISTERED AS MANAGER");
        return;
      }

      Debug.WriteLine("StartElectionCommand.Execute REGISTERED AS STATION");
      receiver.StartElection();
      receiver.UI.ElectionStarted();
    }

    #endregion
  }
}
