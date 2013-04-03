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

namespace Kostassoid.Anodyne.MongoDb
{
    using System;
    using Abstractions.DataAccess;

    public class MongoDataProvider : IDataAccessProvider
    {
        public IDataSessionFactory SessionFactory { get; private set; }

		public MongoDataProvider(string connectionString, string databaseName)
		{
			RegisterClassMaps();

			SessionFactory = new MongoDataSessionFactory(NormalizeConnectionString(connectionString), databaseName);
		}

		public MongoDataProvider(Tuple<string, string> connectionStringAndDatabaseName)
			: this(connectionStringAndDatabaseName.Item1, connectionStringAndDatabaseName.Item2)
		{
		}

		private static void RegisterClassMaps()
        {
            //TODO: limit assembly selection
            var assemblies = From.AllAssemblies().ToList();//.Where(a => a.FullName.StartsWith("Anodyne") || a.FullName.StartsWith(systemNamespace)).ToList();

            MongoHelper.CreateMapForAllClassesBasedOn<IPersistable>(assemblies);
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