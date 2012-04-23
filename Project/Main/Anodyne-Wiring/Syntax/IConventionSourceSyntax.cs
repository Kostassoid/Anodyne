using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IConventionSourceSyntax : ISyntax
    {
        IAssemblySourceSyntax<TEvent> AllBasedOn<TEvent>() where TEvent : class, IEvent;
    }
}