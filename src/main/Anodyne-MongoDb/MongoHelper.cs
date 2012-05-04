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
    using MongoDB.Bson.Serialization;
    using MongoDB.Driver;
    using MongoDB.Driver.Builders;
    using MongoDB.Driver.Wrappers;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MongoHelper
    {
        public static void EnsureCappedCollectionExists<T>(int collectionSizeMb) where T : IAggregateRoot
        {
            if (UnitOfWork.Current.IsNone)
                throw new InvalidOperationException("Must be inside UnitOfWork context");

            var database = ((IDataSessionEx) UnitOfWork.Current.Value.DataSession).NativeSession as MongoDatabase;
            var collectionName = typeof (T).Name;
            var collectionSize = collectionSizeMb*1024*1024;

            if (database.CollectionExists(collectionName))
            {
                if (!database.GetCollection<T>(collectionName).IsCapped())
                {
                    var result = database.RunCommand(new CommandWrapper(new {convertToCapped = collectionName, size = collectionSize}));
                    if (!result.Ok)
                    {
                        throw new Exception(string.Format("Unable to convert collection {0} to capped! Result: {1}", collectionName, result.ErrorMessage));
                    }
                }
            }

            if (!database.CollectionExists(collectionName))
            {
                var options = CollectionOptions.SetCapped(true);
                options.SetMaxSize(collectionSize);
                database.CreateCollection(collectionName, options);
            }
        }

        public static void CreateMapForAllClassesBasedOn<TBase>(IEnumerable<Assembly> assemblies)
        {
            BsonClassMap.RegisterClassMap<TBase>(cm =>
                                                     {
                                                         cm.AutoMap();
                                                         cm.SetDiscriminatorIsRequired(true);
                                                         cm.SetIsRootClass(true);
                                                     });


            var types = assemblies.SelectMany(s => s.GetTypes())
                .Where(typeof (TBase).IsAssignableFrom).Where(t => !t.ContainsGenericParameters);

            foreach (var type in types)
            {
                if (BsonClassMap.IsClassMapRegistered(type)) continue;
                BsonClassMap.LookupClassMap(type);
            }
        }
    }
}