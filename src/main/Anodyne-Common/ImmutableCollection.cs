// Copyright 2011-2013 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.Common
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class ImmutableCollection<T> : ICollection<T>, ICollection
    {
        private readonly object _lock = new object();
        private readonly ICollection<T> _inner;

        public ImmutableCollection(ICollection<T> inner)
        {
            _inner = inner;
        }

        public virtual int Count
        {
            get { return _inner.Count; }
        }
        public virtual object SyncRoot
        {
            get { return _lock; }
        }
        public virtual bool IsSynchronized
        {
            get { return false; }
        }
        public virtual bool IsReadOnly
        {
            get { return true; }
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual void Add(T item)
        {
            throw new NotSupportedException("This is a read only collection");
        }
        public virtual bool Remove(T item)
        {
            throw new NotSupportedException("This is a read only collection");
        }
        public virtual void Clear()
        {
            throw new NotSupportedException("This is a read only collection");
        }

        public virtual bool Contains(T item)
        {
            return _inner.Contains(item);
        }
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }
        public virtual void CopyTo(Array array, int index)
        {
            CopyTo(array.Cast<T>().ToArray(), index);
        }
    }
}