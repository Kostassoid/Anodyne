using System;
using Kostassoid.Anodyne.Common.CodeContracts;
using Kostassoid.Anodyne.Wiring.Internal;

namespace Kostassoid.Anodyne.Wiring.Subscription
{
    internal class SubscriptionSpecification<TEvent> where TEvent : class, IEvent
    {
        public IEventAggregator EventAggregator { get; protected set; }
        public Type BaseEventType { get; set; }
        public AssemblySpecification Assembly { get; set; }
        public Predicate<Type> TypePredicate { get; set; }
        public Predicate<TEvent> EventPredicate { get; set; }
        public Action<TEvent> HandlerAction { get; set; }
        public int Priority { get; set; }

        public bool IsValid { get { return HandlerAction != null; }}

        public bool IsPolimorphic { get { return Assembly != null; } }

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