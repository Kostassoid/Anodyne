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
    public abstract class AggregateEvent<TRoot, TKey> : PersistentDomainEvent, IAggregateEvent where TRoot : AggregateRoot<TKey>
    {
        private readonly TRoot _aggregate;

        public TRoot Aggregate
        {
            get { return _aggregate; }
        }

        // should not be stored!
        public IAggregateRoot AggregateObject
        {
            get { return _aggregate; }
        }

        // should not be stored!
        public TKey AggregateId { get; protected set; }

        public object AggregateIdObject
        {
            get { return AggregateId; }
        }

        public int AggregateVersion { get; protected set; }

        private AggregateEvent()
        {
        }

        protected AggregateEvent(TRoot aggregate, DateTime happened, EventData data) : base(happened, data)
        {
            _aggregate = aggregate;
            AggregateId = aggregate.Id;
            AggregateVersion = aggregate.NewVersion();
        }

        protected AggregateEvent(TRoot aggregate, EventData data)
            : this(aggregate, SystemTime.Now, data)
        {
        }

        protected AggregateEvent(TRoot aggregate)
            : this(aggregate, SystemTime.Now, new EmptyEventData())
        {
        }
    }
}