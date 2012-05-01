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
// 

namespace Kostassoid.BlogNote.Host.Startup
{
    using System.ServiceModel;
    using Anodyne.System;
    using Anodyne.System.Configuration;
    using Anodyne.Windsor;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Contracts;
    using Service;

    public class WcfServicesRegistration : IStartupAction, IShutdownAction
    {
        public void OnStartup(IConfigurationSettings configuration)
        {
            var windsorContainer = (configuration.Container as WindsorContainerAdapter).NativeContainer;

            var userServiceModel = new DefaultServiceModel()
                .AddBaseAddresses(Configured.From.AppSettings("UserServiceUrl"))
                .AddEndpoints(WcfEndpoint.BoundTo(new BasicHttpBinding()))
                .PublishMetadata(o => o.EnableHttpGet());

            windsorContainer.Register(Component.For<IUserService>().ImplementedBy<UserService>().AsWcfService(userServiceModel));

            //Publish<TService>().ImplementedBy<TImpl>().At(Endpoint.)
        }

        public void OnShutdown(IConfigurationSettings configuration)
        {
        }
    }
}