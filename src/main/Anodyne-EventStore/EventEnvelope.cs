namespace Kostassoid.Anodyne.EventStore
{
    using System;
    using Domain.Events;

    [Serializable]
    public class EventEnvelope
    {
        public Guid Id { get; private set; }
        public string TargetType { get; private set; }
        public object TargetId { get; private set; }
        public long TargetVersion { get; private set; }
        public string EventType { get; private set; }
        public IAggregateEvent Event { get; private set; }

        public EventEnvelope(IAggregateEvent @event)
        {
            Event = @event;

            Id = @event.Id;
            TargetType = @event.Target.GetType().Name;
            TargetId = @event.Target.IdObject;
            TargetVersion = @event.TargetVersion;
            EventType = @event.GetType().Name;
        }
    }
}