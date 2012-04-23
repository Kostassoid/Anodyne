using System;
using Kostassoid.Anodyne.Common;
using Kostassoid.Anodyne.Domain.Base;

namespace Kostassoid.Anodyne.Domain.Events
{
    [Serializable]
    public abstract class AggregateEvent<TRoot, TKey> : PersistentDomainEvent, IMutationEvent where TRoot : AggregateRoot<TKey>
    {
        private readonly TRoot _aggregate;
        public TRoot Aggregate { get { return _aggregate; } } // should not be stored!

        public TKey AggregateId { get; protected set; }
        public int AggregateVersion { get; protected set; }

        protected AggregateEvent() {}

        protected AggregateEvent(TRoot aggregate, DateTime happened, EventData data):base(happened, data)
        {
            _aggregate = aggregate;
            AggregateId = aggregate.Id;
            AggregateVersion = aggregate.NewVersion();
        }

        protected AggregateEvent(TRoot aggregate, EventData data)
            : this(aggregate, SystemTime.Now, data)
        {
        }

        protected AggregateEvent(TRoot aggregate)
            : this(aggregate, SystemTime.Now, new EmptyEventData())
        {
        }
    }
}