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

namespace Kostassoid.Anodyne.EventStore.Specs
{
    using System.IO;
    using System.Linq;
    using Adapters;
    using Adapters.SimpleFile;
    using Anodyne.Specs.Shared;
    using Domain.Base;
    using Domain.DataAccess;
    using Domain.Events;
    using System;
    using Common.Tools;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class EventStoreSpecs
    {
        public class EventStoreScenario
        {
            public EventStoreScenario()
            {
                IntegrationContext.Init();
            }
        }

        [Serializable]
        public class TestRoot : AggregateRoot<Guid>
        {
            public string Value { get; private set; }

            protected TestRoot()
            {
                Id = SeqGuid.NewGuid();
            }

            public static TestRoot Create()
            {
                var root = new TestRoot();
                Apply(new TestRootCreated(root));
                return root;
            }

            protected void OnCreated(TestRootCreated @event)
            {
                Value = "created";
            }

            public void Update()
            {
                Apply(new TestRootUpdated(this, Value + "-boo"));
            }

            protected void OnUpdated(TestRootUpdated @event)
            {
                Value = @event.Payload;
            }
        }

        public class TestRootCreated : AggregateEvent<TestRoot>
        {
            public TestRootCreated(TestRoot target)
                : base(target)
            {
            }
        }

        public class TestRootUpdated : AggregateEvent<TestRoot>
        {
            public string Payload { get; private set; }

            public TestRootUpdated(TestRoot target, string payload)
                : base(target)
            {
                Payload = payload;
            }
        }

        [TestFixture]
        [Category("Unit")]
        [Ignore("Playground mode")]
        public class when_capturing_events : EventStoreScenario
        {
            private readonly IEventStoreAdapter _adapter = new SimpleFileEventStoreAdapter("test.log");
            private readonly EventStoreObserver _eventStore;

            private Guid _root1Id;
            private Guid _root2Id;

            public when_capturing_events()
            {
                _eventStore = new EventStoreObserver(_adapter);
            }

            [Test]
            public void should_store_all_applied_events()
            {
                var root1Events = _adapter.LoadFor<TestRoot>(_root1Id).ToList();
                root1Events.Count().Should().Be(3);
                root1Events.Skip(0).First().Should().BeOfType<TestRootCreated>();
                root1Events.Skip(0).First().TargetVersion.Should().Be(0);
                root1Events.Skip(1).First().Should().BeOfType<TestRootUpdated>();
                root1Events.Skip(1).First().TargetVersion.Should().Be(1);
                root1Events.Skip(2).First().Should().BeOfType<TestRootUpdated>();
                root1Events.Skip(2).First().TargetVersion.Should().Be(2);
            }

            [SetUp]
            public void SetUp()
            {
                File.Delete("test.log");

                TestRoot root1;
                TestRoot root2;
                using (UnitOfWork.Start())
                {
                    root1 = TestRoot.Create();
                    root2 = TestRoot.Create();
                    root1.Update();
                }

                using (var uow = UnitOfWork.Start())
                {
                    var foundRoot1 = uow.Query<TestRoot>().FindOne(root1.Id);
                    var foundRoot2 = uow.Query<TestRoot>().FindOne(root2.Id);
                    foundRoot2.Value.Update();
                    foundRoot2.Value.Update();
                    foundRoot2.Value.Update();
                    foundRoot1.Value.Update();
                }

                _root1Id = root1.Id;
                _root2Id = root2.Id;
            }

            [TestFixtureSetUp]
            public void FixtureSetUp()
            {
                _eventStore.Start();
            }

            [TestFixtureTearDown]
            public void FixtureTearDown()
            {
                _eventStore.Stop();
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
