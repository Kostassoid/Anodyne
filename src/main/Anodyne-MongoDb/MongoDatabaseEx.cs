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
    using Domain.Base;
    using MongoDB.Driver;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public static class MongoDatabaseEx
    {
        public static MongoCollection<TEntity> GetCollection<TEntity>(this MongoDatabase database) where TEntity : class, IAggregateRoot
        {
            return database.GetCollection<TEntity>(typeof (TEntity).Name);
        }

        public static MongoCollection GetCollection(this MongoDatabase database, Type type)
        {
            return database.GetCollection(type, type.Name);
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

    }
}