using Kostassoid.Anodyne.Wiring.Internal;
using Kostassoid.Anodyne.Wiring.Syntax;
using Kostassoid.Anodyne.Wiring.Syntax.Concrete;

namespace Kostassoid.Anodyne.Wiring
{
    public static class EventRouter
    {
        private static readonly IEventAggregator EventAggregator = new MultiThreadAggregator();

        public static void Fire(IEvent @event)
        {
            EventAggregator.Publish(@event);
        }

        public static IPredicateSourceSyntax<TEvent> ReactOn<TEvent>() where TEvent : class, IEvent
        {
            return new PredicateSourceSyntax<TEvent>(EventAggregator);
        }

        public static IConventionSourceSyntax ReactOn()
        {
            return new ConventionSourceSyntax(EventAggregator);
        }
    }
}