using System;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IAssemblySourceSyntax<out TEvent> where TEvent : class, IEvent
    {
        ISourceTypeFilterSyntax<TEvent> FromThisAssembly();
        ISourceTypeFilterSyntax<TEvent> From(Predicate<string> assemblyNameFilter);
    }
}