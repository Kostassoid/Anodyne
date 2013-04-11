namespace Kostassoid.Anodyne.EventStore
{
    using System;
    using Domain.DataAccess;
    using Domain.DataAccess.Events;
    using Wiring;

    public class EventStoreObserver
    {
        private readonly IEventStoreAdapter _adapter;
        private Action _stopAction = () => { };

        public EventStoreObserver(IEventStoreAdapter adapter)
        {
            _adapter = adapter;
        }

        public void Start()
        {
            _stopAction += EventBus.SubscribeTo<UnitOfWorkCompleted>().With(ev => Handle(ev.ChangeSet));
            _stopAction += EventBus.SubscribeTo<UnitOfWorkFailed>().With(ev => Handle(ev.ChangeSet));
        }

        private void Handle(DataChangeSet changeSet)
        {
            _adapter.Store(changeSet.AppliedEvents);
        }

        public void Stop()
        {
            _stopAction();
            _stopAction = () => { };
        }
    }
}