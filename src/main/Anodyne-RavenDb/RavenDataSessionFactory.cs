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

namespace Kostassoid.Anodyne.RavenDb
{
    using Abstractions.DataAccess;
    using Raven.Client;
    using Raven.Client.Document;

    public class RavenDataSessionFactory : IDataSessionFactory
    {
        protected string DatabaseName { get; private set; }
        protected IDocumentStore Store { get; private set; }

        public RavenDataSessionFactory(IDocumentStore store)
        {
            Store = store;
            DatabaseName = null;
        }

        public RavenDataSessionFactory(string url, string databaseName)
        {
            DatabaseName = databaseName;

            Store = new DocumentStore { Url = url };
            Store.Initialize();
        }

        public virtual IDataSession Open()
        {
            return new RavenDataSession(Store.OpenSession(DatabaseName));
        }
    }
}