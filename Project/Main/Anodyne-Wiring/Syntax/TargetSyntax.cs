using System;
using Kostassoid.Anodyne.Wiring.Subscription;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    internal class TargetSyntax<TEvent> : ITargetSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public TargetSyntax(SubscriptionSpecification<TEvent> specification)
        {
            _specification = specification;
        }

        public Action With(IHandlerOf<TEvent> handler, int priority = 0)
        {
            _specification.HandlerAction = handler.Handle;
            _specification.Priority = priority;

            return SubscriptionPerformer.Perform(_specification);
        }

        public Action With(Action<TEvent> action, int priority = 0)
        {
            _specification.HandlerAction = action;
            _specification.Priority = priority;

            return SubscriptionPerformer.Perform(_specification);
        }
    }
}