#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Logger.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Logging {
  using System;
  using System.Collections.Generic;
  using System.Data;
  using System.Data.SQLite;
  using System.Diagnostics.Contracts;
  using System.IO;
  using System.Linq;
  using System.Net;

  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Util;

  /// <summary>
  /// The logger.
  /// </summary>
  public class Logger : ILogger {
    #region Fields

    /// <summary>
    /// The _db.
    /// </summary>
    private readonly Entities _db;

    /// <summary>
    /// The _station address.
    /// </summary>
    private readonly IPEndPoint _stationAddress;

    /// <summary>
    /// The _i.
    /// </summary>
    private int _i;

    /// <summary>
    /// The _is disposed.
    /// </summary>
    private bool _isDisposed;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Logger"/> class. 
    /// May I have a new Logger?
    /// </summary>
    /// <param name="parent">
    /// The parent-station who the Logger belongs to.
    /// </param>
    /// <param name="logPrefix">
    /// The prefix of the file where the log should be saved.
    /// </param>
    public Logger(Station parent, string logPrefix = "Log") {
      Contract.Requires(parent != null);
      Contract.Requires(logPrefix != null);
      Contract.Ensures(_db != null);
      Contract.Ensures(_stationAddress == parent.Address);
      Contract.Ensures(_db.Connection.State == ConnectionState.Open);

      _stationAddress = parent.Address;
      string password = parent.MasterPassword.AsBase64();
      if (parent.Communicator != null) {
        logPrefix = logPrefix + "_" + parent.Communicator.GetIdentifyingString();
      }

      string logName = InitDb(parent, logPrefix, password);
      string conStr = string.Format(
        "metadata=res://*/Logging.LogModel.csdl|" +
        "res://*/Logging.LogModel.ssdl|" +
        "res://*/Logging.LogModel.msl;" +
        "provider=System.Data.SQLite;" +
        "provider connection string='Data Source={0}",
        logName);
      if (parent.IsMasterPasswordInUse) conStr += string.Format(";Password={0}'", password);
      else conStr += "'";
      _db = new Entities(conStr);
      _db.Connection.Open();
        
      Log("Logger created", Level.Info);
    }

    /// <summary>
    /// Finalizes an instance of the <see cref="Logger"/> class. 
    /// </summary>
    ~Logger() { Dispose(false); }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the export.
    /// </summary>
    public IEnumerable<LogEntry> Export { get { return _db.Logs.ToArray().Select(data => data.LogEntry.To<LogEntry>()); } }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() {
      if (!_isDisposed) Dispose(true);
      GC.SuppressFinalize(this);
    }

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
      Console.WriteLine(message);
        /*
      lock (_db) {
        _db.Logs.AddObject(
          Logging.Log.CreateLog(++_i, Bytes.From(new LogEntry(message, level, _stationAddress))));
        _db.SaveChanges();
      }*/
    }

    #endregion

    #region Methods

    /// <summary>
    /// Create a new logging DB and, if one already exists under the proposed name,
    ///   archive it with a suffix of the current time.  Only create a password-
    /// protected DB if theStation tells us to.
    /// </summary>
    /// <param name="logPrefix">
    /// the proposed log database name
    /// </param>
    /// <param name="password">
    /// the database password
    /// </param>
    private static string InitDb(Station theStation, string logPrefix, string password) {
      string logName = logPrefix + ".sqlite";
      if (File.Exists(logName)) {
        DateTime now = DateTime.Now;
        File.Move(
          logName, 
          string.Format(
            logPrefix + "_{0}.sqlite", 
            now.Date.Day + "." + now.Date.Month + "." + now.Date.Year +
            "-" + now.Hour + "." + now.Minute + "." + now.Second + "." + now.Millisecond));
      }

      SQLiteConnection.CreateFile(logName);
      string connString = string.Format("Data Source={0}", logName);
      if ((theStation.IsMasterPasswordInUse) && (password != null)) 
        connString += string.Format(";Password={0}", password);
      using (var db = new SQLiteConnection(connString)) {
        db.Open();
        using (SQLiteCommand cmd = db.CreateCommand()) {
          cmd.CommandText = "CREATE TABLE Logs(Id INTEGER PRIMARY KEY AUTOINCREMENT, LogEntry BLOB NOT NULL)";
          cmd.ExecuteNonQuery();
        }
      }

      return logName;
    }

    /// <summary>
    /// The dispose.
    /// </summary>
    /// <param name="disposing">
    /// The disposing.
    /// </param>
    private void Dispose(bool disposing) {
      _isDisposed = true;
      _db.SaveChanges();
      if (disposing) _db.Dispose();
    }

    #endregion
  }
}
