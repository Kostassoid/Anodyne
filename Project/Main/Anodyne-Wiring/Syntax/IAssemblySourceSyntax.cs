using System;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IAssemblySourceSyntax<out TEvent> : ISyntax where TEvent : class, IEvent
    {
        ISourceTypeFilterSyntax<TEvent> FromThisAssembly();
        ISourceTypeFilterSyntax<TEvent> From(Predicate<string> assemblyNameFilter);
    }
}