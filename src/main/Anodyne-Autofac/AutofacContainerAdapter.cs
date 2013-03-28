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
	using System.Collections;
	using System.Linq;
	using System.Reflection;
	using Abstractions.Dependency.Registration;
	using System;
	using System.Collections.Generic;
	using global::Autofac;
	using IContainer = Abstractions.Dependency.IContainer;

	public class AutofacContainerAdapter : IContainer
    {
        public global::Autofac.IContainer NativeContainer { get; protected set; }

		public AutofacContainerAdapter(global::Autofac.IContainer container)
        {
            NativeContainer = container;
        }

		public AutofacContainerAdapter()
		{
			NativeContainer = new ContainerBuilder().Build();
		}

        public IList<T> GetAll<T>()
        {
			return NativeContainer.Resolve<IEnumerable<T>>().ToList();
        }

        public T Get<T>()
        {
            return NativeContainer.Resolve<T>();
        }

        public T Get<T>(string name)
        {
            return NativeContainer.ResolveNamed<T>(name);
        }

        public IList GetAll(Type type)
        {
	        var enumerableType = typeof (IEnumerable<>).MakeGenericType(type);
			var enumerableToListMethod = typeof(Enumerable).GetMethod("ToList", BindingFlags.Public | BindingFlags.Static);
			var genericToListMethod = enumerableToListMethod.MakeGenericMethod(new[] { type });

	        var enumerable = ((IEnumerable) NativeContainer.Resolve(enumerableType));

	        return (IList)genericToListMethod.Invoke(null, new[] { enumerable });
        }

        public object Get(Type type)
        {
            return NativeContainer.Resolve(type);
        }

        public object Get(Type type, string name)
        {
            return NativeContainer.ResolveNamed(name, type);
        }

        public void Release(object instance)
        {
			// emulating disposal
	        var disposable = instance as IDisposable;

	        if (disposable != null)
				disposable.Dispose();
        }

		public void Put(IBindingSyntax binding)
        {
            AutofacContainerRegistrator.Register(NativeContainer, (dynamic)binding.Binding);
        }

        public bool Has<T>()
        {
            return NativeContainer.IsRegistered<T>();
        }

		public bool Has(Type type)
		{
			return NativeContainer.IsRegistered(type);
		}

		public bool Has<T>(string name)
		{
			return NativeContainer.IsRegisteredWithName<T>(name);
		}

		public bool Has(Type type, string name)
		{
			return NativeContainer.IsRegisteredWithName(name, type);
		}
    }
}