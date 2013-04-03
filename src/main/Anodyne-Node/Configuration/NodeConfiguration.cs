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

namespace Kostassoid.Anodyne.Node.Configuration
{
	using Abstractions.DataAccess;
	using Abstractions.Dependency;
    using Abstractions.Wcf;
	using Common.CodeContracts;

	/// <summary>
    /// Access to base Node configuration and depencencies.
    /// </summary>
    public class NodeConfiguration
    {
        /// <summary>
        /// Node instance runtime mode.
        /// </summary>
        public RuntimeMode RuntimeMode { get; internal set; }
        /// <summary>
        /// IoC container.
        /// </summary>
        public IContainer Container { get; internal set; }
        /// <summary>
        /// Wcf Proxy Factory.
        /// </summary>
        public IWcfProxyFactory WcfProxyFactory { get { return Container.Get<IWcfProxyFactory>(); } }
        /// <summary>
        /// System (project) namespace.
        /// </summary>
        public string SystemNamespace { get; internal set; }

		private DataAccessProviderResolver _dataAccessProviderResolver;

		/// <summary>
		/// Provides access to registered data access providers.
		/// </summary>
		public DataAccessProviderResolver DataAccess
		{
			get
			{
				if (_dataAccessProviderResolver == null)
				{
					Assumes.True(Container != null, "Container should be set before using any DataAccess related methods/properties.");
					_dataAccessProviderResolver = new DataAccessProviderResolver(Container);
				}

				return _dataAccessProviderResolver;
			}
		}
    }
}