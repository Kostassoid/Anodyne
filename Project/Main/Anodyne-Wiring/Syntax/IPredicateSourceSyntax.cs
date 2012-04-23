using System;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IPredicateSourceSyntax<out TEvent> : ITargetSyntax<TEvent>, ISyntax where TEvent : class, IEvent
    {
        ITargetSyntax<TEvent> When(Predicate<TEvent> predicate);
    }
}