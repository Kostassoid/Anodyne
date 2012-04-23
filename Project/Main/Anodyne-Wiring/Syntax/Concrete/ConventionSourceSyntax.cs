using Kostassoid.Anodyne.Wiring.Internal;
using Kostassoid.Anodyne.Wiring.Subscription;

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    internal class ConventionSourceSyntax : IConventionSourceSyntax
    {
        private readonly IEventAggregator _eventAggregator;

        public ConventionSourceSyntax(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }

        public IAssemblySourceSyntax<TEvent> AllBasedOn<TEvent>() where TEvent : class, IEvent
        {
            var specification = new SubscriptionSpecification<TEvent>(_eventAggregator)
                                    {
                                        BaseEventType = typeof (TEvent)
                                    };


            return new AssemblySourceSyntax<TEvent>(specification);
        }
    }
}