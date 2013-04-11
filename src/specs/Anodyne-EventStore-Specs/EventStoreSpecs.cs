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
            }

            public void Update()
            {
                Apply(new TestRootUpdated(this));
            }

            protected void OnUpdated(TestRootUpdated @event)
            {

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
            public TestRootUpdated(TestRoot target)
                : base(target)
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        [Ignore("Playground mode")]
        public class when_capturing_events : EventStoreScenario
        {
            readonly private EventStoreObserver _eventStore = new EventStoreObserver(new SimpleFileEventStoreAdapter("test.log"));

            [Test]
            public void should_store_all_applied_events()
            {
                _eventStore.Start();

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
            }

            [TearDown]
            void TearDown()
            {
                _eventStore.Stop();
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
