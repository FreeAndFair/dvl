// Type: System.Collections.Generic.LinkedList`1
// Assembly: System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089
// Assembly location: C:\Windows\Microsoft.NET\Framework\v4.0.30319\System.dll

using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace System.Collections.Generic
{
    [DebuggerTypeProxy(typeof(System_CollectionDebugView<>))]
    [DebuggerDisplay("Count = {Count}")]
    [ComVisible(false)]
    [Serializable]
    public class LinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISerializable, IDeserializationCallback
    {
        #region Constructors and Destructors

        public LinkedList();

        public LinkedList(IEnumerable<T> collection);

        protected LinkedList(SerializationInfo info, StreamingContext context);

        #endregion

        #region Public Properties

        public int Count { get; }

        public LinkedListNode<T> First { get; }

        public LinkedListNode<T> Last { get; }

        #endregion

        #region Explicit Interface Properties

        bool ICollection.IsSynchronized { get; }

        object ICollection.SyncRoot { get; }

        bool ICollection<T>.IsReadOnly { get; }

        #endregion

        #region Public Methods

        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value);

        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode);

        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value);

        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode);

        public LinkedListNode<T> AddFirst(T value);

        public void AddFirst(LinkedListNode<T> node);

        public LinkedListNode<T> AddLast(T value);

        public void AddLast(LinkedListNode<T> node);

        public void Clear();

        public bool Contains(T value);

        public void CopyTo(T[] array, int index);

        public LinkedListNode<T> Find(T value);

        public LinkedListNode<T> FindLast(T value);

        public LinkedList<T>.Enumerator GetEnumerator();

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context);

        public virtual void OnDeserialization(object sender);

        public bool Remove(T value);

        public void Remove(LinkedListNode<T> node);

        public void RemoveFirst();

        public void RemoveLast();

        #endregion

        #region Explicit Interface Methods

        void ICollection.CopyTo(Array array, int index);

        void ICollection<T>.Add(T value);

        IEnumerator<T> IEnumerable<T>.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator();

        #endregion

        [Serializable]
        public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
        {
            #region Public Properties

            public T Current { get; }

            #endregion

            #region Explicit Interface Properties

            object IEnumerator.Current { get; }

            #endregion

            #region Public Methods

            public void Dispose();

            public bool MoveNext();

            #endregion

            #region Explicit Interface Methods

            void IDeserializationCallback.OnDeserialization(object sender);

            void IEnumerator.Reset();

            void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context);

            #endregion
        }
    }
}
