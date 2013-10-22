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

namespace Kostassoid.Anodyne.Web.Mvc
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Web.Mvc;
    using Common.Reflection;
    using Abstractions.Dependency;
    using Abstractions.Dependency.Registration;
    using Node.Configuration;

    public static class ConfigurationEx
    {
        public static void ResolveControllersFromContainer(this INodeConfigurator nodeConfigurator)
        {
            var cfg = nodeConfigurator.Configuration;

            DependencyResolver.SetResolver(new ContainerDependencyResolver(cfg.Container));
            ControllerBuilder.Current.SetControllerFactory(new ContainerControllerFactory(cfg.Container));
        }

        public static void RegisterControllers(this INodeConfigurator nodeConfigurator, IEnumerable<Assembly> assemblies)
        {
            nodeConfigurator.Configuration
                .Container.Put(Binding.Use(AllTypes.BasedOn<IController>(assemblies)).With(Lifecycle.Transient));
        }
    }
}