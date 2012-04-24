using System;
using System.Linq;
using System.Linq.Expressions;
using Kostassoid.Anodyne.Common;
using Kostassoid.Anodyne.Domain.Base;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public class Repository<TEntity>: IRepository<TEntity> where TEntity : class, IAggregateRoot
    {

        private readonly MongoDatabase _session;
        private readonly Lazy<MongoCollection<TEntity>> _collection;

        public Repository(MongoDatabase session)
        {
            _session = session;
            _collection = new Lazy<MongoCollection<TEntity>>(session.GetCollection<TEntity>);
        }

        protected virtual MongoDatabase Session
        {
            get { return _session; }
        }

        public virtual TEntity Get(object key)
        {
            var found = _collection.Value.FindOne(Query.EQ("_id", key.ToBson()));
            if (found == null)
                throw new EntityNotFoundException(key);

            return found;

        }

        public virtual Option<TEntity> FindBy(object key)
        {
            return _collection.Value.FindOne(Query.EQ("_id", key.ToBson()));
        }


        public virtual IQueryable<TEntity> All()
        {
            return _collection.Value.AsQueryable();
        }

        public virtual bool Exists(object key)
        {
            return FindBy(key).IsSome;
        }

        public long Count(Expression<Func<TEntity, bool>> criteria)
        {
            return _collection.Value.AsQueryable().Count(criteria);
        }

        public virtual long Count()
        {
            return _collection.Value.Count();
        }

        public virtual TEntity this[object key]
        {
            get { return Get(key); }
        }
    }
}