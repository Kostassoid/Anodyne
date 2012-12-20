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

using System;
using Kostassoid.Anodyne.Common.Tools;
using Kostassoid.Anodyne.Domain.Base;
using Kostassoid.Anodyne.MongoDb.Specs.Domain.Events;

namespace Kostassoid.Anodyne.MongoDb.Specs.Domain
{
    [Serializable]
    public class TestRoot : AggregateRoot<Guid>
    {
        public string Data { get; protected set; }

        protected TestRoot()
        {
            Id = SeqGuid.NewGuid();
        }

        public static TestRoot Create(string data)
        {
            var root = new TestRoot();
            Apply(new TestRootCreated(root, data));
            return root;
        }

        protected void OnCreated(TestRootCreated @event)
        {
            Data = @event.Data;
        }

        virtual public void Update()
        {
            Apply(new TestRootUpdated(this));
        }

        protected void OnUpdated(TestRootUpdated @event)
        {

        }


    }
}