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
    using System.Collections.Generic;
    using System.Linq;
    using Common.CodeContracts;
    using Internal;

    internal static class SubscriptionPerformer
    {
        public static Action Perform<TEvent>(SubscriptionSpecification<TEvent> specification) where TEvent : class, IEvent
        {
            Requires.NotNull(specification, "specification");
            Requires.True(specification.IsValid, "specification", "Invalid specification");

            var sources = FindSources(specification.BaseEventType, specification.Assembly, specification.TypePredicate);

            Action unsubscribeAction = () => { };

            foreach (var source in sources)
            {
                unsubscribeAction += specification.EventAggregator.Subscribe(new InternalEventHandler<TEvent>(source, specification.HandlerAction,
                                                                                                              specification.EventPredicate,
                                                                                                              specification.Priority));
            }

            return unsubscribeAction;
        }

        private static IEnumerable<Type> FindSources(Type baseEventType, AssemblySpecification assembly, Predicate<Type> typePredicate)
        {
            if (assembly == null)
            {
                yield return baseEventType;
                yield break;
            }

            IEnumerable<Type> types;

            if (assembly.This.IsSome)
                types = assembly.This.Value.GetTypes();
            else
                types = AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Where(a => assembly.Filter(a.FullName))
                    .SelectMany(a => a.GetTypes());


            foreach (var type in types.Where(t => baseEventType.IsAssignableFrom(t) && typePredicate(t)))
            {
                yield return type;
            }
        }
    }
}