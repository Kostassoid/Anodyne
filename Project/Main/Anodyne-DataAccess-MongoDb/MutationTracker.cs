using System;
using Kostassoid.Anodyne.Domain.Events;
using Kostassoid.Anodyne.Wiring;

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    public class MutationTracker : IHandlerOf<IMutationEvent>
    {
        public MutationTracker()
        {
            SetupSubscriptions();
        }

        public void SetupSubscriptions()
        {
            EventRouter.ReactOn().AllBasedOn<IMutationEvent>().From(a => a.Contains("Domain")).With(this);
        }

        public void Handle(IMutationEvent @event)
        {
            var aggregateEvent = @event as IAggregateEvent;
            if (aggregateEvent == null) return;

            var uow = UnitOfWork.Current;
            if (uow.IsNone)
                throw new InvalidOperationException(String.Format("Should be inside UnitOfWork context to handle {0} for {1} ", aggregateEvent.GetType().Name, aggregateEvent.AggregateIdObject));
            
            uow.Value.MarkAsUpdated(aggregateEvent.AggregateObject);
        }

    }
}