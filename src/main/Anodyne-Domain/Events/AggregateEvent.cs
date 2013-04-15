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
    public abstract class AggregateEvent<TRoot> : IAggregateEvent, IUncommitedEvent where TRoot : class, IAggregateRoot
    {
		public Guid Id { get; private set; }
		public DateTime Happened { get; private set; }
        public long TargetVersion { get; private set; }
        public int SchemaVersion { get; private set; }

        IAggregateRoot IUncommitedEvent.Target { get; set; }

        protected AggregateEvent(IAggregateRoot target, long targetVersion, DateTime happened)
        {
            Id = SeqGuid.NewGuid();
            SchemaVersion = 1;

            Happened = happened;
	        TargetVersion = targetVersion;

            ((IUncommitedEvent)this).Target = target;
        }

        protected AggregateEvent(TRoot target)
            : this(target, target != null ? target.Version : -1, SystemTime.Now)
        {
        }
    }
}