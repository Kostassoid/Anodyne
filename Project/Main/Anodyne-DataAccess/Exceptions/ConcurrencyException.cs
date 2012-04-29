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

namespace Kostassoid.Anodyne.DataAccess.Exceptions
{
    using System;
    using Domain.Base;

    public class ConcurrencyException : Exception
    {
        public IAggregateRoot Aggregate { get; protected set; }

        public ConcurrencyException(IAggregateRoot aggregate)
            :base(string.Format("Two different versions of aggregate root {0} of type '{1}' was detected in one DataSession",
                        aggregate.IdObject, aggregate.GetType().Name))
        {
            Aggregate = aggregate;
        }
    }
}