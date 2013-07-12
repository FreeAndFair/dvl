#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="Core.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Data_Types {
  using System;
  using System.Diagnostics.Contracts;
  using System.Globalization;

  using Aegis_DVL.Util;

  using Org.BouncyCastle.Crypto;

  #region Crypto

  /// <summary>
  ///   Encrypted voterdata is the encrypted combination of CPR, 
  /// VoterNumber and BallotStatus.
  /// </summary>
  [Serializable] public struct EncryptedVoterData {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="EncryptedVoterData"/> struct.
    /// </summary>
    /// <param name="voternumber">
    /// The voternumber.
    /// </param>
    /// <param name="cpr">
    /// The cpr.
    /// </param>
    /// <param name="ballotstatus">
    /// The ballotstatus.
    /// </param>
    public EncryptedVoterData(CipherText voternumber, 
                              CipherText cpr, 
                              CipherText ballotstatus)
      : this() {
      this.VoterNumber = voternumber;
      this.CPR = cpr;
      this.BallotStatus = ballotstatus;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What is the encrypted ballot status of this encrypted voterdata?
    /// </summary>
    public CipherText BallotStatus { get; private set; }

    /// <summary>
    ///   What is the encrypted CPR-number of this encrypted voterdata?
    /// </summary>
    public CipherText CPR { get; private set; }

    /// <summary>
    ///   What is the encrypted voter-number of this encrypted voterdata?
    /// </summary>
    public CipherText VoterNumber { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() {
      return string.Format("VoterNumber: {0}; CPR: {1}; Ballot: {2}", 
        this.VoterNumber, this.CPR, this.BallotStatus);
    }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() {
      Contract.Invariant(!Equals(this.CPR, null));
      Contract.Invariant(!Equals(this.VoterNumber, null));
      Contract.Invariant(!Equals(this.BallotStatus, null));
    }

    #endregion
  }

  /// <summary>
  ///   CipherText is encrypted data.
  /// </summary>
  [Serializable] public struct CipherText {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CipherText"/> struct. 
    /// May I have a new ciphertext?
    /// </summary>
    /// <param name="cipher">
    /// The back ciphertext. Commonly a byte-array.
    /// </param>
    public CipherText(byte[] cipher)
      : this() {
      Contract.Requires(cipher != null);
      this.Value = cipher;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What does this CipherText look like?
    /// </summary>
    public byte[] Value { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="cipher">
    /// The cipher.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator byte[](CipherText cipher) { return cipher.Value; }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() { return this.Value.AsBase64(); }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() { Contract.Invariant(this.Value != null); }

    #endregion
  }

  /// <summary>
  ///   An asymmetric key can be used for either encryption or decryption of data.
  /// </summary>
  public struct AsymmetricKey {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="AsymmetricKey"/> struct. 
    /// May I have a new asymmetric key?
    /// </summary>
    /// <param name="key">
    /// The backing asymmetric crypto-key.
    /// </param>
    public AsymmetricKey(AsymmetricKeyParameter key)
      : this() {
      Contract.Requires(key != null);
      this.Value = key;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What does this a symmetric key look like?
    /// </summary>
    public AsymmetricKeyParameter Value { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="asymmetricKey">
    /// The asymmetric key.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator AsymmetricKeyParameter(AsymmetricKey asymmetricKey) { return asymmetricKey.Value; }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() { Contract.Invariant(this.Value != null); }

    #endregion
  }

  /// <summary>
  /// The symmetric key.
  /// </summary>
  public struct SymmetricKey {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="SymmetricKey"/> struct. 
    /// May I have a new symmetric key?
    /// </summary>
    /// <param name="key">
    /// The backing symmetric crypto-key.
    /// </param>
    public SymmetricKey(byte[] key)
      : this() {
      Contract.Requires(key != null);
      this.Value = key;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What does this symmetric key look like?
    /// </summary>
    public byte[] Value { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="symmetricKey">
    /// The symmetric key.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator byte[](SymmetricKey symmetricKey) { return symmetricKey.Value; }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() { Contract.Invariant(this.Value != null); }

    #endregion
  }

  #endregion

  /// <summary>
  ///   A message contains the ciphertexts of a symmetric key, a message 
  ///   encrypted with the symmetric key and a hash encrypted with the 
  ///   sender's public key. Used for secure communication.
  /// </summary>
  [Serializable] public struct Message {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="Message"/> struct. 
    /// May I have a new Message?
    /// </summary>
    /// <param name="symmetricKey">
    /// The asymetrically-encrypted symmetric key.
    /// </param>
    /// <param name="content">
    /// The content of the message. Should be a symmetrically encrypted command.
    /// </param>
    /// <param name="senderHash">
    /// The asymetrically-encrypted sender-hash of the content.
    /// </param>
    /// <param name="iv">
    /// The initilization-vector for the content.
    /// </param>
    public Message(CipherText symmetricKey, 
                   CipherText content, 
                   CipherText senderHash, 
                   byte[] iv) : this() {
      Contract.Requires(iv != null);

      this.SymmetricKey = symmetricKey;
      this.Command = content;
      this.SenderHash = senderHash;
      this.Iv = iv;
    }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What is the CipherText of the encrypted command?
    /// </summary>
    public CipherText Command { get; private set; }

    /// <summary>
    ///   What is the initialization vector used to encrypt the command?
    /// </summary>
    public byte[] Iv { get; private set; }

    /// <summary>
    ///   What is the CipherText of the senderhash of the command?
    /// </summary>
    public CipherText SenderHash { get; private set; }

    /// <summary>
    ///   What is the CipherText of the symmetric key used to encrypt the command?
    /// </summary>
    public CipherText SymmetricKey { get; private set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() {
      return string.Format(
        "Symmetric key: {0}\nValue: {1}\nSenderHash: {2}\nIv: {3}", 
        this.SymmetricKey, 
        this.Command, 
        this.SenderHash, 
        this.Iv.AsString());
    }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() {
      Contract.Invariant(!Equals(this.SymmetricKey, null));
      Contract.Invariant(!Equals(this.Iv, null));
      Contract.Invariant(!Equals(this.Command, null));
      Contract.Invariant(!Equals(this.SenderHash, null));
    }

    #endregion
  }

  #region Voter Data

  /// <summary>
  ///   A voternumber is a unique number used in conjunction with the 
  /// CPR-number to request a ballot.
  /// </summary>
  [Serializable] public struct VoterNumber {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="VoterNumber"/> struct. 
    /// May I have a new VoterNumber?
    /// </summary>
    /// <param name="value">
    /// The value of the voternumber.
    /// </param>
    public VoterNumber(uint value)
      : this() { this.Value = value; }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What does this voter-number look like?
    /// </summary>
    public uint Value { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="voterNum">
    /// The voter num.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator uint(VoterNumber voterNum) {
      return voterNum.Value;
    }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() {
      return this.Value.ToString(CultureInfo.InvariantCulture);
    }

    #endregion
  }

  /// <summary>
  ///   A CPR-number is a number and a unique identifier for a danish citizen, consisting of the birthdate and a number.
  /// </summary>
  [Serializable] public struct CPR {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="CPR"/> struct. 
    /// May I have a new CPR?
    /// </summary>
    /// <param name="value">
    /// The value of the CPR-number.
    /// </param>
    public CPR(uint value)
      : this() { this.Value = value; }

    #endregion

    #region Public Properties

    /// <summary>
    ///   What does this CPR-number look like?
    /// </summary>
    public uint Value { get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The op_ implicit.
    /// </summary>
    /// <param name="cpr">
    /// The cpr.
    /// </param>
    /// <returns>
    /// </returns>
    public static implicit operator uint(CPR cpr) { return cpr.Value; }

    /// <summary>
    /// The to string.
    /// </summary>
    /// <returns>
    /// The <see cref="string"/>.
    /// </returns>
    public override string ToString() { return this.Value.ToString(CultureInfo.InvariantCulture); }

    #endregion

    #region Methods

    /// <summary>
    /// The object invariant.
    /// </summary>
    [ContractInvariantMethod] private void ObjectInvariant() { Contract.Invariant(this.Value > 0); }

    #endregion
  }

  /// <summary>
  ///   A ballot status is used in conjunction with a cpr-number and a voternumber, and indicates
  ///   wheither status that indicates whether the ballot has been handed out, not handed out, or
  ///   if it is unavailable at the given election venue."
  /// </summary>
  public enum BallotStatus {
    /// <summary>
    /// The received.
    /// </summary>
    Received = 7, 

    /// <summary>
    /// The not received.
    /// </summary>
    NotReceived = 11, 

    /// <summary>
    /// The unavailable.
    /// </summary>
    Unavailable = 13
  }

  #endregion
}
