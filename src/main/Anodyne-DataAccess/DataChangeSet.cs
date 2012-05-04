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

namespace Kostassoid.Anodyne.DataAccess
{
    using Domain.Base;
    using Domain.Events;
    using System.Collections.Generic;

    public class DataChangeSet
    {
        public IList<IAggregateEvent> AppliedEvents { get; protected set; }
        public IList<IAggregateRoot> StaleData { get; protected set; }

        public bool StaleDataDetected { get { return StaleData.Count > 0; } }

        public DataChangeSet(IList<IAggregateEvent> appliedEvents, IList<IAggregateRoot> staleData)
        {
            AppliedEvents = appliedEvents;
            StaleData = staleData;
        }
    }
}