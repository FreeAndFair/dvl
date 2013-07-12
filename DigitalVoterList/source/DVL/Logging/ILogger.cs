#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ILogger.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Logging {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.Contracts;
  using System.Linq;

  using Aegis_DVL.Data_Types;

  /// <summary>
  /// A log is used to track events in the system.
  /// </summary>
  [ContractClass(typeof(LoggerContract))] public interface ILogger : IDisposable {
    #region Public Properties

    /// <summary>
    /// What does the entire log look like?
    /// </summary>
    IEnumerable<LogEntry> Export { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Log this message!
    /// </summary>
    /// <param name="message">
    /// The message to log.
    /// </param>
    /// <param name="level">
    /// The logging level the message.
    /// </param>
    void Log(object message, Level level);

    #endregion
  }

  /// <summary>
  /// The logger contract.
  /// </summary>
  [ContractClassFor(typeof(ILogger))] public abstract class LoggerContract : ILogger {
    #region Public Properties

    /// <summary>
    /// Gets the export.
    /// </summary>
    public IEnumerable<LogEntry> Export {
      get {
        Contract.Ensures(Contract.Result<IEnumerable<LogEntry>>() != null);
        return default(IEnumerable<LogEntry>);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// The log.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="level">
    /// The level.
    /// </param>
    public void Log(object message, Level level) {
      Contract.Requires(message != null);
      Contract.Ensures(
        this.Export.Any(
          entry => entry.Message.Equals(message) &&
                   entry.Level == level));
    }

    #endregion
  }
}
