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
    using Subscription;

    internal class TargetSyntax<TEvent> : ITargetSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public TargetSyntax(SubscriptionSpecification<TEvent> specification)
        {
            _specification = specification;
        }

        public Action With(IHandlerOf<TEvent> handler, int priority = 0)
        {
            _specification.HandlerAction = handler.Handle;
            _specification.Priority = priority;

            return SubscriptionPerformer.Perform(_specification);
        }

        public Action With(Action<TEvent> action, int priority = 0)
        {
            _specification.HandlerAction = action;
            _specification.Priority = priority;

            return SubscriptionPerformer.Perform(_specification);
        }

        public ITargetDiscoverySyntax<TEvent, THandler> With<THandler>(EventMatching strict) where THandler : class
        {
            return new TargetDiscoverySyntax<TEvent, THandler>(_specification);
        }
    }
}