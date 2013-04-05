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

namespace Kostassoid.Anodyne.StructureMap
{
	using System;
	using System.Linq;
	using Abstractions.Dependency;
	using Common.Extentions;
	using global::StructureMap;
	using global::StructureMap.Configuration.DSL.Expressions;
	using global::StructureMap.Pipeline;

	public static class StructureMapContainerRegistrator
    {
        public static void Register(global::StructureMap.IContainer container, SingleBinding binding)
        {
			container.Configure(ce =>
				{
					var registration = ce.For(binding.Service);

					registration = ApplyLifecycle(registration, binding.Lifecycle);

					var unnamed = ApplyResolver(registration, (dynamic)binding.Resolver);

					ApplyName(unnamed, binding.Name);
				});

        }

		public static void Register(global::StructureMap.IContainer container, MultipleBinding binding)
		{
			container.Configure(ce =>
				{
					foreach (var implementation in binding.Services.Where(t => !t.IsInterface && !t.IsAbstract))
					{
						var tempImplementation = implementation;
						binding.BindTo.ForEach(t => ApplyLifecycle(ce.For(t), binding.Lifecycle).Add(tempImplementation));

						if (binding.BindTo.Count == 0)
						{
							ApplyLifecycle(ce.For(implementation), binding.Lifecycle).Add(implementation);
						}
					}
				});
		}

		private static ConfiguredInstance ApplyResolver(GenericFamilyExpression builder, StaticResolver resolver)
		{
			return builder.Add(resolver.Target);
		}

		private static ObjectInstance ApplyResolver(GenericFamilyExpression builder, InstanceResolver resolver)
		{
			return builder.Add(resolver.Instance);
		}

		private static LambdaInstance<object> ApplyResolver(GenericFamilyExpression builder, DynamicResolver resolver)
		{
			return builder.Add(c => resolver.FactoryFunc());
		}

		private static GenericFamilyExpression ApplyLifecycle(GenericFamilyExpression registration, Lifecycle lifecycle)
        {
            if (lifecycle.Name == Lifecycle.Singleton.Name)
                return registration.Singleton();

            if (lifecycle.Name == Lifecycle.Transient.Name)
                return registration.LifecycleIs(InstanceScope.PerRequest);

            if (lifecycle.Name == Lifecycle.PerWebRequest.Name)
				return registration.HttpContextScoped();

			if (lifecycle.Name == Lifecycle.Unmanaged.Name)
				return registration;

			if (lifecycle.Name == Lifecycle.ProviderDefault.Name)
				return registration;

			if (lifecycle.Name == Lifecycle.Default.Name)
				return registration.Singleton();

			throw new ArgumentException(string.Format("Unknown Lifecycle : {0}", lifecycle), "lifecycle");
        }

		private static void ApplyName(dynamic registration, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            registration.Named(name);
        }

    }
}