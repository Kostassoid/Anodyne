using System;
using System.Reflection;
using Kostassoid.Anodyne.Common;

namespace Kostassoid.Anodyne.Wiring.Subscription
{
    internal class AssemblySpecification
    {
        public Option<Assembly> This { get; protected set; }
        public Predicate<string> Filter { get; protected set; }

        public AssemblySpecification(Assembly thisAssembly)
        {
            This = thisAssembly;
        }

        public AssemblySpecification(Predicate<string> filter)
        {
            This = new None<Assembly>();
            Filter = filter;
        }
    }
}