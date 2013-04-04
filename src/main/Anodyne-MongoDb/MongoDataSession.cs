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
    using MongoDB.Bson;
    using MongoDB.Driver;
    using System;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.Linq;

    internal class MongoDataSession : IDataSession
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
            Requires.True(typeof(IPersistableRoot).IsAssignableFrom(type), "type", "type should be IPersistableRoot");
        }

        public IPersistableRoot FindOne(Type type, object id)
        {
            EnsureTypeIsPersistable(type);

            var collection = _nativeSession.GetCollection(type);
            return (IPersistableRoot)collection.FindOneByIdAs(type, id.AsIdValue());
        }

		private static IMongoQuery BuildRootQuery(object id, long? specificVersion)
		{
			var idMatch = MongoDB.Driver.Builders.Query.EQ("_id", id.AsIdValue());

			return
				specificVersion.HasValue
				? MongoDB.Driver.Builders.Query.And(idMatch, MongoDB.Driver.Builders.Query.EQ("Version", new BsonInt64(specificVersion.Value)))
				: idMatch;
		}

	    public bool SaveOne(IPersistableRoot o, long? specificVersion)
        {
            var collection = _nativeSession.GetCollection(o.GetType());
		    try
		    {
				var result = collection.Update(BuildRootQuery(o.IdObject, specificVersion), Update.Replace(o), UpdateFlags.Upsert);
				return result.DocumentsAffected == 1;
		    }
			catch (WriteConcernException)
			{
				//TODO: can we do more?
				return false;
		    }
        }

		public bool RemoveOne(Type type, object id, long? specificVersion)
        {
            EnsureTypeIsPersistable(type);

            var collection = _nativeSession.GetCollection(type);
			var result = collection.Remove(BuildRootQuery(id, specificVersion));

			return result.DocumentsAffected == 1;
		}

        public void Dispose()
        { }
    }
}