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
using System.ComponentModel;

namespace Kostassoid.Anodyne.Wiring.Internal
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInternalEventHandler
    {
        Action<object> HandlerAction { get; }
        Type EventType { get; }
        int Priority { get; }
        bool Suitable(object ev);
    }

    internal class InternalEventHandler<TEvent> : IInternalEventHandler where TEvent : class
    {
        private readonly Type _eventType;
        private readonly Action<TEvent> _handlerAction;
        private readonly Predicate<TEvent> _predicate;
        private readonly int _priority;

        public InternalEventHandler(Action<TEvent> handlerAction, Predicate<TEvent> predicate, int priority)
        {
            _eventType = typeof (TEvent);
            _handlerAction = handlerAction;
            _predicate = predicate;
            _priority = priority;
        }

        #region IInternalEventHandler Members

        int IInternalEventHandler.Priority
        {
            get { return _priority; }
        }

        bool IInternalEventHandler.Suitable(object ev)
        {
            var typedEv = ev as TEvent;
            return typedEv != null && _predicate(typedEv);
        }

        Action<object> IInternalEventHandler.HandlerAction
        {
            get
            {
                return e =>
                           {
                               var ev = e as TEvent;
                               if (ev != null)
                               {
                                   _handlerAction(ev);
                               }
                           };
            }
        }

        Type IInternalEventHandler.EventType
        {
            get { return _eventType; }
        }

        #endregion
    }
}