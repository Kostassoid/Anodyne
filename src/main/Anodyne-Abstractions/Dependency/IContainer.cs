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
        /// Resolve all instances of specified type.
        /// </summary>
        /// <typeparam name="T">Type of required component it was registered with.</typeparam>
        /// <returns>List of resolved instances, or empty list if no components were found.</returns>
        IList<T> GetAll<T>();
        /// <summary>
        /// Resolve instance of specified type.
        /// </summary>
        /// <typeparam name="T">Type of required component it was registered with.</typeparam>
        /// <returns>Resolved instance or null.</returns>
        T Get<T>();
        T Get<T>(string name);

        IList GetAll(Type type);
        object Get(Type type);
        object Get(Type type, string name);

        void Release(object instance);

        void Put(IBindingSyntax binding);

        bool Has<T>();
        bool Has(Type type);
    }
}