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

namespace Kostassoid.Anodyne.Wiring
{
    using Internal;
    using Syntax;
    using Syntax.Concrete;

    public static class EventRouter
    {
        private static readonly IEventAggregator EventAggregator = new MultiThreadAggregator();

        public static void Fire(IEvent @event)
        {
            EventAggregator.Publish(@event);
        }

        public static IPredicateSourceSyntax<TEvent> ReactOn<TEvent>() where TEvent : class, IEvent
        {
            return new PredicateSourceSyntax<TEvent>(EventAggregator);
        }

        public static IConventionSourceSyntax ReactOn()
        {
            return new ConventionSourceSyntax(EventAggregator);
        }
    }
}