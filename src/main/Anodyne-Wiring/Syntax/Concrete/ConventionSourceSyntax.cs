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

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Common.Extentions;
    using Common.Reflection;
    using Internal;
    using Subscription;

    internal class ConventionSourceSyntax : IConventionSourceSyntax
    {
        private readonly IEventAggregator _eventAggregator;

        public ConventionSourceSyntax(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public ISourceTypeFilterSyntax<TEvent> AllBasedOn<TEvent>() where TEvent : class, IEvent
        {
            var specification = new SubscriptionSpecification<TEvent>(_eventAggregator)
                                    {
                                        BaseEventType = typeof (TEvent),
                                        SourceAssemblies = Assembly.GetCallingAssembly().AsEnumerable().ToList()
                                    };


            return new SourceTypeFilterSyntax<TEvent>(specification);
        }

        public ISourceTypeFilterSyntax<TEvent> AllBasedOn<TEvent>(IEnumerable<Assembly> assemblies) where TEvent : class, IEvent
        {
            var specification = new SubscriptionSpecification<TEvent>(_eventAggregator)
            {
                BaseEventType = typeof(TEvent),
                SourceAssemblies = assemblies.ToList()
            };

            return new SourceTypeFilterSyntax<TEvent>(specification);
        }
    }
}