// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AbstractDataAccessObject.cs" company="DVL">
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
    using System.Linq;
    using System.Linq.Expressions;

    using global::DBComm.DBComm.DO;

    /// <summary>
    /// An abstract implementation of the DAO interface. All methods are fully implemented, and 
    /// a concrete implementation need only provide the type of entity that the DAO should return /
    /// work with via the type parameter. 
    /// </summary>
    /// <typeparam name="T">The type of objects this DAO handles.
    /// </typeparam>
    public abstract class AbstractDataAccessObject<T> : IDataAccessObject<T>
        where T : class, IDataObject
    {
        /// <summary>
        /// The database.
        /// </summary>
        protected readonly DigitalVoterList db;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDataAccessObject{T}"/> class, 
        /// connects to the default instance.
        /// </summary>
        protected AbstractDataAccessObject()
        {
            this.db = DigitalVoterList.GetDefaultInstance();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractDataAccessObject{T}"/> class, 
        /// that connects to the specified DataContext.
        /// </summary>
        /// <param name="dc">
        /// The datacontext
        /// </param>
        protected AbstractDataAccessObject(DigitalVoterList dc)
        {
            this.db = dc;
        }

        /// <summary>
        /// Create this object.
        /// </summary>
        /// <param name="t">
        /// The object to insert.
        /// </param>
        /// <returns>
        /// True if the object was created, false otherwise.
        /// </returns>
        public bool Create(T t)
        {
            this.db.GetTable<T>().InsertOnSubmit(t);
            this.db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// Create all these objects.
        /// </summary>
        /// <param name="t">
        /// The objects to insert.
        /// </param>
        /// <returns>
        /// True if these objects were created, false otherwise.
        /// </returns>
        public bool Create(IEnumerable<T> t)
        {
            this.db.GetTable<T>().InsertAllOnSubmit(t);
            this.db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// Return all of the objects for which this predicate holds.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <returns>
        /// An iterable collection of objects matching the predicate.
        /// </returns>
        public IEnumerable<T> Read(Expression<Func<T, bool>> f)
        {
            return this.db.GetTable<T>().AsQueryable().Where(f);
        }

        /// <summary>
        /// Update all of the objects for which this predicate holds.
        /// Note that when trying to update the primary key, the dummy 
        /// object must be fully initialized, since the old record will be
        /// deleted, and a new one inserted.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <param name="dummy">
        /// The dummy object representing the new values to update to. If a value is null, it is not updated.
        /// </param>
        /// <returns>
        /// True if the update was successful, false otherwise.
        /// </returns>
        public bool Update(Expression<Func<T, bool>> f, T dummy)
        {
            IQueryable<T> oldValues = this.Read(f) as IQueryable<T>;

            if (oldValues != null)
            {
                foreach (var oldValue in oldValues)
                {
                    oldValue.UpdateValues(dummy);
                }
            }

            this.db.SubmitChanges();

            return true;
        }

        /// <summary>
        /// Delete all of the objects for which this predicate holds.
        /// </summary>
        /// <param name="f">
        /// The predicate to select by (where).
        /// </param>
        /// <returns>
        /// True if the delete was successful, false otherwise.
        /// </returns>
        public bool Delete(Expression<Func<T, bool>> f)
        {
            var oldValues = this.Read(f);

            if (oldValues != null)
            {
                this.db.GetTable<T>().DeleteAllOnSubmit(oldValues);
            }

            this.db.SubmitChanges();

            return true;
        }
    }
}