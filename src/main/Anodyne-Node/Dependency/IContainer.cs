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

using Kostassoid.Anodyne.Node.Dependency.Registration;

namespace Kostassoid.Anodyne.Node.Dependency
{
    using System;
    using System.Collections;
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

        // container.Put(Binding.For<IFoo>().AsSelf().Named("Ololo").WithLifestyle(Lifestyle.Singleton))
        // container.Put(Binding.For(AllTypes.BasedOn<IFoo>()))
        void Put(IBindingSyntax binding);

        //ISingleBindingSyntax<TService> For<TService>() where TService : class;
        //ISingleBindingSyntax For(Type type);
        //IMultipleBindingSyntax For(IEnumerable<Type> types);

        bool Has<T>();
        bool Has(Type type);
    }
}