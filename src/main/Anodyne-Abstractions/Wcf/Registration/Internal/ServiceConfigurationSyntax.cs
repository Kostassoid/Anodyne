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

namespace Kostassoid.Anodyne.Abstractions.Wcf.Registration.Internal
{
    using System;

    internal class ServiceConfigurationSyntax<TService, TImpl> : IServiceConfigurationSyntax<TService, TImpl> where TService : class where TImpl : class, TService
    {
        private readonly WcfProxyFactory _wcfProxyFactory;

        public ServiceConfigurationSyntax(WcfProxyFactory wcfProxyFactory)
        {
            _wcfProxyFactory = wcfProxyFactory;
        }

        public void ConfiguredWith(Action<IWcfServiceConfiguration> configurationAction)
        {
            var specification = new WcfServiceSpecification<TService, TImpl>();

            configurationAction(specification);

            _wcfProxyFactory.Publish(specification);
        }
    }
}