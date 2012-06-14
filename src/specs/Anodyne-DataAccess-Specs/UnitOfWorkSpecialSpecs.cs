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

namespace Kostassoid.Anodyne.DataAccess.Specs
{
    using System;

    using Anodyne.Specs.Shared;
    using Common.Tools;
    using Domain.Base;
    using Domain.Events;

    using NUnit.Framework;
    using Policy;

    // ReSharper disable InconsistentNaming
    public class UnitOfWorkSpecialSpecs
    {
        public class UnitOfWorkScenario
        {
            public UnitOfWorkScenario()
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
                Update();
            }

            public void Update()
            {
                Apply(new TestRootUpdated(this));
            }

            protected void OnUpdated(TestRootUpdated @event)
            {

            }


        }

        public class TestRootCreated : AggregateEvent<TestRoot, EmptyEventPayload>
        {
            public TestRootCreated(TestRoot aggregate)
                : base(aggregate, new EmptyEventPayload())
            {
            }
        }

        public class TestRootUpdated : AggregateEvent<TestRoot, EmptyEventPayload>
        {
            public TestRootUpdated(TestRoot aggregate)
                : base(aggregate, new EmptyEventPayload())
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_calling_nested_apply_with_ignored_policy : UnitOfWorkScenario
        {
            [Test]
            public void should_not_throw_and_events_order_should_be_correct()
            {
                Guid rootId;
                using (var uow = new UnitOfWork(StaleDataPolicy.Ignore))
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId);

                    Assert.That(root.IsSome, Is.True);
                    Assert.That(root.Value.Id, Is.EqualTo(rootId));
                    Assert.That(root.Value.Version, Is.EqualTo(2));
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_calling_nested_apply_with_strict_policy : UnitOfWorkScenario
        {
            [Test]
            public void should_not_throw_and_events_order_should_be_correct()
            {
                Guid rootId;
                using (var uow = new UnitOfWork(StaleDataPolicy.Strict))
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId);

                    Assert.That(root.IsSome, Is.True);
                    Assert.That(root.Value.Id, Is.EqualTo(rootId));
                    Assert.That(root.Value.Version, Is.EqualTo(2));
                }
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
