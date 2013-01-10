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

namespace Kostassoid.BlogNote.Web.Installers
{
    using System.Configuration;
    using System.ServiceModel;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Contracts;

    public class WcfClientsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var userServiceUrl = ConfigurationManager.AppSettings["UserServiceUrl"];

            //container.AddFacility<WcfFacility>();
            container.Register(Component.For<IUserService>().AsWcfClient(WcfEndpoint.BoundTo(new BasicHttpBinding()).At(userServiceUrl)));
        }
    }
}