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
    using System.Collections.Generic;
    using Base;
    using Policy;
    using Domain.Events;
    using Abstractions.DataAccess;

    public class DomainDataSession : IDomainDataSession
    {
        private readonly IDataSession _dataSession;
        protected IDictionary<object, AggregateRootChangeSet> ChangeSets = new Dictionary<object, AggregateRootChangeSet>();

        public IDataSession DataSession { get { return _dataSession; } }

        public DomainDataSession(IDataSession dataSession)
        {
            _dataSession = dataSession;
        }

        protected void RegisterEvent(IAggregateEvent ev)
        {
            AggregateRootChangeSet changeSet;
            if (!ChangeSets.TryGetValue(ev.Target.IdObject, out changeSet))
            {
                changeSet = new AggregateRootChangeSet(ev.Target);
                ChangeSets.Add(ev.Target.IdObject, changeSet);
            }
			changeSet.Register(ev);
        }

        public void Handle(IAggregateEvent @event)
        {
            RegisterEvent(@event);
        }

        protected bool ApplyChangeSet(AggregateRootChangeSet changeSet, bool ignoreConflicts = false)
        {
            var type = changeSet.Aggregate.GetType();

	        var targetVersion = ignoreConflicts ? (long?) null : changeSet.TargetVersion;

			if (changeSet.IsDeleted)
				return _dataSession.RemoveOne(type, changeSet.Aggregate.IdObject, targetVersion);

			return _dataSession.SaveOne(changeSet.Aggregate, targetVersion);
        }

        public DataChangeSet SaveChanges(StaleDataPolicy staleDataPolicy)
        {
            var appliedEvents = new List<IAggregateEvent>();
            var staleData = new List<IAggregateRoot>();

            foreach (var changeSet in ChangeSets.Values)
            {
                if (ApplyChangeSet(changeSet, staleDataPolicy == StaleDataPolicy.Ignore))
                {
                    appliedEvents.AddRange(changeSet.GetStreamOfEvents());
                }
                else
                {
                    staleData.Add(changeSet.Aggregate);
                }
            }

            ChangeSets.Clear();

            return new DataChangeSet(appliedEvents, staleData);
        }

        public void ForgetChanges()
        {
            ChangeSets.Clear();
        }

        public virtual void Dispose()
        {
            _dataSession.Dispose();
        }
    }
}