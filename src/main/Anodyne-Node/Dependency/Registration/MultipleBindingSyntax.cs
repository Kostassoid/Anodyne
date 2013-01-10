using System;
using System.Collections.Generic;

namespace Kostassoid.Anodyne.Node.Dependency.Registration
{
    public class MultipleBindingSyntax : IMultipleBindingSyntax
    {
        private readonly MultipleBinding _binding;
        IBinding IBindingSyntax.Binding { get { return _binding; } }

        public MultipleBindingSyntax(IEnumerable<Type> services)
        {
            _binding = new MultipleBinding(services);
        }

        public IMultipleBindingSyntax As<TService>() where TService : class
        {
            _binding.ForwardTo<TService>();
            return this;
        }

        public IMultipleBindingSyntax With(Lifestyle lifestyle)
        {
            _binding.SetLifestyle(lifestyle);
            return this;
        }
    }
}