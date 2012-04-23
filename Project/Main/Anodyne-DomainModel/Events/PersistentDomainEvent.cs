using System;
using Kostassoid.Anodyne.Common;
using Kostassoid.Anodyne.Domain.Base;

namespace Kostassoid.Anodyne.Domain.Events
{
    [Serializable]
    public abstract class PersistentDomainEvent : AggregateRoot<Guid>, IDomainEvent
    {
        public DateTime Happened { get; protected set; }
        public EventData Data { get; protected set; }
        public override int Version { get { return 0; } } // we don't want version-tracking events

        public bool IsReplaying { get; set; }

        protected PersistentDomainEvent(DateTime happened, EventData data)
        {
            Happened = happened;
            Data = data;
        }

        protected PersistentDomainEvent(EventData data)
            : this(SystemTime.Now, data)
        {
        }

        protected PersistentDomainEvent()
            : this(SystemTime.Now, new EmptyEventData())
        {
        }

    }
}