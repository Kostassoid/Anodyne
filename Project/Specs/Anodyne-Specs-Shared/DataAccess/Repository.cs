// Copyright 2011-2012 Anodyne.
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

namespace Kostassoid.Anodyne.Specs.Shared.DataAccess
{
    using Common;
    using Common.Extentions;
    using Domain.Base;
    using Anodyne.DataAccess.Exceptions;
    using Anodyne.DataAccess.Operations;
    using global::System;
    using global::System.Collections.Generic;
    using global::System.Linq;
    using global::System.Linq.Expressions;

    public class Repository<TRoot> : IRepository<TRoot> where TRoot : class, IAggregateRoot
    {
        private readonly IList<TRoot> _collection;

        public Repository(IDictionary<object, IAggregateRoot> roots)
        {
            _collection = roots.Values.Cast<TRoot>().ToList();
        }

        public virtual TRoot Get(object key)
        {
            var found = _collection.FirstOrDefault(r => r.IdObject.Equals(key));
            if (found == null)
                throw new AggregateRootNotFoundException(key);

            return found.DeepClone();
        }

        public virtual Option<TRoot> FindBy(object key)
        {
            var found = _collection.FirstOrDefault(r => r.IdObject.Equals(key));
            return found != null ? found.DeepClone() : null;
        }


        public virtual IQueryable<TRoot> All()
        {
            return _collection.AsQueryable().Select(r => r.DeepClone());
        }

        public virtual bool Exists(object key)
        {
            return FindBy(key).IsSome;
        }

        public long Count(Expression<Func<TRoot, bool>> criteria)
        {
            return _collection.AsQueryable().Count(criteria);
        }

        public virtual long Count()
        {
            return _collection.Count();
        }

        public virtual TRoot this[object key]
        {
            get { return Get(key); }
        }
    }
}