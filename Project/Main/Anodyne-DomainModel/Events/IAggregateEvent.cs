using Kostassoid.Anodyne.Domain.Base;

namespace Kostassoid.Anodyne.Domain.Events
{
    public interface IAggregateEvent : IMutationEvent
    {
        IAggregateRoot AggregateObject { get; }
        object AggregateIdObject { get; }
    }
}