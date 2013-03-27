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
    using System.Collections.Generic;

    internal class MultipleBindingSyntax : IMultipleBindingSyntax
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

	    public IMultipleBindingSyntax As(params Type[] services)
	    {
			_binding.ForwardTo(services);
			return this;
		}

	    public IMultipleBindingSyntax With(Lifestyle lifestyle)
        {
            _binding.Lifestyle = lifestyle;
            return this;
        }
    }
}