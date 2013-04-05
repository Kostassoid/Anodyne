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

namespace Kostassoid.Anodyne.Autofac
{
	using System;
	using System.Linq;
	using Abstractions.Dependency;
	using global::Autofac;
	using global::Autofac.Builder;
	using global::Autofac.Features.Scanning;

	public static class AutofacContainerRegistrator
    {
        public static void Register(global::Autofac.IContainer container, SingleBinding binding)
        {
	        var builder = new ContainerBuilder();

			IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registration = ApplyResolver(builder, (dynamic)binding.Resolver);

			registration = registration.As(binding.Service).PreserveExistingDefaults();

            registration = ApplyLifecycleSingle(registration, binding.Lifecycle);

            ApplyName(registration, binding.Name, binding.Service);

			builder.Update(container);
        }

		public static void Register(global::Autofac.IContainer container, MultipleBinding binding)
        {
			var builder = new ContainerBuilder();

			var registration = builder.RegisterTypes(binding.Services.ToArray());

            if (binding.BindTo.Count > 0)
                registration = registration.As(binding.BindTo.ToArray());

            ApplyLifecycleMultiple(registration, binding.Lifecycle);

			builder.Update(container);
		}

		private static IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> ApplyResolver(ContainerBuilder builder, StaticResolver resolver)
        {
	        return builder.RegisterType(resolver.Target);
        }

		private static IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> ApplyResolver(ContainerBuilder builder, InstanceResolver resolver)
        {
			return builder.RegisterInstance(resolver.Instance).ExternallyOwned();
        }

		private static IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> ApplyResolver(ContainerBuilder builder, DynamicResolver resolver)
        {
			return builder.Register(ctx => resolver.FactoryFunc());
        }

		private static IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> ApplyLifecycleSingle(IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registration, Lifecycle lifecycle)
        {
            if (lifecycle.Name == Lifecycle.Singleton.Name)
                return registration.SingleInstance();

            if (lifecycle.Name == Lifecycle.Transient.Name)
                return registration.InstancePerDependency();

            if (lifecycle.Name == Lifecycle.PerWebRequest.Name)
				return registration.InstancePerMatchingLifetimeScope("httpRequest");

			if (lifecycle.Name == Lifecycle.Unmanaged.Name)
				return registration.ExternallyOwned();

            if (lifecycle.Name == Lifecycle.Default.Name)
                return registration.SingleInstance();

            if (lifecycle.Name == Lifecycle.ProviderDefault.Name)
                return registration;

            throw new ArgumentException(string.Format("Unknown Lifecycle : {0}", lifecycle), "lifecycle");
        }

		private static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> ApplyLifecycleMultiple(IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> registration, Lifecycle lifecycle)
        {
            if (lifecycle.Name == Lifecycle.Singleton.Name)
                return registration.SingleInstance();

            if (lifecycle.Name == Lifecycle.Transient.Name)
                return registration.InstancePerDependency();

            if (lifecycle.Name == Lifecycle.PerWebRequest.Name)
				return registration.InstancePerMatchingLifetimeScope("httpRequest");

            if (lifecycle.Name == Lifecycle.Unmanaged.Name)
                return registration.ExternallyOwned();

            if (lifecycle.Name == Lifecycle.Default.Name)
                return registration.SingleInstance();

            if (lifecycle.Name == Lifecycle.ProviderDefault.Name)
                return registration;

            throw new ArgumentException(string.Format("Unknown Lifecycle : {0}", lifecycle), "lifecycle");
        }

		private static IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> ApplyName(IRegistrationBuilder<object, IConcreteActivatorData, SingleRegistrationStyle> registration, string name, Type service)
        {
            if (string.IsNullOrEmpty(name))
                return registration;

            return registration.Named(name, service);
        }

    }
}