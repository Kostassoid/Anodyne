using System;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface ITargetSyntax<out TEvent> : ISyntax where TEvent : class, IEvent
    {
        Action With(IHandlerOf<TEvent> handler, int priority = 0);
        Action With(Action<TEvent> action, int priority = 0);
    }
}