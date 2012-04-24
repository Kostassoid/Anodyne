using System;
using Kostassoid.Anodyne.Domain.Base;
using MongoDB.Driver;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public static class MongoDatabaseEx
    {
        public static MongoCollection<TEntity> GetCollection<TEntity>(this MongoDatabase database) where TEntity : class, IAggregateRoot
        {
            return database.GetCollection<TEntity>(typeof(TEntity).Name);
        }

        public static MongoCollection GetCollection(this MongoDatabase database, Type type)
        {
            return database.GetCollection(type, type.Name);
        }

    }
}