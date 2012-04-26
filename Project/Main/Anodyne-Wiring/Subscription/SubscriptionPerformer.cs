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
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using Common;
    using Common.CodeContracts;
    using Internal;

    internal static class SubscriptionPerformer
    {
        public static Action Perform<TEvent>(SubscriptionSpecification<TEvent> specification) where TEvent : class, IEvent
        {
            Requires.NotNull(specification, "specification");
            Requires.True(specification.IsValid, "specification", "Invalid specification");

            var sources = FindTypes(specification.BaseEventType, specification.SourceAssembly, specification.TypePredicate);

            Action unsubscribeAction = () => { };

            foreach (var source in sources)
            {
                if (specification.TargetDiscoveryFunction == null)
                {
                    unsubscribeAction += specification.EventAggregator.Subscribe(new InternalEventHandler<TEvent>(source, specification.HandlerAction,
                                                                                                                  specification.EventPredicate,
                                                                                                                  specification.Priority));
                }
                else
                {
                    var posibleTarget = FindHandler(specification.TargetType, source, specification.TargetDiscoveryFunction);

                    if (posibleTarget.IsSome)
                    {
                        unsubscribeAction += specification.EventAggregator.Subscribe(new InternalEventHandler<TEvent>(source, posibleTarget.Value,
                                                                                                                      specification.EventPredicate,
                                                                                                                      specification.Priority));
                    }
                }
            }

            return unsubscribeAction;
        }

        private static Option<Action<TEvent>> FindHandler<TEvent>(Type targetType, Type eventType, SubscriptionSpecification<TEvent>.TargetDiscoveryFunc targetDiscoveryFunction) where TEvent : class, IEvent
        {
            var method = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).FirstOrDefault(mi => IsTargetCompatibleWithSource(mi, eventType));
            if (method == null) return new None<Action<TEvent>>();

            var methodParam = Expression.Parameter(method.GetParameters()[0].ParameterType);
            var targetParam = Expression.Parameter(targetType);
            var methodCall = Expression.Call(targetParam, method, methodParam);
            var handlerDelegate = Expression.Lambda(methodCall, targetParam, methodParam).Compile();

            Action<TEvent> handler = ev =>
                                         {
                                             var targetObject = targetDiscoveryFunction(ev);
                                             if (targetObject.IsNone) return;

                                             //TODO: use typed lambra instead of using DynamicInvoke()!
                                             handlerDelegate.DynamicInvoke(targetObject.Value, ev);
                                         };

            return handler;
        }

        private static bool IsTargetCompatibleWithSource(MethodInfo methodInfo, Type eventType)
        {
            if (methodInfo.ReturnType != typeof(void)) return false;

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) return false;

            return parameters[0].ParameterType.IsAssignableFrom(eventType);
        }

        private static IEnumerable<Type> FindTypes(Type baseEventType, AssemblySpecification assembly, Predicate<Type> typePredicate)
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