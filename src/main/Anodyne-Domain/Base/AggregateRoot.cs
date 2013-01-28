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
    using System.Linq;
    using Common.Extentions;
    using Common.Reflection;
    using Events;
    using Wiring;

    [Serializable]
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        object IEntity.IdObject { get { return Id; } }

        public virtual int Version { get; protected set; }

        // ReSharper disable StaticFieldInGenericType
        private static bool _handlersBinded;
        private static readonly object _locker = new object();
        // ReSharper restore StaticFieldInGenericType

        protected AggregateRoot()
        {
            if (!_handlersBinded) EnsureAggregateEventsAreBinded();
        }

        public virtual int NewVersion()
        {
            return Version++;
        }

        protected static void Apply(IAggregateEvent @event)
        {
            EventBus.Publish(@event);
        }

        private static void EnsureAggregateEventsAreBinded()
        {
            lock (_locker)
            {
                if (_handlersBinded) return;
                _handlersBinded = true;

                AllTypes.BasedOn<IAggregateRoot>()
                    .Where(r => !r.IsAbstract && !r.IsInterface)
                    .ForEach(r => EventBus.Extentions.BindDomainEvents(r));
            }
        }
    }
}