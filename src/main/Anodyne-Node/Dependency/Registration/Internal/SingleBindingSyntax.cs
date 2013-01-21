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

namespace Kostassoid.Anodyne.Node.Dependency.Registration.Internal
{
    using System;

    internal class SingleBindingSyntax<TService> : ISingleBindingSyntax<TService> where TService : class
    {
        private readonly SingleBinding _binding;
        IBinding IBindingSyntax.Binding { get { return _binding; } }

        public SingleBindingSyntax()
        {
            _binding = new SingleBinding(typeof(TService));
        }

        public ISingleBindingSyntax<TService> Use<TImpl>() where TImpl : TService
        {
            _binding.SetResolver(new StaticResolver(typeof(TImpl)));
            return this;
        }

        public ISingleBindingSyntax<TService> Use(Func<TService> bindingFunc)
        {
            _binding.SetResolver(new DynamicResolver(bindingFunc));
            return this;
        }

        public ISingleBindingSyntax<TService> UseSelf()
        {
            _binding.SetResolver(new StaticResolver(_binding.Service));
            return this;
        }

        public ISingleBindingSyntax<TService> UseInstance<TImpl>(TImpl instance) where TImpl : class, TService
        {
            _binding.SetResolver(new InstanceResolver(instance));
            return this;
        }

        public ISingleBindingSyntax<TService> With(Lifestyle lifestyle)
        {
            _binding.SetLifestyle(lifestyle);
            return this;
        }

        public ISingleBindingSyntax<TService> Named(string name)
        {
            _binding.SetName(name);
            return this;
        }
    }

    internal class SingleBindingSyntax : ISingleBindingSyntax, IBindingSyntax
    {
        private readonly SingleBinding _binding;
        IBinding IBindingSyntax.Binding { get { return _binding; } }

        public SingleBindingSyntax(Type service)
        {
            _binding = new SingleBinding(service);
        }

        public ISingleBindingSyntax Use<TImpl>() where TImpl : class
        {
            _binding.SetResolver(new StaticResolver(typeof(TImpl)));
            return this;
        }

        public ISingleBindingSyntax Use(Func<object> bindingFunc)
        {
            _binding.SetResolver(new DynamicResolver(bindingFunc));
            return this;
        }

        public ISingleBindingSyntax UseSelf()
        {
            _binding.SetResolver(new StaticResolver(_binding.Service));
            return this;
        }

        public ISingleBindingSyntax UseInstance<TImpl>(TImpl instance) where TImpl : class
        {
            _binding.SetResolver(new InstanceResolver(instance));
            return this;
        }

        public ISingleBindingSyntax With(Lifestyle lifestyle)
        {
            _binding.SetLifestyle(lifestyle);
            return this;
        }

        public ISingleBindingSyntax Named(string name)
        {
            _binding.SetName(name);
            return this;
        }
    }
}