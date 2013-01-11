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
    using DataAccess;
    using Domain.Base;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using System;

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

        public object FindOne(Type type, object id)
        {
            var collection = _nativeSession.GetCollection(type);
            return collection.FindOneByIdAs(type, id.AsIdValue()) as IAggregateRoot;
        }

        public void SaveOne(Type type, object o)
        {
            var collection = _nativeSession.GetCollection(type);
            collection.Save(o);
        }

        public void RemoveOne(Type type, object id)
        {
            var collection = _nativeSession.GetCollection(type);
            collection.Remove(Query.EQ("_id", id.AsIdValue()));
        }

        public void Dispose()
        { }
    }
}