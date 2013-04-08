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
    using Anodyne.Specs.Shared;
    using Base;
    using DataAccess;
    using DataAccess.Policy;
    using Events;
    using System;
    using Common.Tools;
    using FluentAssertions;
    using NUnit.Framework;

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

        public class TestRootCreated : AggregateEvent<TestRoot>
        {
            public TestRootCreated(TestRoot aggregate)
                : base(aggregate)
            {
            }
        }

        public class TestRootUpdated : AggregateEvent<TestRoot>
        {
            public TestRootUpdated(TestRoot aggregate)
                : base(aggregate)
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_calling_nested_apply_with_ignore_policy : UnitOfWorkScenario
        {
            [Test]
            public void should_not_throw_and_events_order_should_be_correct()
            {
                Guid rootId;
                using (UnitOfWork.Start(StaleDataPolicy.Ignore))
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = UnitOfWork.Start())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId);

                    root.IsSome.Should().BeTrue();
                    root.Value.Id.Should().Be(rootId);
                    root.Value.Version.Should().Be(2);
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
                using (UnitOfWork.Start(StaleDataPolicy.Strict))
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = UnitOfWork.Start())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId);

                    root.IsSome.Should().BeTrue();
                    root.Value.Id.Should().Be(rootId);
                    root.Value.Version.Should().Be(2);
                }
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
