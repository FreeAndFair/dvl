/// Contract class for IDataAcessObject.

namespace DBComm.DBComm.DAO
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Linq.Expressions;

    using global::DBComm.DBComm.DO;

    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "*", Justification = "Contract code")]
    [ContractClassFor(typeof(IDataAccessObject<>))]
    public abstract class IDataAccessObjectContract<T> : IDataAccessObject<T>
        where T : class, IDataObject
    {
        #region Implementation of IDataAccessObject<T>

        public bool Create(T t)
        {
            Contract.Requires(t != null);
            Contract.Requires(t.FullyInitialized());
            throw new NotImplementedException();
        }


        public bool Create(IEnumerable<T> t)
        {
            Contract.Requires(t != null);
            Contract.Requires(Contract.ForAll(t, ts => ts.FullyInitialized()));
            throw new NotImplementedException();
        }

        [Pure]
        public IEnumerable<T> Read(Expression<Func<T, bool>> f)
        {
            Contract.Requires(f != null);
            /*Contract.Ensures(Contract.ForAll(Contract.Result<IEnumerable<T>>(), r => f.Compile().Invoke(r)));
              This fails for more advanced predicates, i.e. ones that use associations.*/
            throw new NotImplementedException();
        }

        public bool Update(Expression<Func<T, bool>> f, T dummy)
        {
            Contract.Requires(f != null);
            Contract.Requires(dummy != null);
            Contract.Requires(dummy.PrimaryKey == null);
            throw new NotImplementedException();
        }

        public bool Delete(Expression<Func<T, bool>> f)
        {
            Contract.Requires(f != null);
            Contract.Ensures(!this.Read(f).Any()); // The result of a read operation does not have any elements.
            throw new NotImplementedException();
        }

        #endregion
    }
}