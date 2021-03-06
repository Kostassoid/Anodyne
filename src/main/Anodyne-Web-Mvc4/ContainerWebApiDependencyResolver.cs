﻿// Copyright 2011-2013 Anodyne.
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

using System;
using System.Collections.Generic;
using System.Linq;
using Kostassoid.Anodyne.Abstractions.Dependency;

namespace Kostassoid.Anodyne.Web.Mvc4
{
    public class ContainerWebApiDependencyResolver : System.Web.Http.Dependencies.IDependencyResolver 
    {
        private readonly IContainer _container;

        public ContainerWebApiDependencyResolver(IContainer container)
        {
            _container = container;
        }
        
        public System.Web.Http.Dependencies.IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return _container.Has(serviceType) ? _container.Get(serviceType) : null;
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return _container.Has(serviceType) ? _container.GetAll(serviceType).Cast<object>() : new object[] { };
        }

        public void Dispose(){}
    }
}
