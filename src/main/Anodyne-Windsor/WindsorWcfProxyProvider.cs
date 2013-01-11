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

using System.Linq;

namespace Kostassoid.Anodyne.Windsor
{
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.Windsor;
    using Node.Configuration;
    using Node.Wcf;
    using Node.Wcf.Registration;
    using System;

    public class WindsorWcfProxyProvider : WcfProxyProvider
    {
        private readonly IWindsorContainer _container;

        public WindsorWcfProxyProvider(IConfiguration configuration)
        {
            var containerAdapter = ((INodeInstance)configuration).Container;

            if (!(containerAdapter is WindsorContainerAdapter))
                throw new InvalidOperationException("WindsorWcfServicePublisher requires Windsor Container");

            _container = (containerAdapter as WindsorContainerAdapter).NativeContainer;

            _container.AddFacility<WcfFacility>();
        }

        private static IWcfEndpoint GetBindingEndpointModelFrom(WcfEndpointSpecification endpointSpecification)
        {
            var endpoint = WcfEndpoint.BoundTo(endpointSpecification.Binding);

            if (!string.IsNullOrEmpty(endpointSpecification.Address))
                return endpoint.At(endpointSpecification.Address);

            return endpoint;
        }

        public override void Consume<TService>(WcfEndpointSpecification endpoint)
        {
            _container.Register(Component.For<TService>().AsWcfClient(GetBindingEndpointModelFrom(endpoint)));
        }

        public override void Publish<TService, TImpl>(WcfServiceSpecification<TService, TImpl> specification)
        {
            var serviceModel = new DefaultServiceModel();

            if (!string.IsNullOrEmpty(specification.BaseAddress))
                serviceModel = serviceModel.AddBaseAddresses(specification.BaseAddress);

            if (specification.PublishMetadata)
                serviceModel = serviceModel.PublishMetadata(o => o.EnableHttpGet());

            serviceModel = serviceModel.AddEndpoints(specification.Endpoints.Select(GetBindingEndpointModelFrom).ToArray());

            _container.Register(Component.For<TService>().ImplementedBy<TImpl>().AsWcfService(serviceModel));
        }
    }
}