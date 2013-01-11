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
    using DataAccess;
    using MongoDB.Driver;

    public class MongoDataSessionFactory : IDataSessionFactory
    {
        protected string DatabaseName { get; private set; }
        protected MongoServer Server { get; private set; }

        public MongoDataSessionFactory(string connectionString, string databaseName)
        {
            DatabaseName = databaseName;

            Server = new MongoClient(connectionString).GetServer();
        }

        public virtual IDataSession Open()
        {
            //reusing database connection (ref: http://www.mongodb.org/display/DOCS/CSharp+Driver+Tutorial#CSharpDriverTutorial-Threadsafety)
            return new MongoDataSession(Server.GetDatabase(DatabaseName));
        }
    }
}