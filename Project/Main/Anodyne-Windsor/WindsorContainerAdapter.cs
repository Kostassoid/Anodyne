// Copyright 2011-2012 Anodyne.
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

namespace Kostassoid.Anodyne.Windsor
{
    using System.Dependency;
    using System.Dependency.Registration;
    using Castle.Facilities.Startable;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Registration;
    using global::System;
    using global::System.Collections.Generic;

    public class WindsorContainerAdapter : IContainer
    {
        private readonly IWindsorContainer _container;

        public WindsorContainerAdapter(IWindsorContainer container)
        {
            _container = container;

            _container.Kernel.ReleasePolicy = new TransientReleasePolicy(_container.Kernel);
            _container.Kernel.Resolver.AddSubResolver(new ListResolver(_container.Kernel));

            _container.AddFacility<StartableFacility>();
        }

        public IList<T> GetAll<T>()
        {
            return _container.ResolveAll<T>();
        }

        public T Get<T>()
        {
            return _container.Resolve<T>();
        }

        public IBindingSyntax<TService> For<TService>() where TService : class
        {
            return new BindingSyntax<TService>(_container);
        }

        public IServiceAssemblySyntax<TService> ForAll<TService>() where TService : class
        {
            throw new NotImplementedException();
        }
    }
}