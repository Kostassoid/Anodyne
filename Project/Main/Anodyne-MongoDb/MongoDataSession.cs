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
            return new Repository<TRoot>(_nativeSession);
        }

        protected override bool ApplyChangeSet(AggregateRootChangeSet changeSet)
        {
            var type = changeSet.Aggregate.GetType();

            var collection = _nativeSession.GetCollection(type);

            var storedAggregate = collection.FindOneByIdAs(type, changeSet.Aggregate.IdObject.ToBson());

            if (storedAggregate == null && !changeSet.IsNew)
                return false;

// ReSharper disable PossibleNullReferenceException
            if ((storedAggregate as IAggregateRoot).Version != changeSet.TargetVersion)
// ReSharper restore PossibleNullReferenceException
                return false;

            collection.Save(changeSet.Aggregate);

            if (changeSet.IsDeleted)
                collection.Remove(MongoDB.Driver.Builders.Query.EQ("_id", changeSet.Aggregate.IdObject.ToBson()));

            return true;
        }

    }
}