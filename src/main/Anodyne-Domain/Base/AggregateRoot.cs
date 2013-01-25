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

namespace Kostassoid.Anodyne.Domain.Base
{
    using System;
    using System.Collections.Generic;
    using Events;
    using Wiring;

    [Serializable]
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        object IEntity.IdObject { get { return Id; } }

        public virtual int Version { get; protected set; }

// ReSharper disable StaticFieldInGenericType
        private static readonly ISet<Type> Binded = new HashSet<Type>();
// ReSharper restore StaticFieldInGenericType

        protected AggregateRoot()
        {
            //TODO: extract
            lock(Binded) EnsureAggregateEventsAreBindedFor(GetType());
        }

        private static void EnsureAggregateEventsAreBindedFor(Type aggregateType)
        {
            if (Binded.Contains(aggregateType)) return;

            EventBus.Extentions.BindDomainEvents(aggregateType);
            Binded.Add(aggregateType);
        }

        public virtual int NewVersion()
        {
            return Version++;
        }

        protected static void Apply(IAggregateEvent @event)
        {
            EventBus.Publish(@event);
        }
    }
}