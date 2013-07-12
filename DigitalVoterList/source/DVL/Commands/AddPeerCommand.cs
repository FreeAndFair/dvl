#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="AddPeerCommand.cs" company="DemTech">
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
  using Aegis_DVL.Util;

  /// <summary>
  /// The add peer command.
  /// </summary>
  [Serializable] public class AddPeerCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _peer.
    /// </summary>
    private readonly IPEndPoint _peer;

    /// <summary>
    /// The _peer public asymmetric key.
    /// </summary>
    private readonly byte[] _peerPublicAsymmetricKey;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AddPeerCommand"/> class. 
    /// May I have a new command that tells the target to add a peer to its peer-list?
    /// </summary>
    /// <param name="sender">
    /// The address of the one sending the command.
    /// </param>
    /// <param name="peer">
    /// The address of the peer to be added to the peer-list.
    /// </param>
    /// <param name="peerPublicAsymmetricKey">
    /// The public key of the peer to be added to the peer-list.
    /// </param>
    public AddPeerCommand(IPEndPoint sender, IPEndPoint peer, AsymmetricKey peerPublicAsymmetricKey) {
      Contract.Requires(sender != null);
      Contract.Requires(peer != null);

      this._peer = peer;
      this._peerPublicAsymmetricKey = peerPublicAsymmetricKey.Value.ToBytes();
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
    public void Execute(Station receiver) { if (receiver.Manager.Equals(this.Sender)) receiver.AddPeer(this._peer, new AsymmetricKey(this._peerPublicAsymmetricKey.ToKey())); }

    #endregion
  }
}
