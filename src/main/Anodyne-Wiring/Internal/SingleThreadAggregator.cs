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

namespace Kostassoid.Anodyne.Wiring.Internal
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SingleThreadAggregator : IEventAggregator, IDisposable
    {
        protected IDictionary<Type, IList<IInternalEventHandler>> HandlersDictionary =
            new Dictionary<Type, IList<IInternalEventHandler>>();

        //protected IDictionary<Type, IEnumerable<Type>> Targets = new Dictionary<Type, IEnumerable<Type>>();

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Dispose();
        }

        #endregion

        protected virtual void Dispose()
        {
            Reset();
        }

        #region IEventAggregator Members

        public virtual void Publish<TEvent>(TEvent ev) where TEvent : IEvent
        {
            IList<IInternalEventHandler> eventHandlers;
            if (!HandlersDictionary.TryGetValue(ev.GetType(), out eventHandlers))
                return;

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
                HandlersDictionary.Add(handler.EventType, new List<IInternalEventHandler> {handler});
            }
            else
            {
                handlers.Add(handler);
            }

            return GenerateUnsubscriptionAction(handler);
        }

        public virtual void Reset()
        {
            HandlersDictionary.Clear();
        }

        #endregion

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