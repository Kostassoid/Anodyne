namespace Kostassoid.Anodyne.Wiring.Syntax
{
    public interface IConventionSourceSyntax
    {
        IAssemblySourceSyntax<TEvent> AllBasedOn<TEvent>() where TEvent : class, IEvent;
    }
}