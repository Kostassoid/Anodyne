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

namespace Kostassoid.Anodyne.Domain.Events
{
    using System;
    using Common;
    using Base;
    using Common.Tools;

    [Serializable]
    public abstract class AggregateEvent<TRoot> : AggregateRoot<Guid>, IAggregateEvent where TRoot : IAggregateRoot
    {
        private readonly TRoot _aggregate;
        public object AggregateId { get; protected set; }

        public DateTime Happened { get; protected set; }

        // we don't want version-tracking events
        public override int Version
        {
            get { return 0; }
        }

        // should not be stored!
        public TRoot Target { get { return _aggregate; } }

        // should not be stored!
        IAggregateRoot IAggregateEvent.Aggregate { get { return _aggregate; } }

        public int AggregateVersion { get; protected set; }

        private bool _isReplaying;
        public bool IsReplaying { get { return _isReplaying; } }

        protected AggregateEvent(TRoot aggregate, DateTime happened)
        {
            Id = SeqGuid.NewGuid();

            _aggregate = aggregate;
            Happened = happened;

            AggregateId = ((IEntity) aggregate).IdObject;
            AggregateVersion = aggregate.NewVersion();
        }

        protected AggregateEvent(TRoot aggregate)
            : this(aggregate, SystemTime.Now)
        {
        }

        public void MarkAsReplaying()
        {
            _isReplaying = true;
        }

    }
}