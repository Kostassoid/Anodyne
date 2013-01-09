using System;

namespace Kostassoid.Anodyne.Node.Dependency
{
    public class StaticResolver : ImplementationResolver
    {
        public Type Target { get; protected set; }
    }
}