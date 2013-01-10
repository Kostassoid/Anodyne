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

namespace Kostassoid.Anodyne.Specs.Shared.DataAccess
{
    using Anodyne.DataAccess;
    using Anodyne.DataAccess.Operations;
    using Domain.Base;
    using System;
    using System.Collections.Generic;

    public class InMemoryDataSession : DataSession
    {
        private readonly IDictionary<object, IAggregateRoot> _roots;

        public InMemoryDataSession(IOperationResolver operationResolver, IDictionary<object, IAggregateRoot> roots)
            : base(operationResolver)
        {
            _roots = roots;
        }

        public override IRepository<TRoot> GetRepository<TRoot>()
        {
            return new Repository<TRoot>(_roots);
        }

        protected override IAggregateRoot FindOne(Type type, object id)
        {
            return _roots.ContainsKey(id) ? _roots[id] : null;
        }

        protected override void SaveOne(Type type, IAggregateRoot root)
        {
            _roots[root.IdObject] = root;
        }

        protected override void RemoveOne(Type type, object id)
        {
            _roots.Remove(id);
        }
    }
}