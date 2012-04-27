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
    using System.Linq.Expressions;
    using System.Reflection;
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
                                                                                                                  specification.Priority,
                                                                                                                  specification.Async));
                }
                else
                {
                    foreach (var target in FindHandlers(specification.TargetType, source, specification.TargetDiscoveryFunction, specification.EventMatching))
                    { 
                        unsubscribeAction += specification.EventAggregator.Subscribe(new InternalEventHandler<TEvent>(source, target,
                                                                                                                      specification.EventPredicate,
                                                                                                                      specification.Priority,
                                                                                                                      specification.Async));
                    }
                }
            }

            return unsubscribeAction;
        }

        private static IEnumerable<Action<TEvent>> FindHandlers<TEvent>(Type targetType, Type eventType, SubscriptionSpecification<TEvent>.TargetDiscoveryFunc targetDiscoveryFunction, EventMatching eventMatching) where TEvent : class, IEvent
        {
            var methods = targetType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(mi => IsTargetCompatibleWithSource(mi, eventType, eventMatching)).ToList();
            if (methods.Count == 0) yield break;

            foreach (var method in methods)
            {
                var handlerDelegate = CreateHandlerDelegate(method, eventType);

                Action<TEvent> handler = ev =>
                {
                    var targetObject = targetDiscoveryFunction(ev);
                    if (targetObject.IsNone) return;

                    handlerDelegate(targetObject.Value, ev);
                };

                yield return handler;
            }
        }

        delegate void HandlerMethodDelegate(object instance, IEvent @event);

        private static HandlerMethodDelegate CreateHandlerDelegate(MethodInfo methodInfo, Type eventType)
        {
            var instance = Expression.Parameter(typeof(object), "instance");
            var ev = Expression.Parameter(typeof(IEvent), "event");

            var lambda = Expression.Lambda<HandlerMethodDelegate>(
                Expression.Call(
                    Expression.Convert(instance, methodInfo.DeclaringType),
                    methodInfo,
                    Expression.Convert(ev, eventType)
                    ),
                instance,
                ev
                );

            return lambda.Compile();
        }

        private static bool IsTargetCompatibleWithSource(MethodInfo methodInfo, Type eventType, EventMatching eventMatching)
        {
            if (methodInfo.ReturnType != typeof(void)) return false;

            var parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) return false;

            switch (eventMatching)
            {
                case EventMatching.Strict:
                    return parameters[0].ParameterType == eventType;
                case EventMatching.All:
                    return parameters[0].ParameterType.IsAssignableFrom(eventType);
                default:
                    throw new NotSupportedException(string.Format("EventMatching.{0} is not supported", eventMatching));
            }
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