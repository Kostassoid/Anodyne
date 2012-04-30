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
    using Operations;
    using global::System;
    using global::System.Collections.Generic;

    public abstract class DataSession : IDataSession
    {
        protected IDictionary<object, AggregateRootChangeSet> ChangeSets = new Dictionary<object, AggregateRootChangeSet>();

        protected IOperationResolver OperationResolver { get; private set; }

        protected DataSession(IOperationResolver operationResolver)
        {
            OperationResolver = operationResolver;
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

        public abstract IRepository<TRoot> GetRepository<TRoot>() where TRoot : class, IAggregateRoot;

        public TOp GetOperation<TOp>() where TOp : class, IDataOperation
        {
            var operation = OperationResolver.Get<TOp>();

            if (operation == null)
            {
                throw new Exception(String.Format("Operation {0} wasn't found. Check your configuration.", typeof(TOp).Name));
            }

            return operation;
        }

        public void MarkAsDeleted<TRoot>(TRoot aggregate) where TRoot : class, IAggregateRoot
        {
            GetChangeSetFor(aggregate).MarkAsDeleted();
        }

        protected bool ApplyChangeSet(AggregateRootChangeSet changeSet)
        {
            var type = changeSet.Aggregate.GetType();

            var storedAggregate = FindOne(type, changeSet.Aggregate.IdObject);

            //var collection = _nativeSession.GetCollection(type);
            //var storedAggregate = collection.FindOneByIdAs(type, changeSet.Aggregate.IdObject.ToBson());

            if (storedAggregate == null && !changeSet.IsNew)
                return false;

            if (storedAggregate != null && storedAggregate.Version != changeSet.TargetVersion)
                return false;

            SaveOne(type, changeSet.Aggregate);

            if (changeSet.IsDeleted)
                RemoveOne(type, changeSet.Aggregate.IdObject);

            return true;
        }

        public DataChangeSet SaveChanges()
        {
            var appliedEvent = new List<IAggregateEvent>();
            var staleDate = new List<IAggregateRoot>();

            foreach (var changeSet in ChangeSets.Values)
            {
                if (ApplyChangeSet(changeSet))
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

        public void Dispose() {}

        protected abstract IAggregateRoot FindOne(Type type, object id);
        protected abstract void SaveOne(Type type, IAggregateRoot root);
        protected abstract void RemoveOne(Type type, object id);

    }
}