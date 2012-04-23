using System;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface ISourceTypeFilterSyntax<out TEvent> : ITargetSyntax<TEvent> where TEvent : class, IEvent 
    {
        ITargetSyntax<TEvent> Where(Predicate<Type> filter);
    }
}