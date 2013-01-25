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

namespace Kostassoid.Anodyne.Abstractions.DataAccess.Internal
{
    using System;
    using System.Linq;
    using Common.ExecutionContext;

    internal class DefaultDataAccessContextSession : IDataAccessContextSession
    {
        private readonly string _contextKey;
        private readonly IDataSession _dataSession;

        public DefaultDataAccessContextSession(string contextKey, IDataSession dataSession)
        {
            _contextKey = contextKey;
            _dataSession = dataSession;
        }

        public object NativeSession { get { return _dataSession.NativeSession; } }

        public void Dispose()
        {
            if (Context.FindAs<IDataSession>(_contextKey).IsSome)
                Context.Release(_contextKey);
        }

        public IQueryable<T> Query<T>() where T : class, IPersistableRoot
        {
            return _dataSession.Query<T>();
        }

        public T FindOne<T>(object id) where T : class, IPersistableRoot
        {
            return _dataSession.FindOne<T>(id);
        }

        public IPersistableRoot FindOne(Type type, object id)
        {
            return _dataSession.FindOne(type, id);
        }

        public void SaveOne(IPersistableRoot o)
        {
            _dataSession.SaveOne(o);
        }

        public void RemoveOne(Type type, object id)
        {
            _dataSession.RemoveOne(type, id);
        }
    }
}