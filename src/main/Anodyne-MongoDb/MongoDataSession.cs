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

namespace Kostassoid.Anodyne.MongoDb
{
    using DataAccess;
    using Domain.Base;
    using MongoDB.Bson;
    using MongoDB.Driver;
    using DataAccess.Operations;
    using MongoDB.Driver.Builders;
    using System;

    public class MongoDataSession : DataSession, IDataSessionEx
    {
        private readonly MongoDatabase _nativeSession;

        object IDataSessionEx.NativeSession
        {
            get { return _nativeSession; }
        }

        public MongoDataSession(MongoDatabase dataContext, IOperationResolver operationResolver) : base(operationResolver)
        {
            _nativeSession = dataContext;
        }

        public override IRepository<TRoot> GetRepository<TRoot>()
        {
            return new MongoRepository<TRoot>(_nativeSession);
        }

        private BsonValue GetIdValue(object id)
        {
            if (id is Guid) return (Guid)id;
            if (id is string) return (string)id;
            if (id is int) return (int)id;
            if (id is long) return (long)id;
            if (id is ObjectId) return (ObjectId)id;

            throw new InvalidOperationException(string.Format("Unsupported _id type : {0}", id.GetType().Name));
        }

        protected override IAggregateRoot FindOne(Type type, object id)
        {
            var collection = _nativeSession.GetCollection(type);
            return collection.FindOneByIdAs(type, GetIdValue(id)) as IAggregateRoot;
        }

        protected override void SaveOne(Type type, IAggregateRoot root)
        {
            var collection = _nativeSession.GetCollection(type);
            collection.Save(root);
        }

        protected override void RemoveOne(Type type, object id)
        {
            var collection = _nativeSession.GetCollection(type);
            collection.Remove(Query.EQ("_id", GetIdValue(id)));
        }

    }
}