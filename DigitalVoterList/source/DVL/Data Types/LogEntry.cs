#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="LogEntry.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Data_Types {
  using System;
  using System.Diagnostics.Contracts;
  using System.Net;

  /// <summary>
  /// A log entry is an entry in a log. It contains a message, a time, \
  /// and a level indicating its severity.
  /// </summary>
  [Serializable] public struct LogEntry {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LogEntry"/> struct. 
    /// May I have a new LogEntry?
    /// </summary>
    /// <param name="message">
    /// The object to log. Typically a string.
    /// </param>
    /// <param name="level">
    /// The severity-level of the log-entry. Typically Info
    /// </param>
    /// <param name="stationAddress">
    /// The station Address.
    /// </param>
    public LogEntry(object message, Level level, IPEndPoint stationAddress)
      : this() {
      Contract.Requires(message != null);
      Contract.Requires(stationAddress != null);

      StationAddress = stationAddress;
      Message = message;
      Level = level;
      Timestamp = DateTime.Now;
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// What type of log entry is this?
    /// </summary>
    public Level Level { get; private set; }

    /// <summary>
    /// What is the message of the log entry?
    /// </summary>
    public object Message { get; private set; }

    /// <summary>
    /// Where did the log entry originate from?
    /// </summary>
    public IPEndPoint StationAddress { get; private set; }

    /// <summary>
    /// At what time was the log entry added?
    /// </summary>
    public DateTime Timestamp { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() { return Timestamp + "; " + Level + "; " + Message + "; " + StationAddress; }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() {
      Contract.Invariant(Message != null);
      Contract.Invariant(!Equals(Level, null));
      Contract.Invariant(!Equals(Timestamp, null));
    }

    #endregion
  }
}
