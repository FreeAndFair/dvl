// -----------------------------------------------------------------------
// <copyright file="IDataObject.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace DBComm.DBComm.DO
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [ContractClass(typeof(IDataObjectContracts))]
    public interface IDataObject
    {
        /// <summary>
        /// Gets the primary key of this data object.
        /// </summary>
        uint? PrimaryKey { get; }

        /// <summary>
        /// Has all the fields of the object been initialized?
        /// </summary>
        /// <returns>
        /// True if all fields are non-null.
        /// </returns>
        bool FullyInitialized();

        /// <summary>
        /// Set the values of this object to the values of the dummy object, if the dummys value is non-null.
        /// </summary>
        /// <param name="dummy">
        /// The dummy object, representing new values.
        /// </param>
        void UpdateValues(IDataObject dummy);

        /// <summary>
        /// Reset all associations.
        /// </summary>
        void ResetAssociations();
    }
}
