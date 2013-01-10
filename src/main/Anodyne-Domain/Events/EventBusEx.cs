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

namespace Kostassoid.Anodyne.Domain.Events
{
    using System;
    using Common;
    using Common.Reflection;
    using Wiring;

    public static class EventBusEx
    {
        public static Action BindDomainEvents<T>(this EventBusExtentions eventBus) where T : class
        {
            return EventBus
                .SubscribeTo()
                .AllBasedOn<IAggregateEvent>(From.Assemblies(a => typeof(T).Assembly == a)) //assuming our domain is one assembly with its events
                .With<T>(EventMatching.Strict, Priority.Critical)
                .As(e => e.Aggregate as T);
        }

        public static Action BindDomainEvents(this EventBusExtentions eventBus, Type aggregateType)
        {
            return EventBus
                .SubscribeTo()
                .AllBasedOn<IAggregateEvent>(From.Assemblies(a => aggregateType.Assembly == a)) //assuming our domain is one assembly with its events
                .With(aggregateType, EventMatching.Strict, Priority.Critical)
                .As(e => e.Aggregate as object);
        }

    }
}