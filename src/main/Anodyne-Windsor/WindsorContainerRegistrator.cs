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

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Kostassoid.Anodyne.Node.Dependency;
using Kostassoid.Anodyne.Node.Dependency.Registration;

namespace Kostassoid.Anodyne.Windsor
{
    public static class WindsorContainerRegistrator
    {
        public static void Register(IWindsorContainer container, SingleBinding binding)
        {
            ComponentRegistration<object> registration = Component.For(binding.Service);

            registration = ApplyResolver(registration, (dynamic) binding.Resolver);

            registration = ApplyLifestyle(registration, binding.Lifestyle);

            registration = ApplyName(registration, binding.Named);

            container.Register(registration);
        }

        public static void Register(IWindsorContainer container, MultipleBinding binding)
        {
            ComponentRegistration<object> registration = Component.For(binding.Services);

            if (binding.BindTo.Count > 0)
                registration = registration.Forward(binding.BindTo);

            registration = ApplyLifestyle(registration, binding.Lifestyle);

            container.Register(registration);
        }

        private static ComponentRegistration<object> ApplyResolver(ComponentRegistration<object> registration, StaticResolver resolver)
        {
            return registration.ImplementedBy(resolver.Target);
        }

        private static ComponentRegistration<object> ApplyResolver(ComponentRegistration<object> registration, InstanceResolver resolver)
        {
            return registration.Instance(resolver.Instance);
        }

        private static ComponentRegistration<object> ApplyResolver(ComponentRegistration<object> registration, DynamicResolver resolver)
        {
            return registration.UsingFactoryMethod(resolver.FactoryFunc);
        }

        private static ComponentRegistration<object> ApplyLifestyle(ComponentRegistration<object> registration, Lifestyle lifestyle)
        {
            switch (lifestyle)
            {
                case Lifestyle.Singleton: return registration.LifestyleSingleton();
                case Lifestyle.Transient: return registration.LifestyleTransient();
                case Lifestyle.PerWebRequest: return registration.LifestylePerWebRequest();
                case Lifestyle.Unmanaged: return registration.LifestyleCustom<UnmanagedLifestyleManager>();
                case Lifestyle.ProviderDefault: return registration;
                default:
                    throw new ArgumentException(string.Format("Unknown lifestyle : {0}", lifestyle), "lifestyle");
            }
        }

        private static ComponentRegistration<object> ApplyName(ComponentRegistration<object> registration, string name)
        {
            if (string.IsNullOrEmpty(name))
                return registration;

            return registration.Named(name);
        }

    }
}