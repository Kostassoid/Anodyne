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
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Node.Dependency;
    using Node.Dependency.Registration;
    using System;

    public class BindingSyntax<TService> : IBindingSyntax<TService> where TService : class
    {
        private readonly IWindsorContainer _container;

        public BindingSyntax(IWindsorContainer container)
        {
            _container = container;
        }

        //TODO: DRY
        public void Use<TImpl>(Lifestyle lifestyle, string name) where TImpl : TService
        {
            var componentRegistration = Component
                .For<TService>()
                .ImplementedBy<TImpl>()
                .LifeStyle.Is(GetLifestyle(lifestyle));

            if (!string.IsNullOrEmpty(name))
                componentRegistration = componentRegistration.Named(name);

            _container.Register(componentRegistration);
        }

        public void Use(Func<TService> bindingFunc, Lifestyle lifestyle, string name)
        {
            var componentRegistration = Component
                .For<TService>()
                .UsingFactoryMethod(bindingFunc)
                .LifeStyle.Is(GetLifestyle(lifestyle));

            if (!string.IsNullOrEmpty(name))
                componentRegistration = componentRegistration.Named(name);

            _container.Register(componentRegistration);
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