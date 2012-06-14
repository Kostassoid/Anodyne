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

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    using System;
    using Common;
    using Internal;
    using Subscription;

    internal class PredicateSourceSyntax<TEvent> : IPredicateSourceSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public PredicateSourceSyntax(IEventAggregator eventAggregator)
        {
            _specification = new SubscriptionSpecification<TEvent>(eventAggregator);
        }

        public Action With(IHandlerOf<TEvent> handler, Priority priority)
        {
            return new TargetSyntax<TEvent>(_specification).With(handler, priority);
        }

        public Action With(Action<TEvent> action, Priority priority)
        {
            return new TargetSyntax<TEvent>(_specification).With(action, priority);
        }

        public ITargetDiscoverySyntax<TEvent, THandler> With<THandler>(EventMatching eventMatching, Priority priority) where THandler : class
        {
            return new TargetSyntax<TEvent>(_specification).With<THandler>(eventMatching, priority);
        }

        public ITargetDiscoveryByTypeSyntax<TEvent> With(Type handlerType, EventMatching eventMatching, Priority priority)
        {
            return new TargetSyntax<TEvent>(_specification).With(handlerType, eventMatching, priority);
        }

        public Action WithAsync(IHandlerOf<TEvent> handler)
        {
            return new TargetSyntax<TEvent>(_specification).WithAsync(handler);
        }

        public Action WithAsync(Action<TEvent> action)
        {
            return new TargetSyntax<TEvent>(_specification).WithAsync(action);
        }

        public ITargetDiscoverySyntax<TEvent, THandler> WithAsync<THandler>(EventMatching eventMatching) where THandler : class
        {
            return new TargetSyntax<TEvent>(_specification).WithAsync<THandler>(eventMatching);
        }

        public ITargetDiscoveryByTypeSyntax<TEvent> WithAsync(Type handlerType, EventMatching eventMatching)
        {
            return new TargetSyntax<TEvent>(_specification).WithAsync(handlerType, eventMatching);
        }

        public ITargetSyntax<TEvent> When(Predicate<TEvent> predicate)
        {
            _specification.EventPredicate = predicate;

            return new TargetSyntax<TEvent>(_specification);
        }
    }
}