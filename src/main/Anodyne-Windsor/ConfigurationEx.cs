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

namespace Kostassoid.Anodyne.Windsor
{
    using Castle.Windsor;
    using Node.Configuration;
    using System;

    public static class ConfigurationEx
    {
        public static void UseWindsorContainer(this IConfiguration configuration, Func<IWindsorContainer> containerProvider)
        {
            (configuration as IConfigurationBuilder).SetContainerAdapter(new WindsorContainerAdapter(containerProvider()));
        }

        public static void UseWindsorContainer(this IConfiguration configuration)
        {
            (configuration as IConfigurationBuilder).SetContainerAdapter(new WindsorContainerAdapter(new WindsorContainer()));
        }

        public static void UseWindsorWcfServicePublisher(this IConfiguration configuration)
        {
            (configuration as IConfigurationBuilder).SetWcfServiceProvider(new WindsorWcfServicePublisher(configuration));
        }

    }
}