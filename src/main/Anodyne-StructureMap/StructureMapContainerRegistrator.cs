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

					registration = ApplyLifestyleSingle(registration, binding.Lifestyle);

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
						binding.BindTo.ForEach(t => ApplyLifestyleSingle(ce.For(t), binding.Lifestyle).Use(tempImplementation));

						if (binding.BindTo.Count == 0)
						{
							ApplyLifestyleSingle(ce.For(implementation), binding.Lifestyle).Use(implementation);
						}
					}
				});
		}

		private static ConfiguredInstance ApplyResolver(GenericFamilyExpression builder, StaticResolver resolver)
		{
			return builder.Use(resolver.Target);
		}

		private static ObjectInstance ApplyResolver(GenericFamilyExpression builder, InstanceResolver resolver)
		{
			return builder.Use(resolver.Instance);
		}

		private static LambdaInstance<object> ApplyResolver(GenericFamilyExpression builder, DynamicResolver resolver)
		{
			return builder.Use(c => resolver.FactoryFunc());
		}

		private static GenericFamilyExpression ApplyLifestyleSingle(GenericFamilyExpression registration, Lifestyle lifestyle)
        {
            if (lifestyle.Name == Lifestyle.Singleton.Name)
                return registration.Singleton();

            if (lifestyle.Name == Lifestyle.Transient.Name)
                return registration.LifecycleIs(InstanceScope.PerRequest);

            if (lifestyle.Name == Lifestyle.PerWebRequest.Name)
				return registration.HttpContextScoped();

			if (lifestyle.Name == Lifestyle.Unmanaged.Name)
				return registration;

			if (lifestyle.Name == Lifestyle.ProviderDefault.Name)
				return registration;

			if (lifestyle.Name == Lifestyle.Default.Name)
				return registration.Singleton();

			throw new ArgumentException(string.Format("Unknown lifestyle : {0}", lifestyle), "lifestyle");
        }

		private static void ApplyName(dynamic registration, string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            registration.Named(name);
        }

    }
}