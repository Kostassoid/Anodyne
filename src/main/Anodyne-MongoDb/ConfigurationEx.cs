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
    using Abstractions.DataAccess;
    using Abstractions.Dependency;
    using Abstractions.Dependency.Registration;
    using Domain.DataAccess;
    using Node.Configuration;
    using System;

    public static class ConfigurationEx
    {
        public static DataAccessTargetSelector UseMongoDataAccess(this INodeConfigurator nodeConfigurator, string name, string databaseServer, string databaseName)
        {
            var componentName = string.Format("DataAccessProvider-{0}", name);
            var cfg = nodeConfigurator.Configuration;

            if (cfg.Container.Has(componentName))
                throw new ArgumentException(string.Format("DataAccessProvider with name '{0}' already registered. Pick another name.", componentName), "name");

            var dataProvider = new MongoDataAccessProvider(cfg.SystemNamespace, databaseServer, databaseName);
            cfg.Container.Put(
                Binding.For<IDataAccessProvider>()
                .UseInstance(dataProvider)
                .With(Lifestyle.Singleton)
                .Named(name));

            //TODO: rethink multi-provider cases
            //only one domain data access is allowed
            if (!cfg.Container.Has<IRepositoryResolver>())
            {
                cfg.Container.Put(
                    Binding.For<IRepositoryResolver>()
                           .Use(() => new MongoRepositoryResolver())
                           .With(Lifestyle.Singleton));
            }

            return new DataAccessTargetSelector(cfg.Container, dataProvider);
        }

        public static DataAccessTargetSelector UseMongoDataAccess(this INodeConfigurator nodeConfigurator, string name, Tuple<string, string> databaseServerAndName)
        {
            return UseMongoDataAccess(nodeConfigurator, name, databaseServerAndName.Item1, databaseServerAndName.Item2);
        }

        public static DataAccessTargetSelector UseMongoDataAccess(this INodeConfigurator nodeConfigurator, string databaseServer, string databaseName)
        {
            return UseMongoDataAccess(nodeConfigurator, Guid.NewGuid().ToString("n"), databaseServer, databaseName);
        }

        public static DataAccessTargetSelector UseMongoDataAccess(this INodeConfigurator nodeConfigurator, Tuple<string, string> databaseServerAndName)
        {
            return UseMongoDataAccess(nodeConfigurator, databaseServerAndName.Item1, databaseServerAndName.Item2);
        }
    }
}