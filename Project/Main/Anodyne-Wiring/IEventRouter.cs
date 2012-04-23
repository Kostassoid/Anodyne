using System;
using Kostassoid.Anodyne.Wiring.Internal;

namespace Kostassoid.Anodyne.Wiring
{
    public interface IEventRouter
    {
        void Fire(IEvent @event);
        Action Subscribe(IInternalEventHandler eventHandler);
    }
}