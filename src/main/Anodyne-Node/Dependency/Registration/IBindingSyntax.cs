using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Node.Dependency.Registration
{
    public interface IBindingSyntax : ISyntax
    {
        IBinding Binding { get; } 
    }
}