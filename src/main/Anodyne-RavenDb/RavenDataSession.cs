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

namespace Kostassoid.Anodyne.RavenDb
{
    using System.Linq;
    using Abstractions.DataAccess;
    using System;
    using Raven.Client;

    internal class RavenDataSession : IDataSession
    {
        private readonly IDocumentSession _nativeSession;

        object IDataSessionEx.NativeSession
        {
            get { return _nativeSession; }
        }

        public RavenDataSession(IDocumentSession dataContext)
        {
            _nativeSession = dataContext;
        }

        public IQueryable<T> Query<T>() where T : class, IPersistableRoot
        {
            return _nativeSession.Query<T>();
        }

        public T FindOne<T>(object id) where T : class, IPersistableRoot
        {
            return _nativeSession.Load<T>(id.AsIdValue());
        }

        public IPersistableRoot FindOne(Type type, object id)
        {
            //TODO: just a prototype
            var method = _nativeSession.GetType().GetMethod("Load");
            return (IPersistableRoot)method.MakeGenericMethod(type).Invoke(_nativeSession, null);
        }

        public void SaveOne(IPersistableRoot o)
        {
            _nativeSession.Store(o, o.IdObject.AsIdValue());
        }

        public void RemoveOne(Type type, object id)
        {
            _nativeSession.Delete(FindOne(type, id));
        }

        public void Dispose()
        {
            _nativeSession.SaveChanges();
            _nativeSession.Dispose();
        }
    }
}