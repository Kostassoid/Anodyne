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
    using Exceptions;
    using global::System.Collections.Generic;

    public class AggregateRootChangeSet
    {
        public IAggregateRoot Aggregate { get; protected set; }
        public int TargetVersion { get; protected set; }
        public int CurrentVersion { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public bool IsNew { get { return TargetVersion == 1; } }

        public IList<IAggregateEvent> Events { get; protected set; }

        public AggregateRootChangeSet(IAggregateRoot aggregate)
        {
            Aggregate = aggregate;
            TargetVersion = CurrentVersion = aggregate.Version;
            Events = new List<IAggregateEvent>();
        }

        public void Register(IAggregateEvent @event)
        {
            if (Aggregate != @event.Aggregate || CurrentVersion != @event.Aggregate.Version)
                throw new ConcurrencyException(Aggregate);

            Events.Add(@event);
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
         
    }
}