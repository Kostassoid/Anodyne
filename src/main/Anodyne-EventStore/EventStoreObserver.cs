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

namespace Kostassoid.Anodyne.EventStore
{
    using System;
    using Adapters;
    using Domain.DataAccess;
    using Domain.DataAccess.Events;
    using Wiring;

    public class EventStoreObserver
    {
        private readonly IEventStoreAdapter _adapter;
        private Action _stopAction = () => { };

        public EventStoreObserver(IEventStoreAdapter adapter)
        {
            _adapter = adapter;
        }

        public void Start()
        {
            _stopAction += EventBus.SubscribeTo<UnitOfWorkCompleted>().With(ev => Handle(ev.ChangeSet));
            _stopAction += EventBus.SubscribeTo<UnitOfWorkFailed>().With(ev => Handle(ev.ChangeSet));
        }

        private void Handle(DataChangeSet changeSet)
        {
            _adapter.Store(changeSet.AppliedEvents);
        }

        public void Stop()
        {
            _stopAction();
            _stopAction = () => { };
        }
    }
}