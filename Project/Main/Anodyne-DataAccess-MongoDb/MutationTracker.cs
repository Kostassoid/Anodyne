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

namespace Kostassoid.Anodyne.DataAccess.MongoDb
{
    using System;
    using Domain.Events;
    using Wiring;

    public class MutationTracker : IHandlerOf<IMutationEvent>
    {
        public MutationTracker()
        {
            SetupSubscriptions();
        }

        public void SetupSubscriptions()
        {
            EventBus.SubscribeTo().AllBasedOn<IMutationEvent>().From(a => a.Contains("Domain")).With(this);
        }

        public void Handle(IMutationEvent @event)
        {
            var aggregateEvent = @event as IAggregateEvent;
            if (aggregateEvent == null) return;

            var uow = UnitOfWork.Current;
            if (uow.IsNone)
                throw new InvalidOperationException(String.Format("Should be inside UnitOfWork context to handle {0} for {1} ", aggregateEvent.GetType().Name, aggregateEvent.AggregateId));

            uow.Value.MarkAsUpdated(aggregateEvent.Aggregate);
        }
    }
}