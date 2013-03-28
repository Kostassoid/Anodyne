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

namespace Kostassoid.Anodyne.Abstractions.Dependency
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Registration;

    /// <summary>
    /// IoC Container interface.
    /// </summary>
    public interface IContainer
    {
        /// <summary>
        /// Resolve instance of specific type.
        /// </summary>
        /// <typeparam name="T">Service type of required component.</typeparam>
        /// <returns>Resolved instance or throws if no component was found.</returns>
        T Get<T>();
        /// <summary>
        /// Resolve component of specific type with specific name.
        /// </summary>
        /// <typeparam name="T">Service type of required component.</typeparam>
        /// <param name="name">Component name.</param>
        /// <returns>Resolved instance or throws if no component was found.</returns>
        T Get<T>(string name);
        /// <summary>
        /// Resolve instance of specific type.
        /// </summary>
        /// <param name="type">Service type of required component.</param>
        /// <returns>Resolved instance or throws if no component was found.</returns>
        object Get(Type type);
        /// <summary>
        /// Resolve component of specific type with specific name.
        /// </summary>
        /// <param name="type">Service type of required component.</param>
        /// <param name="name">Component name.</param>
        /// <returns>Resolved instance or throws if no component was found.</returns>
        object Get(Type type, string name);
        /// <summary>
        /// Resolve all components of specific type.
        /// </summary>
        /// <typeparam name="T">Service type of required components.</typeparam>
        /// <returns>List of resolved instances, or empty list if no components were found.</returns>
        IList<T> GetAll<T>();
        /// <summary>
        /// Resolve all components of specific type.
        /// </summary>
        /// <param name="type">Service type of required components.</param>
        /// <returns>List of resolved instances, or empty list if no components were found.</returns>
        IList GetAll(Type type);

        /// <summary>
        /// Release previously resolved component allowing it to be GCed.
        /// </summary>
        /// <param name="instance">Component instance.</param>
        void Release(object instance);

        /// <summary>
        /// Register (put) component in container.
        /// </summary>
        /// <param name="binding">Binding specification.</param>
        void Put(IBindingSyntax binding);

		/// <summary>
		/// Check if component for specific type is registered in container.
		/// </summary>
		/// <typeparam name="T">Component service type.</typeparam>
		/// <returns>True if component is registered.</returns>
		bool Has<T>();

		/// <summary>
		/// Check if component for specific type is registered in container.
		/// </summary>
		/// <param name="type">Component service type.</param>
		/// <returns>True if component is registered.</returns>
		bool Has(Type type);

		/// <summary>
		/// Check if component for specific type is registered in container.
		/// </summary>
		/// <typeparam name="T">Component service type.</typeparam>
		/// <param name="name">Component name.</param>
		/// <returns>True if component is registered.</returns>
		bool Has<T>(string name);

		/// <summary>
		/// Check if component for specific type is registered in container.
		/// </summary>
		/// <param name="type">Component service type.</param>
		/// <param name="name">Component name.</param>
		/// <returns>True if component is registered.</returns>
		bool Has(Type type, string name);
    }
}