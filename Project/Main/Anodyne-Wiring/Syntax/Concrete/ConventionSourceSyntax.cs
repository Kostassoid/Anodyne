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
    using Internal;
    using Subscription;

    internal class ConventionSourceSyntax : IConventionSourceSyntax
    {
        private readonly IEventAggregator _eventAggregator;

        public ConventionSourceSyntax(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IAssemblySourceSyntax<TEvent> AllBasedOn<TEvent>() where TEvent : class, IEvent
        {
            var specification = new SubscriptionSpecification<TEvent>(_eventAggregator)
                                    {
                                        BaseEventType = typeof (TEvent)
                                    };


            return new AssemblySourceSyntax<TEvent>(specification);
        }
    }
}