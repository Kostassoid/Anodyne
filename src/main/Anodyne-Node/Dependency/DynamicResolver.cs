using System;

namespace Kostassoid.Anodyne.Node.Dependency
{
    public class DynamicResolver : ImplementationResolver
    {
        public Func<Type, object> FactoryFunc { get; protected set; }
    }
}