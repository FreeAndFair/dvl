#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="IDatabase.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Database {
  using System;
  using System.Collections.Generic;
  using System.Diagnostics.Contracts;

  using Aegis_DVL.Data_Types;

  /// <summary>
  ///   The database-layer is responsible for communicating with the database
  ///   (create, read, update, write). It can also perform batch-operations
  ///   such as importing and exporting the database.
  /// </summary>
  [ContractClass(typeof(DatabaseContract))] public interface IDatabase : IDisposable {
    #region Public Properties

    /// <summary>
    ///   What does the entire database look like?
    /// </summary>
    IEnumerable<Voter> AllVoters { [Pure] get; }

    IEnumerable<Precinct> AllPrecincts { [Pure] get; }

    /// <summary>
    ///   Who is my parent station?
    /// </summary>
    Station Parent { [Pure] get; }

    #endregion

    #region Public Indexers

    /// <summary>
    /// Has this voter received a ballot?
    ///   This voter has received a ballot!
    ///   This user's ballot has been revoked!
    /// </summary>
    /// <param name="voternumber">
    /// The voternumber to be checked.
    /// </param>
    /// <returns>
    /// The BallotStatus for the voternumber/cpr combination.
    /// </returns>
    BallotStatus this[VoterNumber voternumber] { [Pure] get; set; }

    /// <summary>
    /// Has this voter received a ballot?
    ///   This voter has received a ballot!
    ///   This user's ballot has been revoked!
    /// </summary>
    /// <param name="cpr">
    /// The CPR number to be checked.
    /// </param>
    /// <param name="masterPassword">
    /// The master password of the election.
    /// </param>
    /// <returns>
    /// The BallotStatus for the voternumber/cpr combination.
    /// </returns>
   // BallotStatus this[CPR cpr, string masterPassword] { [Pure] get; set; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Import this data into the database!
    /// </summary>
    /// <param name="data">
    /// A dataset to be imported.
    /// </param>
    void Import(IEnumerable<Voter> data);
    void Import(IEnumerable<Precinct> data);

    Voter GetVoterByVoterNumber(VoterNumber voternumber);
    Voter GetVoterByDLNumber(string dlnumber);
    Voter GetVoterByStateId(Int32 stateid);
    List<Voter> GetVotersBySearchStrings(string lastname, string firstname, string middlename,
                                         string address, string municipality, string zipcode);

    Precinct GetPrecinctBySplitId(string sid);

    #endregion
  }

  /// <summary>
  /// The database contract.
  /// </summary>
  [ContractClassFor(typeof(IDatabase))] public abstract class DatabaseContract : IDatabase {
    #region Public Properties

    /// <summary>
    /// Gets the all data.
    /// </summary>
    public IEnumerable<Voter> AllVoters {
      get {
        Contract.Ensures(Contract.Result<IEnumerable<Voter>>() != null);
        return default(IEnumerable<Voter>);
      }
    }

    public IEnumerable<Precinct> AllPrecincts {
      get {
        Contract.Ensures(Contract.Result<IEnumerable<Precinct>>() != null);
        return default(IEnumerable<Precinct>);
      }
    }

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent {
      get {
        Contract.Ensures(Contract.Result<Station>() != null);
        return default(Station);
      }
    }

    #endregion

    #region Public Indexers

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="voternumber">
    /// The voternumber.
    /// </param>
    /// <returns>
    /// The <see cref="BallotStatus"/>.
    /// </returns>
    public BallotStatus this[VoterNumber voternumber] {
      get { return default(BallotStatus); }
      set {
        // You can't set a ballot as unavailable. If it's in the DB, 
        // it's either received or not received.
        Contract.Requires(value != BallotStatus.Unavailable);

        // When you're setting a value, it must be in the DB
        Contract.Requires(this[voternumber] != BallotStatus.Unavailable);

        // You can only hand out or revoke ballots if they have the opposite state
        // If it's not handed out, you can hand it out
        Contract.Requires(
          value != BallotStatus.Received || this[voternumber] ==
          BallotStatus.NotReceived);

        // If it's handed out, you can revoke it.
        Contract.Requires(
          value != BallotStatus.NotReceived || this[voternumber] ==
          BallotStatus.Received);
        Contract.Ensures(this[voternumber] == value);
      }
    }

    /// <summary>
    /// The this.
    /// </summary>
    /// <param name="cpr">
    /// The cpr.
    /// </param>
    /// <param name="masterPassword">
    /// The master password.
    /// </param>
    /// <returns>
    /// The <see cref="BallotStatus"/>.
    /// </returns>
    /*
    public BallotStatus this[CPR cpr, string masterPassword] {
      get {
        Contract.Requires(masterPassword != null);
        Contract.Requires(this.Parent.ValidMasterPassword(masterPassword));
        return default(BallotStatus);
      }

      set {
        Contract.Requires(this.Parent.ValidMasterPassword(masterPassword));

        // You can't set a ballot as unavailable. If it's in the DB, it's 
        // either received or not received.
        Contract.Requires(value != BallotStatus.Unavailable);

        // When you're setting a value, it must be in the DB
        Contract.Requires(this[cpr, masterPassword] != BallotStatus.Unavailable);

        // You can only hand out or revoke ballots if they have the opposite state
        // If it's not handed out, you can hand it out
        Contract.Requires(value != BallotStatus.Received || this[cpr, masterPassword] == BallotStatus.NotReceived);

        // If it's handed out, you can revoke it.
        Contract.Requires(value != BallotStatus.NotReceived || this[cpr, masterPassword] == BallotStatus.Received);
        Contract.Ensures(this[cpr, masterPassword] == value);
      }
    }
    */

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The dispose.
    /// </summary>
    public void Dispose() { }

    /// <summary>
    /// The import.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    public void Import(IEnumerable<Voter> data) { Contract.Requires(data != null); }
    public void Import(IEnumerable<Precinct> data) { Contract.Requires(data != null); }

    public Voter GetVoterByVoterNumber(VoterNumber vn) { return default(Voter);  }
    public Voter GetVoterByDLNumber(string dlnumber) { Contract.Requires(dlnumber != null); return default(Voter); }
    public Voter GetVoterByStateId(Int32 sid) { return default(Voter); }
    public Precinct GetPrecinctBySplitId(string sid) { Contract.Requires(sid != null); return default(Precinct); }
    public List<Voter> GetVotersBySearchStrings(string a, string b, string c, string d, string e, string f) { return default(List<Voter>); }
    #endregion
  }
}
