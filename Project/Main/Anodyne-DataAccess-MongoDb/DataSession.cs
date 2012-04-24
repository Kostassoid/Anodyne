using System;
using System.Collections.Generic;
using Kostassoid.Anodyne.Domain.Base;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
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
                throw new Exception(String.Format("Operation {0} wasn't found. Check your configuration.", typeof(TOp).Name));
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