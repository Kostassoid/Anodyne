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

namespace Kostassoid.Anodyne.Domain.Specs
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Base;
    using Common.Extentions;
    using Events;
    using NUnit.Framework;
    using Wiring;

    // ReSharper disable InconsistentNaming
    public class SubscriptionRootSpecs
    {
        public class TestRoot : AggregateRoot<Guid>
        {
            public int Fired1 { get; set; }
            public int Fired2 { get; set; }

            private void Apply(Change1Event ev)
            {
                Fired1++;
            }

            private void Apply(Change2Event ev)
            {
                Fired2++;
            }

        }

        public class Change1Event : AggregateEvent<TestRoot, Guid>
        {
            public Change1Event(TestRoot aggregate) : base(aggregate)
            {
            }
        }

        public class Change2Event : AggregateEvent<TestRoot, Guid>
        {
            public Change2Event(TestRoot aggregate) : base(aggregate)
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_root_event_using_extentions
        {
            [Test]
            public void should_call_valid_handler()
            {
                EventRouter.Reset();
                EventRouter.Extentions.BindDomainEvents<TestRoot>();

                var root = new TestRoot();

                EventRouter.Fire(new Change1Event(root));
                EventRouter.Fire(new Change2Event(root));

                Assert.That(root.Fired1, Is.EqualTo(1));
                Assert.That(root.Fired2, Is.EqualTo(1));
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
