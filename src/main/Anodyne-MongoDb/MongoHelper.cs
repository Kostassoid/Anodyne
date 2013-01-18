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
    using System;
    using Common.Extentions;
    using MongoDB.Bson.Serialization;

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public static class MongoHelper
    {
        private static IEnumerable<Type> FindClosestClassesTo<TInterface>(IEnumerable<Assembly> assemblies) where TInterface : class // closest to "any interface"
        {
            return assemblies.SelectMany(s => s.GetTypes())
                      .Where(typeof(TInterface).IsAssignableFrom)
                      .Where(t => t.BaseType == typeof(object));
        }

        private static BsonClassMap CreateBsonClassMapFor(Type type)
        {
            // stolen from official MongoDb Driver sources
            var classMapDefinition = typeof(BsonClassMap<>);
            var classMapType = classMapDefinition.MakeGenericType(type);
            return (BsonClassMap)Activator.CreateInstance(classMapType);
        }

        private static void RegisterSubclasses(Type rootType, IEnumerable<Assembly> assemblies)
        {
            var types = assemblies.SelectMany(s => s.GetTypes())
                .Where(rootType.IsAssignableFrom)
                .Where(t => !t.ContainsGenericParameters && !t.IsInterface && t != rootType);

            types
                .Where(t => !BsonClassMap.IsClassMapRegistered(t))
                .ForEach(t => BsonClassMap.LookupClassMap(t));
        }

        private static void RegisterRoot<TBase>(IEnumerable<Assembly> assemblies) where TBase : class
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(TBase)))
                BsonClassMap.RegisterClassMap<TBase>(
                    cm =>
                    {
                        cm.AutoMap();
                        cm.SetDiscriminatorIsRequired(true);
                        cm.SetIsRootClass(true);
                    });

            RegisterSubclasses(typeof(TBase), assemblies);
        }

        private static void RegisterRoot(Type rootType, IEnumerable<Assembly> assemblies)
        {
            var rootClassMap = CreateBsonClassMapFor(rootType);
            rootClassMap.AutoMap();
            rootClassMap.SetDiscriminatorIsRequired(true);
            rootClassMap.SetIsRootClass(true);

            if (!BsonClassMap.IsClassMapRegistered(rootType))
                BsonClassMap.RegisterClassMap(rootClassMap);

            RegisterSubclasses(rootType, assemblies);
        }

        public static void CreateMapForAllClassesBasedOn<TBase>(IList<Assembly> assemblies) where TBase : class
        {
            if (!typeof(TBase).IsInterface)
            {
                RegisterRoot<TBase>(assemblies);
            }
            else
            {
                var concreteRoots = FindClosestClassesTo<TBase>(assemblies);

                concreteRoots.ForEach(t => RegisterRoot(t, assemblies));
            }
        }
    }
}