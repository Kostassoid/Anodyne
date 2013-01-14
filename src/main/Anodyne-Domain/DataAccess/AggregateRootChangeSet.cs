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

using System.Collections.Generic;
using Kostassoid.Anodyne.Domain.Base;
using Kostassoid.Anodyne.Domain.DataAccess.Exceptions;
using Kostassoid.Anodyne.Domain.Events;

namespace Kostassoid.Anodyne.Domain.DataAccess
{
    public class AggregateRootChangeSet
    {
        public IAggregateRoot Aggregate { get; protected set; }
        public int TargetVersion { get; protected set; }
        public int CurrentVersion { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public bool IsNew { get { return TargetVersion == 0; } }

        public IList<IAggregateEvent> Events { get; protected set; }

        public AggregateRootChangeSet(IAggregateRoot aggregate)
        {
            Aggregate = aggregate;
            CurrentVersion = TargetVersion = aggregate.Version - 1; // Aggregate version is already incremented at this moment
            Events = new List<IAggregateEvent>();
        }

        public void Register(IAggregateEvent @event)
        {
            if (Aggregate != @event.Aggregate || CurrentVersion != @event.AggregateVersion)
                throw new ConcurrencyException(Aggregate);

            Events.Add(@event);
            CurrentVersion = @event.Aggregate.Version;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }
         
    }
}