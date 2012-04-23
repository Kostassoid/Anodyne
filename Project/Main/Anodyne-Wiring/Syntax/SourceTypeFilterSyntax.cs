using System;
using Kostassoid.Anodyne.Wiring.Subscription;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    class SourceTypeFilterSyntax<TEvent> : ISourceTypeFilterSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public SourceTypeFilterSyntax(SubscriptionSpecification<TEvent> specification)
        {
            _specification = specification;
        }

        public Action With(IHandlerOf<TEvent> handler, int priority = 0)
        {
            return new TargetSyntax<TEvent>(_specification).With(handler, priority);
        }

        public Action With(Action<TEvent> action, int priority = 0)
        {
            return new TargetSyntax<TEvent>(_specification).With(action, priority);
        }

        public ITargetSyntax<TEvent> Where(Predicate<Type> filter)
        {
            _specification.TypePredicate = filter;

            return new TargetSyntax<TEvent>(_specification);
        }
    }
}