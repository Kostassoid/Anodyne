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

namespace Kostassoid.Anodyne.Node.Dependency
{
    using System;
    using System.Collections;
    using System.Reflection;
    using Registration;
    using System.Collections.Generic;

    public interface IContainer
    {
        IList<T> GetAll<T>();
        T Get<T>();
        T Get<T>(string name);

        IList GetAll(Type type);
        object Get(Type type);
        object Get(Type type, string name);

        void Release(object instance);

        IBindingSyntax<TService> For<TService>() where TService : class;
        ITypePickingSyntax<TService> ForAllBasedOn<TService>(IEnumerable<Assembly> assemblies) where TService : class;

        bool Has<T>();
        bool Has(Type type);
    }
}