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

namespace Kostassoid.Anodyne.MongoDb
{
    using System.Linq;
    using Abstractions.DataAccess;
    using Common.CodeContracts;
    using MongoDB.Driver;
    using System;
    using MongoDB.Driver.Linq;

    public class MongoDataSession : IDataSession
    {
        private readonly MongoDatabase _nativeSession;

        object IDataSessionEx.NativeSession
        {
            get { return _nativeSession; }
        }

        public MongoDataSession(MongoDatabase dataContext)
        {
            _nativeSession = dataContext;
        }

        public IQueryable<T> Query<T>() where T : class, IPersistableRoot
        {
            return _nativeSession.GetCollection<T>().AsQueryable();
        }

        public T FindOne<T>(object id) where T : class, IPersistableRoot
        {
            return (T)FindOne(typeof (T), id);
        }

        private static void EnsureTypeIsPersistable(Type type)
        {
            Requires.True(typeof(IPersistableRoot).IsAssignableFrom(type), "type", "type should be IPersistable");
        }

        public IPersistableRoot FindOne(Type type, object id)
        {
            EnsureTypeIsPersistable(type);

            var collection = _nativeSession.GetCollection(type);
            return (IPersistableRoot)collection.FindOneByIdAs(type, id.AsIdValue());
        }

        public void SaveOne(IPersistableRoot o)
        {
            var collection = _nativeSession.GetCollection(o.GetType());
            collection.Save(o);
        }

        public void RemoveOne(Type type, object id)
        {
            EnsureTypeIsPersistable(type);

            var collection = _nativeSession.GetCollection(type);
            collection.Remove(MongoDB.Driver.Builders.Query.EQ("_id", id.AsIdValue()));
        }

        public void Dispose()
        { }
    }
}