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

using Kostassoid.Anodyne.Common.Reflection;
using Kostassoid.Anodyne.Node.Dependency.Registration;

namespace Kostassoid.Anodyne.Web.Mvc
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Mvc;
    using Node.Configuration;
    using Node.Dependency;

    public static class ConfigurationEx
    {
        public static void ResolveControllersFromContainer(this IConfiguration configuration)
        {
            var node = (INodeInstance)configuration;

            DependencyResolver.SetResolver(new ContainerDependencyResolver(node.Container));
            ControllerBuilder.Current.SetControllerFactory(new ContainerControllerFactory(node.Container));
        }

        public static void RegisterControllers(this IConfiguration configuration, IEnumerable<Assembly> assemblies)
        {
            var node = (INodeInstance)configuration;

            node.Container.Put(Binding.Use(AllTypes.BasedOn<IController>()).As<IController>().With(Lifestyle.Transient));
        }
    }
}