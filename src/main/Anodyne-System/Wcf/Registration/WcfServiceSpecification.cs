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

namespace Kostassoid.Anodyne.System.Wcf.Registration
{
    using global::System.Collections.Generic;

    public class WcfServiceSpecification<TService, TImpl> : IWcfServiceConfiguration
    {
        public IList<WcfEndpointSpecification> Endpoints { get; set; }
        public bool PublishMetadata { get; set; }
        public string BaseAddress { get; set; }

        public WcfServiceSpecification()
        {
            Endpoints = new List<WcfEndpointSpecification>();
        }

        void IWcfServiceConfiguration.BaseAddress(string address)
        {
            BaseAddress = address;
        }

        void IWcfServiceConfiguration.Endpoint(WcfEndpointSpecification endpointSpecification)
        {
            Endpoints.Add(endpointSpecification);
        }

        void IWcfServiceConfiguration.PublishedMetadata()
        {
            PublishMetadata = true;
        }
    }
}