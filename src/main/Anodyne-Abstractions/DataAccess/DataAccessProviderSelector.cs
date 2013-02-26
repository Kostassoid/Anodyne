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

namespace Kostassoid.Anodyne.Abstractions.DataAccess
{
    using Common;
    using Common.CodeContracts;
    using Dependency;
    using Dependency.Registration;

    /// <summary>
    /// Data access provider selector.
    /// </summary>
    public class DataAccessProviderSelector : ISyntax
    {
        internal string Name { get; private set; }
        internal IContainer Container { get; private set; }

        internal DataAccessProviderSelector(string name, IContainer container)
        {
            Requires.NotNullOrEmpty(name, "name");
            Requires.NotNull(container, "container");

            Name = name;
            Container = container;
        }

        /// <summary>
        /// Use provided data access provider instance for this configuration.
        /// </summary>
        /// <param name="dataAccessProvider">Fully configured data access provider instance.</param>
        /// <returns>Data access target selector.</returns>
        public DataAccessTargetSelector Use(IDataAccessProvider dataAccessProvider)
        {
            var providerName = "DataAccessProvider-" + Name;
            Requires.True(!Container.Has(providerName), message: string.Format("DataAccessProvider with name '{0}' is already registered, use another name.", Name));
            
            Container.Put(Binding.For<IDataAccessProvider>().UseInstance(dataAccessProvider).Named(providerName));

            return new DataAccessTargetSelector(this, dataAccessProvider);
        }
    }
}