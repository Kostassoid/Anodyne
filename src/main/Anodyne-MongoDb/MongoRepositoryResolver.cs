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

using Kostassoid.Anodyne.DataAccess;
using Kostassoid.Anodyne.Domain.Base;
using Kostassoid.Anodyne.Domain.DataAccess;
using Kostassoid.Anodyne.Domain.DataAccess.Operations;
using MongoDB.Driver;

namespace Kostassoid.Anodyne.MongoDb
{
    public class MongoRepositoryResolver : IRepositoryResolver
    {
        public IRepository<TRoot> Get<TRoot>(IDataSession dataSession) where TRoot : class, IAggregateRoot
        {
            return new MongoRepository<TRoot>((MongoDatabase)dataSession.NativeSession);
        }
    }
}