using System;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface ISourceTypeFilterSyntax<out TEvent> : ITargetSyntax<TEvent>, ISyntax where TEvent : class, IEvent 
    {
        ITargetSyntax<TEvent> Where(Predicate<Type> filter);
    }
}