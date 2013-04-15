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

namespace Kostassoid.Anodyne.Domain.Specs
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using Base;
    using Common.Extentions;
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

            public static TestRoot Create()
            {
                var root = new TestRoot();
                Apply(new Change1Event(root));
                return root;
            }

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
            public Change1Event(TestRoot target) : base(target)
            {
            }
        }

        public class Change2Event : AggregateEvent<TestRoot>
        {
            public Change2Event(TestRoot target) : base(target)
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

                (root1 as IAggregateRoot).Apply(new Change1Event(root1));
				(root2 as IAggregateRoot).Apply(new Change2Event(root2));
				(root1 as IAggregateRoot).Apply(new Change2Event(root1));
				(root1 as IAggregateRoot).Apply(new Change2Event(root1));

                root1.Fired1.Should().Be(1);
                root1.Fired2.Should().Be(2);
                root2.Fired1.Should().Be(0);
                root2.Fired2.Should().Be(1);

                root1.Version.Should().Be(3);
                root2.Version.Should().Be(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class creating_aggregate_roots
        {
            [Test]
            [MaxTime(1000)]
            [Explicit("For performance tuning")]
            public void should_be_fast()
            {
                const int tasksCount = 1000000;

                TestRoot.Create(); // cold-start

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Enumerable.Range(0, tasksCount).ForEach(_ => TestRoot.Create());

                stopwatch.Stop();

                stopwatch.Elapsed.TotalMilliseconds.Should().BeLessThan(10);
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
