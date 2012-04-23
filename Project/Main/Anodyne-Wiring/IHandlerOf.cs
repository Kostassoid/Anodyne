namespace Kostassoid.Anodyne.Wiring
{
    public interface IHandlerOf<in TEvent> where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}