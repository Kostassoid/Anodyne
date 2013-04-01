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
	using System.Collections;
	using System.Linq;
	using Abstractions.Dependency;
	using Abstractions.Dependency.Registration;
	using System;
	using System.Collections.Generic;
	using global::StructureMap;
	using IContainer = Abstractions.Dependency.IContainer;

	public class StructureMapContainerAdapter : IContainer
    {
        public global::StructureMap.IContainer NativeContainer { get; protected set; }

		public StructureMapContainerAdapter(global::StructureMap.IContainer container)
        {
            NativeContainer = container;
        }

		public StructureMapContainerAdapter()
		{
			NativeContainer = new Container();
		}

        public IList<T> GetAll<T>()
        {
			return NativeContainer.GetAllInstances<T>();
        }

		public IList GetAll(Type type)
		{
			return NativeContainer.GetAllInstances(type);
		}
		
		public T Get<T>()
        {
			if (!Has<T>())
				throw new BindingNotRegisteredException(typeof(T), null);

			try
			{
				return NativeContainer.GetInstance<T>();
			}
			catch (StructureMapException ex)
			{
				throw new BindingNotRegisteredException(typeof(T), ex);
			}
        }

        public T Get<T>(string name)
        {
			if (!Has<T>(name))
				throw new BindingNotRegisteredException(typeof(T), name, null);

			try
	        {
				return NativeContainer.GetInstance<T>(name);
	        }
			catch (StructureMapException ex)
	        {
		        throw new BindingNotRegisteredException(typeof(T), name, ex);
	        }
		}

        public object Get(Type type)
        {
			if (!Has(type))
				throw new BindingNotRegisteredException(type, null);

			try
			{
				return NativeContainer.GetInstance(type);
			}
			catch (StructureMapException ex)
			{
				throw new BindingNotRegisteredException(type, ex);
			}
		}

        public object Get(Type type, string name)
        {
			if (!Has(type, name))
				throw new BindingNotRegisteredException(type, name, null);

			try
			{
				return NativeContainer.GetInstance(type, name);
			}
			catch (StructureMapException ex)
			{
				throw new BindingNotRegisteredException(type, name, ex);
			}
		}

        public void Release(object instance)
        {
        }

		public void Put(IBindingSyntax binding)
        {
            StructureMapContainerRegistrator.Register(NativeContainer, (dynamic)binding.Binding);
        }

        public bool Has<T>()
        {
            return NativeContainer.Model.HasImplementationsFor<T>();
        }

		public bool Has(Type type)
		{
			return NativeContainer.Model.HasImplementationsFor(type);
		}

		public bool Has<T>(string name)
		{
			return NativeContainer.Model.InstancesOf<T>().Any(x => x.Name == name);
		}

		public bool Has(Type type, string name)
		{
			return NativeContainer.Model.InstancesOf(type).Any(x => x.Name == name);
		}
    }
}