using System;
using Kostassoid.Anodyne.Wiring.Internal;
using Kostassoid.Anodyne.Wiring.Syntax;

namespace Kostassoid.Anodyne.Wiring
{
    public static class EventRouter
    {
        private static readonly IEventAggregator EventAggregator = new MultiThreadAggregator();

        public static void Fire(IEvent @event)
        {
            EventAggregator.Publish(@event);
        }

        public static ISingleSourceTargetScope<TEvent> ReactOn<TEvent>() where TEvent : class, IEvent
        {
            return new SingleSourceTargetScope<TEvent>(EventAggregator);
        }

        public static ISubscriptionScope ReactOn()
        {
            throw new NotImplementedException();
        }
    }

    public interface ISingleSourceTargetScope<out TEvent> where TEvent : IEvent
    {
        void With(IHandlerOf<TEvent> handler, int priority = 0);
        void With(Action<TEvent> action, int priority = 0);
        ISingleSourceTargetScope<TEvent> When(Predicate<TEvent> predicate);
    }

    public interface IMultiSourceTargetScope<out TEvent> where TEvent : IEvent
    {
        void With(IHandlerOf<TEvent> handler, int priority = 0);
    }

    public interface ISubscriptionScope
    {
        ISubscriptionScopePredicate<TEvent> AllOf<TEvent>() where TEvent : IEvent;
    }

    public interface ISubscriptionScopePredicate<out TEvent> where TEvent : IEvent
    {
    }
}