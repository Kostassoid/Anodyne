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

using Kostassoid.Anodyne.Domain.DataAccess.Exceptions;
using Kostassoid.Anodyne.Domain.DataAccess.Operations;

namespace Kostassoid.Anodyne.Specs.Shared.DataAccess
{
    using Abstractions.DataAccess;
    using Common;
    using Common.Extentions;
    using Domain.Base;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class InMemoryRepository<TRoot> : IRepository<TRoot> where TRoot : class, IAggregateRoot
    {
        private readonly IList<TRoot> _collection;

        public InMemoryRepository(IDictionary<object, IPersistableRoot> roots)
        {
            _collection = roots.Values.OfType<TRoot>().ToList();
        }

        public virtual TRoot GetOne(object key)
        {
            var found = _collection.FirstOrDefault(r => ((IEntity) r).IdObject.Equals(key));
            if (found == null)
                throw new AggregateRootNotFoundException(key);

            return found.DeepClone();
        }

        public virtual Option<TRoot> FindOne(object key)
        {
            var found = _collection.FirstOrDefault(r => ((IEntity) r).IdObject.Equals(key));
            return found != null ? found.DeepClone() : null;
        }


        public virtual IQueryable<TRoot> All()
        {
            return _collection.AsQueryable().Select(r => r.DeepClone());
        }

        public virtual bool Exists(object key)
        {
            return FindOne(key).IsSome;
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
            get { return GetOne(key); }
        }
    }
}