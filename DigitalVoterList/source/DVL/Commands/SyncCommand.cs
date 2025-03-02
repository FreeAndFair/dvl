﻿#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="SyncCommand.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Commands {
  using System;
  using System.Diagnostics.Contracts;
  using System.Linq;
  using System.Net;

  using Aegis_DVL.Data_Types;
  using Aegis_DVL.Database;
  using Aegis_DVL.Logging;
  using Aegis_DVL.Util;

  /// <summary>
  /// The sync command.
  /// </summary>
  [Serializable] public class SyncCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _addresses.
    /// </summary>
    private readonly string[] _addresses;

    /// <summary>
    /// The _election data key.
    /// </summary>
    private readonly byte[] _electionDataKey;

    /// <summary>
    /// The _master pw hash.
    /// </summary>
    private readonly byte[] _masterPwHash;

    /// <summary>
    /// The _public keys.
    /// </summary>
    private readonly byte[][] _publicKeys;

    /// <summary>
    /// The _sender.
    /// </summary>
    private readonly string _sender;

    /// <summary>
    /// The voter data.
    /// </summary>
    private readonly Voter[] _voterData;

    private readonly Precinct[] _precinctData;

    private readonly PollingPlace _pollingplace;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SyncCommand"/> class. 
    /// Can I have a new command that synchronizes the target?
    /// </summary>
    /// <param name="parent">
    /// The parent-station sending the command.
    /// </param>
    public SyncCommand(Station parent) {
      Contract.Requires(parent != null);
      _sender = parent.Address.ToString();
      _voterData = parent.Database.AllVoters.ToArray();
      _precinctData = parent.Database.AllPrecincts.ToArray();
      _pollingplace = parent.PollingPlace;
      _addresses = parent.Peers.Keys.Select(endpoint => endpoint.ToString()).ToArray();
      _publicKeys = parent.Peers.Values.Select(key => key.Value.ToBytes()).ToArray();
      _masterPwHash = Bytes.FromFile("Master.pw");
      _electionDataKey = parent.Crypto.VoterDataEncryptionKey.Value.ToBytes();
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public IPEndPoint Sender {
      get {
        var pieces = _sender.Split(':');
        var ip = pieces[0];
        var port = pieces[1];
        return new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
      }
    }

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
      receiver.Crypto.VoterDataEncryptionKey = new AsymmetricKey(_electionDataKey.ToKey());
      receiver.MasterPassword = _masterPwHash;
      receiver.Logger = new Logger(receiver);

      for (var i = 0; i < _addresses.Length; i++) {
        var pieces = _addresses[i].Split(':');
        var ip = pieces[0];
        var port = pieces[1];
        var endPoint = new IPEndPoint(IPAddress.Parse(ip), int.Parse(port));
        if (!receiver.Address.Equals(endPoint)) receiver.AddPeer(endPoint, new AsymmetricKey(_publicKeys[i].ToKey()));
      }

      receiver.ImportData(_voterData, _precinctData);
      receiver.PollingPlace = _pollingplace;
      receiver.Logger.Log("Synchronized by " + Sender, Level.Info);
      receiver.Communicator.Send(new StationAvailable(receiver.Address), receiver.Manager);
      receiver.UI.SyncComplete();
    }

    #endregion
  }
}
