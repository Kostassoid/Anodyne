using System;
using System.Collections.Generic;
using System.Linq;
using Kostassoid.Anodyne.Common.CodeContracts;
using Kostassoid.Anodyne.Wiring.Internal;

namespace Kostassoid.Anodyne.Wiring.Subscription
{
    internal static class SubscriptionPerformer
    {
        public static Action Perform<TEvent>(SubscriptionSpecification<TEvent> specification) where TEvent : class, IEvent
        {
            Requires.NotNull(specification, "specification");
            Requires.True(specification.IsValid, "specification", "Invalid specification");

            var sources = FindSources(specification.BaseEventType, specification.Assembly, specification.TypePredicate);

            Action unsubscribeAction = () => {};

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