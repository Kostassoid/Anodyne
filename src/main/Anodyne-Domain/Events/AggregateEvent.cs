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

namespace Kostassoid.Anodyne.Domain.Events
{
    using System;
    using Common;
    using Base;

    [Serializable]
    public abstract class AggregateEvent<TRoot, TData> : PersistentDomainEvent<TData>, IAggregateEvent where TRoot : IAggregateRoot where TData : EventPayload
    {
        private readonly TRoot _aggregate;
        public object AggregateId { get; protected set; }

        // should not be stored!
        public TRoot Target { get { return _aggregate; } }

        // should not be stored!
        public IAggregateRoot Aggregate { get { return _aggregate; } }

        public int AggregateVersion { get; protected set; }

        protected AggregateEvent(TRoot aggregate, DateTime happened, TData data) : base(happened, data)
        {
            _aggregate = aggregate;
            AggregateId = aggregate.IdObject;
            AggregateVersion = aggregate.NewVersion();
        }

        protected AggregateEvent(TRoot aggregate, TData data)
            : this(aggregate, SystemTime.Now, data)
        {
        }

    }
}