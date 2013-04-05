// Copyright 2011-2013 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
//  
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.Abstractions.Dependency.Registration.Internal
{
    using System;

	internal class SingleBindingSyntax<TImpl> : ISingleBindingSyntax<TImpl> where TImpl : class
    {
        private readonly SingleBinding _binding;
        IBinding IBindingSyntax.Binding { get { return _binding; } }

        public SingleBindingSyntax(IImplementationResolver resolver)
        {
			_binding = new SingleBinding
				{
					Service = typeof(TImpl),
					Resolver = resolver
				};
        }

		public ISingleBindingSyntax<TImpl> As<TService>() where TService : class
	    {
			_binding.Service = typeof(TService);
			return this;
		}

		public ISingleBindingSyntax<TImpl> As(Type service)
	    {
			_binding.Service = service;
			return this;
		}

		public ISingleBindingSyntax<TImpl> With(Lifecycle lifecycle)
        {
            _binding.Lifecycle = lifecycle;
            return this;
        }

		public ISingleBindingSyntax<TImpl> Named(string name)
        {
            _binding.Name = name;
            return this;
        }
    }

    internal class SingleBindingSyntax : ISingleBindingSyntax
    {
        private readonly SingleBinding _binding;
        IBinding IBindingSyntax.Binding { get { return _binding; } }

		public SingleBindingSyntax(Type implementation, IImplementationResolver resolver)
        {
			_binding = new SingleBinding
			{
				Service = implementation,
				Resolver = resolver
			};
		}

	    public ISingleBindingSyntax As<TService>() where TService : class
	    {
		    _binding.Service = typeof (TService);
		    return this;
	    }

	    public ISingleBindingSyntax As(Type service)
	    {
			_binding.Service = service;
			return this;
		}

	    public ISingleBindingSyntax With(Lifecycle lifecycle)
        {
            _binding.Lifecycle = lifecycle;
            return this;
        }

        public ISingleBindingSyntax Named(string name)
        {
            _binding.Name = name;
            return this;
        }
    }
}