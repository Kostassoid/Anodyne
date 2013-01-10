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
    using Common.Reflection;
    using Domain.Base;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    using MongoDB.Driver.Builders;
    using MongoDB.Driver.Wrappers;

    public static class MongoDatabaseEx
    {
        public static MongoCollection<TEntity> GetCollection<TEntity>(this MongoDatabase database) where TEntity : class, IAggregateRoot
        {
            return database.GetCollection<TEntity>(GetCollectionNameFor(typeof(TEntity)));
        }

        public static MongoCollection GetCollection(this MongoDatabase database, Type type)
        {
            return database.GetCollection(type, GetCollectionNameFor(type));
        }

        private static string GetCollectionNameFor(Type type)
        {
            var collectionType = type;

            while (collectionType != null && !collectionType.BaseType.IsRawGeneric(typeof(AggregateRoot<>)))
            {
                collectionType = collectionType.BaseType;
            }

            if (collectionType == null)
                throw new ArgumentException(string.Format("Type {0} is not derived from AggregateRoot", type.Name));

            return collectionType.Name;
        }

        public static void MapAllClassesBasedOn<T>(this MongoDatabase database, IEnumerable<Assembly> assemblies)
        {
            MongoHelper.CreateMapForAllClassesBasedOn<T>(assemblies);
        }

        public static void EnsureIndexFor<TRoot>(this MongoDatabase database, Expression<Func<TRoot, object>> index) where TRoot : class, IAggregateRoot
        {
            database.GetCollection<TRoot>().EnsureIndex(index, false, true);
        }

        public static void EnsureUniqueIndexFor<TRoot>(this MongoDatabase database, Expression<Func<TRoot, object>> index) where TRoot : class, IAggregateRoot
        {
            database.GetCollection<TRoot>().EnsureIndex(index, true, true);
        }

        public static void EnsureCappedCollectionExists<T>(this MongoDatabase db, int collectionSizeMb) where T : class, IAggregateRoot
        {
            var collectionName = GetCollectionNameFor(typeof(T));
            var collectionSize = collectionSizeMb * 1024 * 1024;

            if (db.CollectionExists(collectionName))
            {
                if (!db.GetCollection<T>(collectionName).IsCapped())
                {
                    var result = db.RunCommand(new CommandWrapper(new { convertToCapped = collectionName, size = collectionSize }));
                    if (!result.Ok)
                    {
                        throw new Exception(string.Format("Unable to convert collection {0} to capped! Result: {1}", collectionName, result.ErrorMessage));
                    }
                }
            }

            if (!db.CollectionExists(collectionName))
            {
                var options = CollectionOptions.SetCapped(true);
                options.SetMaxSize(collectionSize);
                db.CreateCollection(collectionName, options);
            }

        }



    }
}