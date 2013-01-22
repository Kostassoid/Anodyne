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

namespace Kostassoid.Anodyne.Domain.DataAccess.Operations
{
    using System;
    using Abstractions.Dependency;
    using Domain;

    /// <summary>
    /// Domain operation resolver based on Container.
    /// </summary>
    public class ContainerOperationResolver : IOperationResolver
    {
        private readonly IContainer _container;

        /// <summary>
        /// Construct an instance of ContainerOperationResolver.
        /// </summary>
        /// <param name="container">Container to use for resolving operations.</param>
        public ContainerOperationResolver(IContainer container)
        {
            _container = container;
        }

        /// <summary>
        /// Resolve domain operation.
        /// </summary>
        /// <typeparam name="TOp">Tytpe of concrete IDomainOperation implementation.</typeparam>
        /// <returns>Resolved instance of IDomainOperation.</returns>
        public TOp Get<TOp>() where TOp : class, IDomainOperation
        {
            var operation = _container.Get<TOp>();

            if (operation == null)
            {
                throw new ArgumentException(String.Format("Operation {0} wasn't found. Check your configuration.", typeof(TOp).Name));
            }

            return operation;
        }
    }
}