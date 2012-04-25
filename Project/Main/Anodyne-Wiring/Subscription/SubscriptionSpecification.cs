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

namespace Kostassoid.Anodyne.Wiring.Subscription
{
    using System;
    using Common.CodeContracts;
    using Internal;

    internal class SubscriptionSpecification<TEvent> where TEvent : class, IEvent
    {
        public IEventAggregator EventAggregator { get; protected set; }
        public Type BaseEventType { get; set; }
        public AssemblySpecification Assembly { get; set; }
        public Predicate<Type> TypePredicate { get; set; }
        public Predicate<TEvent> EventPredicate { get; set; }
        public Action<TEvent> HandlerAction { get; set; }
        public int Priority { get; set; }

        public bool IsValid
        {
            get { return HandlerAction != null; }
        }

        public bool IsPolimorphic
        {
            get { return Assembly != null; }
        }

        public SubscriptionSpecification(IEventAggregator eventAggregator)
        {
            Requires.NotNull(eventAggregator, "eventAggregator");

            EventAggregator = eventAggregator;

            Priority = 0;
            EventPredicate = _ => true;
            TypePredicate = _ => true;

            BaseEventType = typeof (TEvent);
        }
    }
}