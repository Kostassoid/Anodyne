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
	using System.Linq;

	public class AggregateRootChangeSet
    {
        public IAggregateRoot Aggregate { get; protected set; }
        public long TargetVersion { get; protected set; }
        public long CurrentVersion { get; protected set; }
        public bool IsDeleted { get; protected set; }
        public bool IsNew { get { return TargetVersion == 0; } }

        private IList<IAggregateEvent> Events { get; set; }

        public AggregateRootChangeSet(IAggregateRoot aggregate)
        {
            Aggregate = aggregate;
            //TODO: looks fragile
            CurrentVersion = TargetVersion = aggregate.Version; // Aggregate version is already incremented at this moment
            Events = new List<IAggregateEvent>();
        }

        public void Register(IAggregateEvent @event)
        {
            var ev = (IUncommitedEvent) @event;
            if (Aggregate != ev.Target || CurrentVersion != ev.TargetVersion)
                throw new ConcurrencyException(ev.Target, ev, CurrentVersion);

            Events.Add(@event);
	        CurrentVersion = @event.TargetVersion + 1;
        }

		public IEnumerable<IAggregateEvent> GetStreamOfEvents()
		{
			return Events.OrderBy(e => e.TargetVersion);
		}
         
    }
}