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
    using Base;
    using Events;
    using FluentAssertions;
    using NUnit.Framework;
    using Wiring;

    // ReSharper disable InconsistentNaming
    public class AggregateRootSpecs
    {
        public class TestRoot : AggregateRoot<Guid>
        {
            public int Fired1 { get; set; }
            public int Fired2 { get; set; }

            private void Handle(Change1Event ev)
            {
                Fired1++;
            }

            private void Handle(Change2Event ev)
            {
                Fired2++;
            }

        }

        public class Change1Event : AggregateEvent<TestRoot>
        {
            public Change1Event(TestRoot aggregate) : base(aggregate)
            {
            }
        }

        public class Change2Event : AggregateEvent<TestRoot>
        {
            public Change2Event(TestRoot aggregate) : base(aggregate)
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_root_event
        {
            [Test]
            public void should_call_valid_root_handler()
            {
                EventBus.Reset();

                var root1 = new TestRoot();
                var root2 = new TestRoot();

                EventBus.Publish(new Change1Event(root1));
                EventBus.Publish(new Change2Event(root2));
                EventBus.Publish(new Change2Event(root1));
                EventBus.Publish(new Change2Event(root1));

                root1.Fired1.Should().Be(1);
                root1.Fired2.Should().Be(2);
                root2.Fired1.Should().Be(0);
                root2.Fired2.Should().Be(1);

                root1.Version.Should().Be(3);
                root2.Version.Should().Be(1);
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
