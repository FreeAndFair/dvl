﻿#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="AllStationsAvailableCommand.cs" company="DemTech">
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
  /// The all stations available.
  /// </summary>
  [Serializable] public class StationAvailable : ICommand {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="StationAvailable"/> class. 
    /// This station is available!
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    public StationAvailable(IPEndPoint sender) {
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
    public void Execute(Station receiver) {
      if (receiver.Manager.Equals(Sender)) return;
      receiver.MakePeerAvailableAndRefresh(Sender);
    }

    #endregion
  }
}
