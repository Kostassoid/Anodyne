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

        protected AggregateRootChangeSet GetChangeSetFor(IAggregateRoot aggregate)
        {
            AggregateRootChangeSet changeSet;
            if (!ChangeSets.TryGetValue(aggregate.IdObject, out changeSet))
            {
                changeSet = new AggregateRootChangeSet(aggregate);
                ChangeSets.Add(aggregate.IdObject, changeSet);
            }

            return changeSet;
        }

        public void Handle(IAggregateEvent @event)
        {
            GetChangeSetFor(@event.Aggregate).Register(@event);
        }

        public void MarkAsDeleted<TRoot>(TRoot aggregate) where TRoot : class, IAggregateRoot
        {
            GetChangeSetFor(aggregate).MarkAsDeleted();
        }

        protected bool ApplyChangeSet(AggregateRootChangeSet changeSet, bool ignoreConflicts = false)
        {
            var type = changeSet.Aggregate.GetType();

            var storedAggregate = (IAggregateRoot)_dataSession.FindOne(type, changeSet.Aggregate.IdObject);

            if (!ignoreConflicts)
            {
                if (storedAggregate == null && !changeSet.IsNew)
                    return false;

                if (storedAggregate != null && storedAggregate.Version != changeSet.TargetVersion)
                    return false;
            }

            _dataSession.SaveOne(changeSet.Aggregate);

            if (changeSet.IsDeleted)
                _dataSession.RemoveOne(type, changeSet.Aggregate.IdObject);

            return true;
        }

        //TODO: use stale data policy
        public DataChangeSet SaveChanges(StaleDataPolicy staleDataPolicy)
        {
            var appliedEvent = new List<IAggregateEvent>();
            var staleDate = new List<IAggregateRoot>();

            foreach (var changeSet in ChangeSets.Values)
            {
                if (ApplyChangeSet(changeSet, staleDataPolicy == StaleDataPolicy.Ignore))
                {
                    appliedEvent.AddRange(changeSet.Events);
                }
                else
                {
                    staleDate.Add(changeSet.Aggregate);
                }
            }

            ChangeSets.Clear();

            return new DataChangeSet(appliedEvent, staleDate);
        }

        public void Rollback()
        {
            ChangeSets.Clear();
        }

        public virtual void Dispose()
        {
            _dataSession.Dispose();
        }
    }
}