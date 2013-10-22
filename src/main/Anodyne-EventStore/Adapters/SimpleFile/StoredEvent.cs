﻿// Copyright 2011-2013 Anodyne.
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

namespace Kostassoid.Anodyne.EventStore.Adapters.SimpleFile
{
    using System;
    using Domain.Events;

    [Serializable]
    public class StoredEvent
    {
        public Guid Id { get; private set; }
        public string TargetType { get; private set; }
        public object TargetId { get; private set; }
        public long TargetVersion { get; private set; }
        public object Raw { get; private set; }

        protected StoredEvent()
        {}

        public StoredEvent(IUncommitedEvent ev)
        {
            Raw = ev;

            Id = ev.Id;
            TargetType = ev.Target.GetType().Name;
            TargetId = ev.Target.IdObject;
            TargetVersion = ev.TargetVersion;
        }
    }
}