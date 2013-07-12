#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="CryptoCommand.cs" company="DemTech">
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
  /// The crypto command.
  /// </summary>
  [Serializable] public class CryptoCommand : ICommand {
    #region Fields

    /// <summary>
    /// The _content.
    /// </summary>
    private Message _content;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CryptoCommand"/> class. 
    /// May I have a new command that wraps and encrypts an inner command, to be transmitted securely?
    /// </summary>
    /// <param name="parent">
    /// The parent-station sending the command.
    /// </param>
    /// <param name="receiver">
    /// The address of the recipient.
    /// </param>
    /// <param name="innerCommand">
    /// The command to be wrapped.
    /// </param>
    public CryptoCommand(Station parent, IPEndPoint receiver, ICommand innerCommand) {
      Contract.Requires(parent != null);
      Contract.Requires(receiver != null);
      Contract.Requires(innerCommand != null);

      this.Sender = parent.Address;
      var crypto = parent.Crypto;

      var cmdBytes = Bytes.From(innerCommand);
      var symmetricKey = crypto.GenerateSymmetricKey();
      crypto.NewIv();

      var symmetricallyEncryptedCmdBytes = crypto.SymmetricEncrypt(cmdBytes, new SymmetricKey(symmetricKey));

      var targetKey = parent.IsManager && receiver.Equals(parent.Address)
                        ? parent.Crypto.Keys.Item1
                        : parent.Peers[receiver];
      var asymKeyBytes = crypto.AsymmetricEncrypt(symmetricKey, new AsymmetricKey(targetKey));

      var senderHashBytes = crypto.AsymmetricEncrypt(crypto.Hash(cmdBytes), new AsymmetricKey(crypto.Keys.Item2));

      var symmetricKeyCipher = new CipherText(asymKeyBytes);
      var commandCipher = new CipherText(symmetricallyEncryptedCmdBytes);
      var senderHash = new CipherText(senderHashBytes);
      var iv = crypto.Iv;

      this._content = new Message(symmetricKeyCipher, commandCipher, senderHash, iv);
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
      var crypto = receiver.Crypto;
      var symmetricKey = crypto.AsymmetricDecrypt(this._content.SymmetricKey, crypto.Keys.Item2);
      crypto.Iv = this._content.Iv;

      var cmd = crypto.SymmetricDecrypt(this._content.Command, new SymmetricKey(symmetricKey)).To<ICommand>();

      // Do we "know" the sender?
      if ((receiver.Peers.ContainsKey(cmd.Sender) || this.Sender.Equals(receiver.Address)) &&
          this.Sender.Equals(cmd.Sender)) {
        var key = receiver.Peers.ContainsKey(cmd.Sender) ? receiver.Peers[this.Sender] : receiver.Crypto.Keys.Item1;
        var senderHash = crypto.AsymmetricDecrypt(this._content.SenderHash, key);
        if (crypto.Hash(Bytes.From(cmd)).IsIdenticalTo(senderHash)) cmd.Execute(receiver);
        else receiver.ShutDownElection();
      } else receiver.ShutDownElection();
    }

    #endregion
  }
}
