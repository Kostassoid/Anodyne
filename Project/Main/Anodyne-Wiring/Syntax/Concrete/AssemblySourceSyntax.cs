using System;
using System.Reflection;
using Kostassoid.Anodyne.Wiring.Subscription;

namespace Kostassoid.Anodyne.Wiring.Syntax.Concrete
{
    internal class AssemblySourceSyntax<TEvent> : IAssemblySourceSyntax<TEvent> where TEvent : class, IEvent
    {
        private readonly SubscriptionSpecification<TEvent> _specification;

        public AssemblySourceSyntax(SubscriptionSpecification<TEvent> specification)
        {
            _specification = specification;
        }

        public ISourceTypeFilterSyntax<TEvent> FromThisAssembly()
        {
            _specification.Assembly = new AssemblySpecification(Assembly.GetCallingAssembly());

            return new SourceTypeFilterSyntax<TEvent>(_specification);
        }

        public ISourceTypeFilterSyntax<TEvent> From(Predicate<string> assemblyNameFilter)
        {
            _specification.Assembly = new AssemblySpecification(assemblyNameFilter);

            return new SourceTypeFilterSyntax<TEvent>(_specification);
        }
    }
}