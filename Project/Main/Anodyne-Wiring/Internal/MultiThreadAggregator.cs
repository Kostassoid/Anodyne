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

namespace Kostassoid.Anodyne.Wiring.Internal
{
    using System;
    using System.Collections.Generic;

    public class MultiThreadAggregator : SingleThreadAggregator
    {
        private readonly object _sync = new object();

        public override Action Subscribe(IInternalEventHandler handler)
        {
            lock (_sync)
            {
                IList<IInternalEventHandler> handlers;
                if (!HandlersDictionary.TryGetValue(handler.EventType, out handlers))
                {
                    var newHandlersDictionary = new Dictionary<Type, IList<IInternalEventHandler>>(HandlersDictionary) {{handler.EventType, new List<IInternalEventHandler> {handler}}};
                    HandlersDictionary = newHandlersDictionary;
                }
                else
                {
                    var newHandlers = new List<IInternalEventHandler>(handlers) {handler};
                    HandlersDictionary[handler.EventType] = newHandlers;
                }
            }

            return GenerateUnsubscriptionAction(handler);
        }


        protected override Action GenerateUnsubscriptionAction(IInternalEventHandler handler)
        {
            return delegate
                       {
                           lock (_sync)
                           {
                               IList<IInternalEventHandler> handlers;
                               if (HandlersDictionary.TryGetValue(handler.EventType, out handlers))
                               {
                                   var newHandlers = new List<IInternalEventHandler>(handlers);
                                   if (newHandlers.Remove(handler))
                                   {
                                       HandlersDictionary[handler.EventType] = newHandlers;
                                   }
                               }
                           }
                       };
        }

        protected override void Dispose()
        {
            lock (_sync)
            {
                HandlersDictionary = new Dictionary<Type, IList<IInternalEventHandler>>();
            }
        }
    }
}