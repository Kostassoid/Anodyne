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

namespace Kostassoid.Anodyne.Abstractions.Dependency.Registration
{
    using System;
    using System.Collections.Generic;
    using Internal;

    /// <summary>
    /// Define component(s) binding specification.
    /// </summary>
    public static class Binding
    {
		/// <summary>
		/// Register single component using concrete type.
		/// </summary>
		/// <param name="implementation">Component implementation type.</param>
		/// <returns>Additional registration options.</returns>
		public static ISingleBindingSyntax Use(Type implementation)
		{
			return new SingleBindingSyntax(implementation, new StaticResolver(implementation));
		}

		/// <summary>
		/// Register single component using concrete type.
		/// </summary>
		/// <typeparam name="TImpl">Component implementation type.</typeparam>
		/// <returns>Additional registration options.</returns>
		public static ISingleBindingSyntax<TImpl> Use<TImpl>() where TImpl : class
		{
			return new SingleBindingSyntax<TImpl>(new StaticResolver(typeof(TImpl)));
		}

		/// <summary>
		/// Register single component using factory method.
		/// </summary>
		/// <typeparam name="TImpl">Component implementation type.</typeparam>
		/// <param name="factoryFunc">Factory method to build component.</param>
		/// <returns>Additional registration options.</returns>
		public static ISingleBindingSyntax<TImpl> Use<TImpl>(Func<TImpl> factoryFunc) where TImpl : class
		{
			return new SingleBindingSyntax<TImpl>(new DynamicResolver(factoryFunc));
		}

		/// <summary>
		/// Register single component using component instance.
		/// </summary>
		/// <typeparam name="TImpl">Component implementation type.</typeparam>
		/// <param name="instance">Component instance.</param>
		/// <returns>Additional registration options.</returns>
		public static ISingleBindingSyntax<TImpl> Use<TImpl>(TImpl instance) where TImpl : class
		{
			return new SingleBindingSyntax<TImpl>(new InstanceResolver(instance));
		}

		/// <summary>
        /// Register multiple components for specific service type.
        /// </summary>
        /// <param name="services">Components service type.</param>
        /// <returns>Additional registration options.</returns>
        public static IMultipleBindingSyntax Use(IEnumerable<Type> services)
        {
            return new MultipleBindingSyntax(services);
        }
    }
}