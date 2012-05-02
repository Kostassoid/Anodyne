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
    using System.Configuration;
    using System.Wcf;
    using Castle.Facilities.WcfIntegration;
    using Castle.Windsor;
    using global::System;

    public class WindsorWcfServiceProvider : IWcfServiceProvider
    {
        private readonly IWindsorContainer _container;

        public WindsorWcfServiceProvider(IConfiguration configuration)
        {
            var containerAdapter = (configuration as ISystemConfiguration).Container;

            if (!(containerAdapter is WindsorContainerAdapter))
                throw new InvalidOperationException("WindsorWcfServiceProvider requires Windsor Container");

            _container = (containerAdapter as WindsorContainerAdapter).NativeContainer;

            _container.AddFacility<WcfFacility>();
        }
    }
}