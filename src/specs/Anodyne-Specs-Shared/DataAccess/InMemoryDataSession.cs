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
    using System.Linq;
    using Anodyne.DataAccess;
    using System;
    using System.Collections.Generic;

    public class InMemoryDataSession : IDataSession
    {
        public IDictionary<object, IPersistableRoot> Roots { get; private set; }

        public InMemoryDataSession(IDictionary<object, IPersistableRoot> roots)
        {
            Roots = roots;
        }

        public IQueryable<T> Query<T>() where T : class, IPersistableRoot
        {
            return Roots.Values.OfType<T>().AsQueryable();
        }

        public T FindOne<T>(object id) where T : class, IPersistableRoot
        {
            return (T)FindOne(typeof (T), id);
        }

        public IPersistableRoot FindOne(Type type, object id)
        {
            return Roots.ContainsKey(id) ? Roots[id] : null;
        }

        public void SaveOne(IPersistableRoot o)
        {
            Roots[o.Identity] = o;
        }

        public void RemoveOne(Type type, object id)
        {
            Roots.Remove(id);
        }

        public object NativeSession { get { throw new InvalidOperationException("InMemoryDataSession doesn't need any native session");} }

        public void Dispose()
        { }
    }
}