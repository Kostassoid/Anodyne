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

using System.Linq;
using Kostassoid.Anodyne.Common.Reflection;
using Kostassoid.Anodyne.DataAccess;

namespace Kostassoid.Anodyne.MongoDb
{
    using Domain.Base;

    public class MongoDataAccessProvider : IDataAccessProvider
    {
        public IDataSessionFactory SessionFactory { get; private set; }

        public MongoDataAccessProvider(IDataSessionFactory sessionFactory)
        {
            SessionFactory = sessionFactory;
        }

        public MongoDataAccessProvider(string systemNamespace, string databaseServer, string databaseName)
        {
            RegisterClassMaps(systemNamespace);

            SessionFactory = new MongoDataSessionFactory(NormalizeConnectionString(databaseServer), databaseName);
        }

        private static void RegisterClassMaps(string systemNamespace)
        {
            var assemblies = From.AllAssemblies().Where(a => a.FullName.StartsWith("Anodyne") || a.FullName.StartsWith(systemNamespace)).ToList();

            //TODO: should register all subclasses
            //MongoHelper.CreateMapForAllClassesBasedOn<IPersistable>(assemblies);

            MongoHelper.CreateMapForAllClassesBasedOn<ValueObject>(assemblies);
            MongoHelper.CreateMapForAllClassesBasedOn<Entity>(assemblies);
        }

        private static string NormalizeConnectionString(string connectionString)
        {
            const string connectionStringPrefix = "mongodb://";
            if (connectionString.StartsWith(connectionStringPrefix))
                return connectionString;

            return connectionStringPrefix + connectionString;
        }
    }
}