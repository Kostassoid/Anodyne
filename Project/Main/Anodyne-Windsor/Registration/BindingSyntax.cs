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

namespace Kostassoid.Anodyne.Windsor.Registration
{
    using System.Dependency;
    using System.Dependency.Registration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using global::System;

    public class BindingSyntax<TService> : IBindingSyntax<TService> where TService : class
    {
        private readonly IWindsorContainer _container;

        public BindingSyntax(IWindsorContainer container)
        {
            _container = container;
        }

        public void Use<TImpl>(Lifestyle lifestyle) where TImpl : TService
        {
            _container.Register(
                Component
                    .For<TService>()
                    .ImplementedBy<TImpl>()
                    .LifeStyle.Is(GetLifestyle(lifestyle)));
        }

        public void Use(Func<TService> bindingFunc, Lifestyle lifestyle)
        {
            _container.Register(
                Component
                    .For<TService>()
                    .UsingFactoryMethod(bindingFunc)
                    .LifeStyle.Is(GetLifestyle(lifestyle)));
        }

        private static Castle.Core.LifestyleType GetLifestyle(Lifestyle lifestyle)
        {
            switch (lifestyle)
            {
                case Lifestyle.Singleton: return Castle.Core.LifestyleType.Singleton;
                case Lifestyle.Transient: return Castle.Core.LifestyleType.Transient;
                case Lifestyle.PerWebRequest: return Castle.Core.LifestyleType.PerWebRequest;
                default:
                    throw new ArgumentException(string.Format("Unknown lifestyle : {0}", lifestyle), "lifestyle");
            }
        }
    }
}