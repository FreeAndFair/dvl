#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ICommunicator.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Communication {
  using System.Collections.Generic;
  using System.Diagnostics.Contracts;
  using System.Net;

  using Aegis_DVL.Commands;

  /// <summary>
  ///   A communicator is responsible for securely passing commands between two parties.
  /// </summary>
  [ContractClass(typeof(CommunicatorContract))] public interface ICommunicator {
    #region Public Properties

    /// <summary>
    ///   Who is my parent?
    /// </summary>
    Station Parent { get; }

    bool ThreadsStarted { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    ///   What are the addresses of machines in the local network?
    /// </summary>
    /// <returns>A collection of IPEndPoints containing the addresses of discovered computers.</returns>
    IEnumerable<IPEndPoint> DiscoverPeers();

    /// <summary>
    /// Gets the local end point for this instance of the pollbook. If 
    /// one does not exist, it is created. Note that depending on the
    /// implementation, the supplied port number may be ignored.
    /// </summary>
    /// <param name="port">
    /// the port number on which to create the local end point
    /// </param>
    /// <returns>
    /// The IPEndPoint representing the local end point.
    /// </returns>
    IPEndPoint GetLocalEndPoint(int port = 0);

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify this DVL.
    /// </summary>
    /// <returns>
    /// A string identifying this DVL.
    /// </returns>
    string GetIdentifyingString();

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify the DVL
    /// at the specified address.
    /// </summary>
    /// <param name="address">
    /// The IPEndPoint representing the address of the DVL
    /// </param>
    /// <returns>
    /// A string identifying the DVL represented by the specified address
    /// </returns>
    string GetIdentifyingStringForStation(IPEndPoint address);

    /// <summary>
    /// Is this machine listening on this port?
    /// </summary>
    /// <param name="address">
    /// The address of the machine to check.
    /// </param>
    /// <returns>
    /// True if the machine is listening on the port, false otherwise.
    /// </returns>
    bool IsListening(IPEndPoint address);

    /// <summary>
    /// Send this command securely to this target!
    /// </summary>
    /// <param name="command">
    /// The command to be sent.
    /// </param>
    /// <param name="target">
    /// The target that should receive and execute the command.
    /// </param>
    void Send(ICommand command, IPEndPoint target);

    void StartThreads();
    void StopThreads(); 

    #endregion
  }

  /// <summary>
  /// The communicator contract.
  /// </summary>
  [ContractClassFor(typeof(ICommunicator))] public abstract class CommunicatorContract : ICommunicator {
    #region Public Properties

    /// <summary>
    /// Gets the parent.
    /// </summary>
    public Station Parent {
      get {
        Contract.Ensures(Contract.Result<Station>() != null);
        return default(Station);
      }
    }

    public bool ThreadsStarted {
      get {
        return default(bool);
      }
    }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// The discover network machines.
    /// </summary>
    /// <returns>
    /// The <see cref="IEnumerable"/>.
    /// </returns>
    public IEnumerable<IPEndPoint> DiscoverPeers() {
      Contract.Ensures(Contract.Result<IEnumerable<IPEndPoint>>() != null);
      Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<IPEndPoint>>(), x => x != null));
      return default(IEnumerable<IPEndPoint>);
    }

    /// <summary>
    /// Gets the local end point for this instance of the pollbook. If 
    /// one does not exist, it is created.
    /// </summary>
    /// <returns>
    /// The IPEndPoint representing the local end point.
    /// </returns>
    public IPEndPoint GetLocalEndPoint(int port) {
      Contract.Ensures(Contract.Result<IPEndPoint>() != null);
      return default(IPEndPoint);
    }

    /// <summary>
    /// Destroys the local end point for this instance of the pollbook. 
    /// </summary>
    public void DestroyLocalEndPoint() {
      return;
    }

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify this DVL.
    /// </summary>
    /// <returns>
    /// A string identifying this DVL.
    /// </returns>
    public string GetIdentifyingString() {
      Contract.Ensures(Contract.Result<string>() != null);
      return default(string);
    }

    /// <summary>
    /// Gets a string (like an IP address or port number) that can identify the DVL
    /// at the specified address.
    /// </summary>
    /// <param name="address">
    /// The IPEndPoint representing the address of the DVL
    /// </param>
    /// <returns>
    /// A string identifying the DVL represented by the specified address
    /// </returns>
    public string GetIdentifyingStringForStation(IPEndPoint address) {
      Contract.Requires(address != null);
      Contract.Ensures(Contract.Result<string>() != null);
      return default(string);
    }

    /// <summary>
    /// The is listening.
    /// </summary>
    /// <param name="address">
    /// The address.
    /// </param>
    /// <returns>
    /// The <see cref="bool"/>.
    /// </returns>
    public bool IsListening(IPEndPoint address) {
      Contract.Requires(address != null);
      Contract.Requires(ThreadsStarted);
      return default(bool);
    }

    /// <summary>
    /// The send.
    /// </summary>
    /// <param name="command">
    /// The command.
    /// </param>
    /// <param name="target">
    /// The target.
    /// </param>
    public void Send(ICommand command, IPEndPoint target) {
      Contract.Requires(target != null);
      Contract.Requires(command != null);
    }

    public void StartThreads() {
      Contract.Ensures(ThreadsStarted);
    }

    public void StopThreads() {
      Contract.Ensures(!ThreadsStarted);
    }

    #endregion
  }
}
