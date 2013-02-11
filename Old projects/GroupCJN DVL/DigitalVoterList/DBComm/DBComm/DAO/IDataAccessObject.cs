// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDataAccessObject.cs" company="DVL">
//   Jan Meier
// </copyright>
// <summary>
//   
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace DBComm.DBComm.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq.Expressions;

    using global::DBComm.DBComm.DO;

    /// <summary>
    /// Interface describing the various operations supported by a DAO that is 
    /// operation via LINQ.
    /// </summary>
    /// <typeparam name="T">The type of data objects that the DAO handles.
    /// </typeparam>
    [ContractClass(typeof(IDataAccessObjectContract<>))]
    public interface IDataAccessObject<T> where T : class, IDataObject
    {
        /// <summary>
        /// Create this object.
        /// </summary>
        /// <param name="t">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// True if the object was created, false otherwise.
        /// </returns>
        bool Create(T t);

        /// <summary>
        /// Create all these objects.
        /// </summary>
        /// <param name="t">
        /// The objects to insert.
        /// </param>
        /// <returns>
        /// True if these objects were created, false otherwise.
        /// </returns>
        bool Create(IEnumerable<T> t);

        /// <summary>
        /// Return all of the objects for which this predicate holds.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <returns>
        /// An iterable collection of objects matching the predicate.
        /// </returns>
        IEnumerable<T> Read(Expression<Func<T, bool>> f);

        /// <summary>
        /// Update all of the objects for which this predicate holds.
        /// Note that when trying to update the primary key, the dummy 
        /// object must be fully initialized, since the old record will be
        /// deleted, and a new one inserted.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <param name="t">
        /// The object to update to.
        /// </param>
        /// <returns>
        /// True if the update was successful, false otherwise.
        /// </returns>
        bool Update(Expression<Func<T, bool>> f, T dummy);

        /// <summary>
        /// Delete all of the objects for which this predicate holds.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <returns>
        /// True if the delete was successful, false otherwise.
        /// </returns>
        bool Delete(Expression<Func<T, bool>> f);
    }
}