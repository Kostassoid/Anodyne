using System;
using Kostassoid.Anodyne.Wiring.Internal;
using Kostassoid.Anodyne.Wiring.Subscription;

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    internal class PredicateSourceSyntax<TEvent> : IPredicateSourceSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public PredicateSourceSyntax(IEventAggregator eventAggregator)
        {
            _specification = new SubscriptionSpecification<TEvent>(eventAggregator);
        }

        public Action With(IHandlerOf<TEvent> handler, int priority = 0)
        {
            return new TargetSyntax<TEvent>(_specification).With(handler, priority);
        }

        public Action With(Action<TEvent> action, int priority = 0)
        {
            return new TargetSyntax<TEvent>(_specification).With(action, priority);
        }

        public ITargetSyntax<TEvent> When(Predicate<TEvent> predicate)
        {
            _specification.EventPredicate = predicate;

            return new TargetSyntax<TEvent>(_specification);
        }
    }
}