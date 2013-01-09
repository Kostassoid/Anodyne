using System;
using System.Collections.Generic;

namespace Kostassoid.Anodyne.Node.Dependency
{
    public abstract class Binding
    {
        public IEnumerable<Type> Services { get; protected set; }
        public ImplementationResolver Resolver { get; protected set; }
        public Lifestyle Lifestyle { get; protected set; }
        public Func<Type, string> Naming { get; protected set; }
    }
}