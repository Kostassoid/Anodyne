using System;
using Kostassoid.Anodyne.Wiring.Internal;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    class SingleSourceTargetScope<TEvent> : ISingleSourceTargetScope<TEvent> where TEvent : class, IEvent
    {
        private readonly IEventAggregator _eventAggregator;

        public SingleSourceTargetScope(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public void With(IHandlerOf<TEvent> handler, int priority = 0)
        {
            _eventAggregator.Subscribe(new InternalEventHandler<TEvent>(handler.Handle, _ => true, priority));
        }

        public void With(Action<TEvent> action, int priority = 0)
        {
            _eventAggregator.Subscribe(new InternalEventHandler<TEvent>(action, _ => true, priority));
        }

        public ISingleSourceTargetScope<TEvent> When(Predicate<TEvent> predicate)
        {
            throw new NotImplementedException();
        }
    }
}