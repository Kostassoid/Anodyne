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

namespace Kostassoid.Anodyne.Domain.DataAccess
{
    using System;
    using Abstractions.DataAccess;
    using Operations;
    using Policy;

    public static class DataAccessTargetSelectorEx
    {
        public static void AsDomainStorage(this DataAccessTargetSelector selector, Action<DomainDataAccessConfigurator> cc = null)
        {
            UnitOfWork.SetDataSessionFactory(selector.DataProvider.SessionFactory);
            UnitOfWork.SetOperationResolver(new ContainerOperationResolver(selector.Selector.Container));

            if (cc != null)
                cc(new DomainDataAccessConfigurator());
        }
    }

    public class DomainDataAccessConfigurator
    {
        /// <summary>
        /// Define default Data Action policy for working with domain entities (UnitOfWork).
        /// </summary>
        /// <param name="policyAction">Data access policy configurator.</param>
        public void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction)
        {
            var dataPolicy = new DataAccessPolicy();
            policyAction(dataPolicy);

            UnitOfWork.EnforcePolicy(dataPolicy);
        }
    }
}