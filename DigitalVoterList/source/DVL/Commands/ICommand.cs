#region Copyright and License

// // -----------------------------------------------------------------------
// // <copyright file="ICommand.cs" company="DemTech">
// // Copyright (C) 2013 Joseph Kiniry, DemTech, 
// // IT University of Copenhagen, Technical University of Denmark,
// // Nikolaj Aaes, Nicolai Skovvart
// // </copyright>
// // -----------------------------------------------------------------------
#endregion

namespace Aegis_DVL.Commands {
  using System.Diagnostics.Contracts;
  using System.Net;

  /// <summary>
  /// A command is sent over the network and can be executed at the destination.
  /// </summary>
  [ContractClass(typeof(CommandContract))] public interface ICommand {
    #region Public Properties

    /// <summary>
    /// Who sent this command?
    /// </summary>
    IPEndPoint Sender { get; }

    #endregion

    #region Public Methods and Operators

    /// <summary>
    /// Execute this command!
    /// </summary>
    /// <param name="receiver">
    /// The station the command will execute commands on.
    /// </param>
    void Execute(Station receiver);

    #endregion
  }

  /// <summary>
  /// The command contract.
  /// </summary>
  [ContractClassFor(typeof(ICommand))] public abstract class CommandContract : ICommand {
    #region Public Properties

    /// <summary>
    /// Gets the sender.
    /// </summary>
    public IPEndPoint Sender {
      get {
        Contract.Ensures(Contract.Result<IPEndPoint>() != null);
        return default(IPEndPoint);
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
    public void Execute(Station receiver) { Contract.Requires(receiver != null); }

    #endregion
  }
}
