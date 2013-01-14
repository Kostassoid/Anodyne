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

namespace Kostassoid.Anodyne.Node.Wcf.Registration.Concrete
{
    internal class ServiceImplementationSyntax<TService> : IServiceImplementationSyntax<TService> where TService : class
    {
        private readonly WcfAdapter _wcfAdapter;

        public ServiceImplementationSyntax(WcfAdapter wcfAdapter)
        {
            _wcfAdapter = wcfAdapter;
        }

        public IServiceConfigurationSyntax<TService, TImpl> ImplementedBy<TImpl>() where TImpl : class, TService
        {
            return new ServiceConfigurationSyntax<TService, TImpl>(_wcfAdapter);
        }
    }
}