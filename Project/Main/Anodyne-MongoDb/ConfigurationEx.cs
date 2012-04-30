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
    using System.Configuration;
    using DataAccess;
    using DataAccess.Operations;

    public static class ConfigurationEx
    {
        public static void UseMongoDataAccess(this IConfiguration configuration, string databaseServer, string databaseName)
        {
            var cfg = (configuration as IConfigurationSettings);

            cfg.Container.For<IDataSessionFactory>()
                .Use(() => new MongoDataSessionFactory(NormalizeConnectionString(databaseServer), databaseName, new ContainerOperationResolver(cfg.Container)));
            cfg.Container.ForAll<IDataOperation>().Use<MongoDataSessionFactory>();
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