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

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Kostassoid.Anodyne.Windsor
{
    using Abstractions.Dependency;

    public static class WindsorContainerRegistrator
    {
        public static void Register(IWindsorContainer container, SingleBinding binding)
        {
            ComponentRegistration<object> registration = Component.For(binding.Service);

            registration = ApplyResolver(registration, (dynamic) binding.Resolver);

            registration = ApplyLifecycle(registration, binding.Lifecycle);

            registration = ApplyName(registration, binding.Name);

            container.Register(registration);
        }

        public static void Register(IWindsorContainer container, MultipleBinding binding)
        {
            var registration = AllTypes.From(binding.Services).Pick();

            if (binding.BindTo.Count > 0)
                registration = registration.WithServices(binding.BindTo);

            registration = ApplyLifecycle(registration, binding.Lifecycle);

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

        private static ComponentRegistration<object> ApplyLifecycle(ComponentRegistration<object> registration, Lifecycle lifecycle)
        {
            if (lifecycle.Name == Lifecycle.Singleton.Name)
                return registration.LifeStyle.Singleton;

            if (lifecycle.Name == Lifecycle.Transient.Name)
                return registration.LifeStyle.Transient;

            if (lifecycle.Name == Lifecycle.PerWebRequest.Name)
                return registration.LifeStyle.PerWebRequest;

            if (lifecycle.Name == Lifecycle.Unmanaged.Name)
                return registration.LifeStyle.Custom<UnmanagedLifestyleManager>();

            if (lifecycle.Name == Lifecycle.Default.Name)
                return registration.LifeStyle.Singleton;

            if (lifecycle.Name == Lifecycle.ProviderDefault.Name)
                return registration;

            throw new ArgumentException(string.Format("Unknown Lifecycle : {0}", lifecycle), "lifecycle");
        }

        private static BasedOnDescriptor ApplyLifecycle(BasedOnDescriptor registration, Lifecycle lifecycle)
        {
            if (lifecycle.Name == Lifecycle.Singleton.Name)
                return registration.LifestyleSingleton();

            if (lifecycle.Name == Lifecycle.Transient.Name)
                return registration.LifestyleTransient();

            if (lifecycle.Name == Lifecycle.PerWebRequest.Name)
                return registration.LifestylePerWebRequest();

            if (lifecycle.Name == Lifecycle.Unmanaged.Name)
                return registration.LifestyleCustom<UnmanagedLifestyleManager>();

            if (lifecycle.Name == Lifecycle.Default.Name)
                return registration.LifestyleSingleton();

            if (lifecycle.Name == Lifecycle.ProviderDefault.Name)
                return registration;

            throw new ArgumentException(string.Format("Unknown Lifecycle : {0}", lifecycle), "lifecycle");
        }

        private static ComponentRegistration<object> ApplyName(ComponentRegistration<object> registration, string name)
        {
            if (string.IsNullOrEmpty(name))
                return registration;

            return registration.Named(name);
        }

    }
}