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
    using DataAccess.Exceptions;
    using DataAccess.Policy;
    using Events;
    using System;
    using Common.Tools;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class UnitOfWorkNestedSpecs
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
		public class when_querying_and_updaing_same_root_within_nested_unit_of_work : UnitOfWorkScenario
		{
			[Test]
			public void should_throw_concurrency_exception()
			{
				Guid rootId;
				using (new UnitOfWork())
				{
					rootId = TestRoot.Create().Id;
				}

				using (var uow = new UnitOfWork())
				{
					var root = uow.Query<TestRoot>().FindOne(rootId);

					root.Value.Update();

					using (var uow2 = new UnitOfWork())
					{
						var sameRoot = uow2.Query<TestRoot>().FindOne(rootId);

						sameRoot.Value.Invoking(r => r.Update()).ShouldThrow<ConcurrencyException>();
					}
				}
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_querying_and_updaing_different_roots_within_nested_unit_of_work : UnitOfWorkScenario
		{
			[Test]
			public void should_update_both_roots_without_exception()
			{
				Guid root1Id;
				Guid root2Id;
				using (new UnitOfWork())
				{
					root1Id = TestRoot.Create().Id;
					root2Id = TestRoot.Create().Id;
				}

				using (var uow = new UnitOfWork())
				{
					var root = uow.Query<TestRoot>().FindOne(root1Id);

					root.Value.Update();

					using (var uow2 = new UnitOfWork())
					{
						var anotherRoot = uow2.Query<TestRoot>().FindOne(root2Id);

						anotherRoot.Value.Invoking(r => r.Update()).ShouldNotThrow<ConcurrencyException>();
					}

					root.Value.Update();
				}

				using (var uow = new UnitOfWork())
				{
					var root1 = uow.Query<TestRoot>().FindOne(root1Id);
					var root2 = uow.Query<TestRoot>().FindOne(root2Id);

					root1.Value.Version.Should().Be(3);
					root2.Value.Version.Should().Be(2);
				}
			}
		}
	}
    // ReSharper restore InconsistentNaming

}
