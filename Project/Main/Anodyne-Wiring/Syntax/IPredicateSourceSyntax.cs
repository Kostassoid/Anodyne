using System;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IPredicateSourceSyntax<out TEvent> : ITargetSyntax<TEvent> where TEvent : class, IEvent
    {
        ITargetSyntax<TEvent> When(Predicate<TEvent> predicate);
    }
}