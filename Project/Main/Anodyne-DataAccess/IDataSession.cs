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


namespace Kostassoid.Anodyne.DataAccess
{
    using Domain.Base;
    using Domain.Events;
    using Operations;
    using Wiring;
    using global::System;

    public interface IDataSession : IDisposable, IHandlerOf<IAggregateEvent>
    {
        IRepository<TRoot> GetRepository<TRoot>() where TRoot : class, IAggregateRoot;
        TOp GetOperation<TOp>() where TOp : class, IDataOperation;

        void MarkAsDeleted<TRoot>(TRoot aggregate) where TRoot : class, IAggregateRoot;

        DataChangeSet SaveChanges();
        void Rollback();
    }
}