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
    using System.ComponentModel;
    using System.Threading.Tasks;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public interface IInternalEventHandler
    {
        Action<object> HandlerAction { get; }
        Type EventType { get; }
        int Priority { get; }
        bool Async { get; }
    }

    internal class InternalEventHandler<TEvent> : IInternalEventHandler where TEvent : class
    {
        private readonly Type _eventType;
        private readonly Action<TEvent> _handlerAction;
        private readonly Predicate<TEvent> _predicate;
        private readonly int _priority;
        private readonly bool _async;

        public InternalEventHandler(Type eventType, Action<TEvent> handlerAction, Predicate<TEvent> predicate, int priority, bool async)
        {
            _eventType = eventType;
            _handlerAction = handlerAction;
            _predicate = predicate;
            _priority = priority;
            _async = async;
        }

        public InternalEventHandler(Action<TEvent> handlerAction, Predicate<TEvent> predicate, int priority, bool async) : this(typeof (TEvent), handlerAction, predicate, priority, async)
        {
        }

        #region IInternalEventHandler Members

        int IInternalEventHandler.Priority
        {
            get { return _priority; }
        }

        bool IInternalEventHandler.Async
        {
            get { return _async; }
        }

        Action<object> IInternalEventHandler.HandlerAction
        {
            get
            {
                return e =>
                           {
                               var ev = e as TEvent;
                               if (ev != null && _predicate(ev))
                               {
                                   if (_async)
                                       Task.Factory.StartNew(() => _handlerAction(ev));
                                   else
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