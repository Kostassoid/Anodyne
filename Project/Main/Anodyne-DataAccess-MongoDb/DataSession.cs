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

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    using System;
    using System.Collections.Generic;
    using Domain.Base;
    using MongoDB.Bson;
    using MongoDB.Driver;

    public class DataSession : IDataSession, IDataSessionEx
    {
        private readonly MongoDatabase _nativeSession;
        private readonly IOperationResolver _operationResolver;

        object IDataSessionEx.NativeSession
        {
            get { return _nativeSession; }
        }

        private readonly ISet<IAggregateRoot> _newEntities = new HashSet<IAggregateRoot>();
        private readonly ISet<IAggregateRoot> _updatedEntities = new HashSet<IAggregateRoot>();
        private readonly ISet<IAggregateRoot> _deletedEntities = new HashSet<IAggregateRoot>();

        public DataSession(MongoDatabase dataContext, IOperationResolver operationResolver)
        {
            _nativeSession = dataContext;
            _operationResolver = operationResolver;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class, IAggregateRoot
        {
            return new Repository<TEntity>(_nativeSession);
        }

        public TOp GetOperation<TOp>() where TOp : class, IDataOperation
        {
            var operation = _operationResolver.Get<TOp>();

            if (operation == null)
            {
                throw new Exception(String.Format("Operation {0} wasn't found. Check your configuration.", typeof (TOp).Name));
            }

            return operation;
        }

        public object MarkAsCreated<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
        {
            _newEntities.Add(entity);
            return entity;
        }

        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
        {
            _deletedEntities.Add(entity);
        }

        public void MarkAsUpdated<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
        {
            _updatedEntities.Add(entity);
        }

        public void SaveChanges()
        {
            foreach (var entity in _newEntities)
            {
                var collection = _nativeSession.GetCollection(entity.GetType());
                collection.Save(entity);
            }
            _newEntities.Clear();

            foreach (var entity in _updatedEntities)
            {
                var collection = _nativeSession.GetCollection(entity.GetType());
                collection.Save(entity);
            }
            _updatedEntities.Clear();

            foreach (var entity in _deletedEntities)
            {
                var collection = _nativeSession.GetCollection(entity.GetType());
                collection.Remove(MongoDB.Driver.Builders.Query.EQ("_id", entity.IdObject.ToBson()));
            }
            _deletedEntities.Clear();
        }

        public void Rollback()
        {
            throw new NotSupportedException("Rollback operation is not supported");
        }

        public void Dispose()
        {
        }
    }
}