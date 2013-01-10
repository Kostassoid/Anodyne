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

namespace Kostassoid.Anodyne.Wiring.Syntax
{
    using System;
    using Common;

    public interface ITargetSyntax<out TEvent> : IAsyncTargetSyntax<TEvent>, ISyntax where TEvent : class, IEvent
    {
        Action With(IHandlerOf<TEvent> handler, Priority priority = null);
        Action With(Action<TEvent> action, Priority priority = null);
        ITargetDiscoverySyntax<TEvent, THandler> With<THandler>(EventMatching eventMatching, Priority priority = null) where THandler : class;
        ITargetDiscoveryByTypeSyntax<TEvent> With(Type handlerType, EventMatching eventMatching, Priority priority = null);
    }
}