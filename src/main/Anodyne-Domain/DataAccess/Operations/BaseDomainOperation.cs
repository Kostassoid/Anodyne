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

using System;

namespace Kostassoid.Anodyne.Domain.DataAccess.Operations
{
    public abstract class BaseDomainOperation : IDomainOperation
    {
        protected UnitOfWork Owner { get; private set; }

        protected BaseDomainOperation()
        {
            if (UnitOfWork.Current.IsNone)
            {
                throw new InvalidOperationException("Should be within UnitOfWork context!");
            }

            Owner = UnitOfWork.Current.Value;
        }
    }
}