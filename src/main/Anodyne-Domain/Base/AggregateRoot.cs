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

namespace Kostassoid.Anodyne.Domain.Base
{
    using System;
    using Abstractions.DataAccess;
    using Common.Extentions;
    using DataAccess;
    using DataAccess.Exceptions;
    using Events;
    using Wiring;

    [Serializable]
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        object IPersistableRoot.IdObject { get { return Id; } }

		void IAggregateRoot.Apply(IAggregateEvent ev)
		{
			Apply(ev);
		}

	    public static void Apply(IAggregateEvent ev)
        {
			if (ev.TargetVersion != ev.Target.Version)
				throw new ConcurrencyException(ev);

            //TODO: decouple?
            UnitOfWork.Handle(ev);
            ev.Target.BumpVersion();

	        var handler = AggregateEventHandlerResolver.ResolveFor(ev);
			handler(ev.Target, ev);

			if (!ev.IsReplaying)
				EventBus.Publish(ev);
        }
    }
}