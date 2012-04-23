// Copyright 2009-2011 Taijutsu.
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

using System;
using System.Collections.Generic;
using System.Linq;

namespace Kostassoid.Anodyne.Wiring.Internal
{
    public class SingleThreadAggregator : IEventAggregator, IDisposable
    {
        protected IDictionary<Type, IList<IInternalEventHandler>> HandlersDictionary =
            new Dictionary<Type, IList<IInternalEventHandler>>();

        protected IDictionary<Type, IEnumerable<Type>> Targets = new Dictionary<Type, IEnumerable<Type>>();

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion


        protected virtual void Dispose()
        {
            HandlersDictionary.Clear();
        }

        #region IEventAggregator Members

        public virtual void Publish<TEvent>(TEvent ev) where TEvent : IEvent
        {
            var eventHandlers = new List<IInternalEventHandler>();

            foreach (var targetType in PotentialSubscribers(ev.GetType()))
            {
                IList<IInternalEventHandler> localHandlers;
                if (HandlersDictionary.TryGetValue(targetType, out localHandlers))
                {
                    eventHandlers.AddRange(localHandlers);
                }
            }

            foreach (var handler in eventHandlers.OrderByDescending(h => h.Priority))
            {
                handler.HandlerAction(ev);
            }
        }

        public virtual Action Subscribe(IInternalEventHandler handler)
        {
            IList<IInternalEventHandler> handlers;
            if (!HandlersDictionary.TryGetValue(handler.EventType, out handlers))
            {
                HandlersDictionary.Add(handler.EventType, new List<IInternalEventHandler> { handler });
            }
            else
            {
                handlers.Add(handler);
            }

            return GenerateUnsubscriptionAction(handler);
        }

        #endregion

        protected virtual IEnumerable<Type> PotentialSubscribers(Type type)
        {
            //possible covariance interface generating should be also added
            IEnumerable<Type> targetsForType;
            if (!Targets.TryGetValue(type, out targetsForType))
            {
                targetsForType =
                    type.GetInterfaces().Where(i => typeof (IEvent).IsAssignableFrom(i)).Union(
                        EventTypeHierarchy(type).Reverse()).ToArray();
                CachePotentialSubscribers(type, targetsForType);
            }
            return targetsForType;
        }

        protected virtual IEnumerable<Type> EventTypeHierarchy(Type type)
        {
            if (typeof (IEvent).IsAssignableFrom(type))
            {
                yield return type;
                foreach (var subtype in EventTypeHierarchy(type.BaseType))
                {
                    yield return subtype;
                }
            }
        }

        protected virtual void CachePotentialSubscribers(Type type, IEnumerable<Type> potentialSubscribers)
        {
            Targets[type] = potentialSubscribers;
        }

        protected virtual Action GenerateUnsubscriptionAction(IInternalEventHandler handler)
        {
            return delegate
                       {
                           IList<IInternalEventHandler> handlers;
                           if (HandlersDictionary.TryGetValue(handler.EventType, out handlers))
                           {
                               handlers.Remove(handler);
                           }
                       };
        }
    }
}