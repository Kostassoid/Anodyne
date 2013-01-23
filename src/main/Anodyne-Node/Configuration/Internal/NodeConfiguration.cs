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

namespace Kostassoid.Anodyne.Node.Configuration.Internal
{
    using Abstractions.DataAccess;
    using Abstractions.Dependency;
    using Abstractions.Wcf;

    internal class NodeConfiguration : INodeConfiguration
    {
        public RuntimeMode RuntimeMode { get; internal set; }
        public IContainer Container { get; internal set; }
        public IWcfProxyFactory WcfProxyFactory { get; internal set; }
        public IDataAccessProvider DataAccess { get; internal set; }
        public string SystemNamespace { get; internal set; }
    }
}