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
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Node.Configuration;
    using Node.Wcf;
    using Node.Wcf.Registration;
    using global::System;

    public class WindsorWcfServicePublisher : WcfServicePublisher
    {
        private readonly IWindsorContainer _container;

        public WindsorWcfServicePublisher(IConfiguration configuration)
        {
            var containerAdapter = (configuration as INodeInstance).Container;

            if (!(containerAdapter is WindsorContainerAdapter))
                throw new InvalidOperationException("WindsorWcfServicePublisher requires Windsor Container");

            _container = (containerAdapter as WindsorContainerAdapter).NativeContainer;

            _container.AddFacility<WcfFacility>();
        }

        public override void Publish<TService, TImpl>(WcfServiceSpecification<TService, TImpl> specification)
        {
            var userServiceModel = new DefaultServiceModel();

            if (!string.IsNullOrEmpty(specification.BaseAddress))
                userServiceModel = userServiceModel.AddBaseAddresses(specification.BaseAddress);

            if (specification.PublishMetadata)
                userServiceModel = userServiceModel.PublishMetadata(o => o.EnableHttpGet());

            foreach (var endpointSpecification in specification.Endpoints)
            {
                var endpoint = WcfEndpoint.BoundTo(endpointSpecification.Binding);
                userServiceModel =
                    string.IsNullOrEmpty(endpointSpecification.Address)
                    ? userServiceModel.AddEndpoints(endpoint)
                    : userServiceModel.AddEndpoints(endpoint.At(endpointSpecification.Address));
            }

            _container.Register(Component.For<TService>().ImplementedBy<TImpl>().AsWcfService(userServiceModel));
        }
    }
}