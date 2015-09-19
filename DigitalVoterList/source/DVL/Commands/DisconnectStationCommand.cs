#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="DisconnectStationCommand.cs" company="DemTech">
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
  /// The disconnect station command.
  /// </summary>
  [Serializable] public class DisconnectStationCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _peer.
    /// </summary>
    private readonly IPEndPoint _peer;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="DisconnectStationCommand"/> class. 
    /// May I have a new command that removes a peer at the target?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="peer">
    /// The address of the peer to remove.
    /// </param>
    public DisconnectStationCommand(IPEndPoint sender, IPEndPoint peer) {
      Contract.Requires(sender != null);
      _peer = peer;
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
      if (!Sender.Equals(receiver.Manager)) return;
      Console.WriteLine("calling removePeer for peer " + _peer);
      receiver.RemovePeer(_peer, true);
    }

    #endregion
  }
}
