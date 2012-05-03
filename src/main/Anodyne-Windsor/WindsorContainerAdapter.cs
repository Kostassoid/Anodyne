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
        public IWindsorContainer NativeContainer { get; protected set; }

        public WindsorContainerAdapter(IWindsorContainer container)
        {
            NativeContainer = container;

            NativeContainer.Kernel.ReleasePolicy = new TransientReleasePolicy(NativeContainer.Kernel);
            NativeContainer.Kernel.Resolver.AddSubResolver(new ListResolver(NativeContainer.Kernel));

            NativeContainer.AddFacility<StartableFacility>();
        }

        public IList<T> GetAll<T>()
        {
            return NativeContainer.ResolveAll<T>();
        }

        public T Get<T>()
        {
            return NativeContainer.Resolve<T>();
        }

        public IBindingSyntax<TService> For<TService>() where TService : class
        {
            return new BindingSyntax<TService>(NativeContainer);
        }

        public IServiceAssemblySyntax<TService> ForAll<TService>() where TService : class
        {
            throw new NotImplementedException();
        }
    }
}