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
    using Common.Extentions;
    using MongoDB.Bson.Serialization;

    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class MongoHelper
    {
        public static void CreateMapForAllClassesBasedOn<TBase>(IEnumerable<Assembly> assemblies)
        {
            if (!typeof(TBase).IsInterface)
            {
                BsonClassMap.RegisterClassMap<TBase>(
                    cm =>
                        {
                            cm.AutoMap();
                            cm.SetDiscriminatorIsRequired(true);
                            cm.SetIsRootClass(true);
                        });
            }

            var types = assemblies.SelectMany(s => s.GetTypes())
                .Where(typeof(TBase).IsAssignableFrom).Where(t => !t.ContainsGenericParameters);

            types
                .Where(t => !BsonClassMap.IsClassMapRegistered(t))
                .ForEach(t => BsonClassMap.LookupClassMap(t));
        }
    }
}