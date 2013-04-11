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
    using System.Linq;
    using Common.Extentions;
    using Common.Reflection;
    using Common.Tools;

    public static class AggregateEventHandlerResolver
    {
	    private static readonly Func<Type, Type, TypeEx.HandlerDelegate> HandlerResolver
		    = MemoizedFunc.From((Type aggregateType, Type eventType) =>
			    {
				    var suitableMethods = aggregateType.FindMethodHandlers(eventType, false).ToList();
					if (suitableMethods.Count == 0)
					{
						throw new InvalidOperationException("No suitable handlers in {0} for {1}".FormatWith(aggregateType.Name, eventType.Name));
					}

					if (suitableMethods.Count > 1)
					{
						throw new InvalidOperationException("Too many suitable handlers in {0} for {1}. Expected exactly one.".FormatWith(aggregateType.Name, eventType.Name));
					}

				    return TypeEx.BuildMethodHandler(suitableMethods.First(), eventType);
			    });

		public static TypeEx.HandlerDelegate ResolveFor(IAggregateEvent ev)
		{
			return HandlerResolver(ev.Target.GetType(), ev.GetType());
		}

    }
}