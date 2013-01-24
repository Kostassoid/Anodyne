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

namespace Kostassoid.Anodyne.Domain.DataAccess
{
    using Exceptions;
    using Operations;
    using Abstractions.DataAccess;
    using Common;
    using Common.Extentions;
    using Base;
    using System;
    using System.Linq;
    using System.Linq.Expressions;

    public class Repository<TRoot> : IRepository<TRoot> where TRoot : class, IAggregateRoot
    {
        private readonly IDataSession _dataSession;

        public Repository(IDataSession dataSession)
        {
            _dataSession = dataSession;
        }

        public virtual TRoot GetOne(object key)
        {
            var found = _dataSession.FindOne<TRoot>(key);
            if (found == null)
                throw new AggregateRootNotFoundException(key);

            return found.DeepClone();
        }

        public virtual Option<TRoot> FindOne(object key)
        {
            return _dataSession.FindOne<TRoot>(key);
        }


        public virtual IQueryable<TRoot> All()
        {
            return _dataSession.Query<TRoot>();
        }

        public virtual bool Exists(object key)
        {
            return FindOne(key).IsSome;
        }

        public long Count(Expression<Func<TRoot, bool>> criteria)
        {
            return _dataSession.Query<TRoot>().Count(criteria);
        }

        public virtual long Count()
        {
            return _dataSession.Query<TRoot>().Count();
        }

        public virtual TRoot this[object key]
        {
            get { return GetOne(key); }
        }
    }
}