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
	using System.Linq;
	using Anodyne.Specs.Shared;
    using Base;
    using DataAccess;
	using DataAccess.Exceptions;
	using DataAccess.RootOperation;
    using Events;
    using System;
	using Common.Tools;
    using FluentAssertions;
    using NUnit.Framework;

	// ReSharper disable InconsistentNaming
    public class RootOperationSpecs
    {
        public class RootOperationScenario
        {
            public RootOperationScenario()
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

			public static TestRoot Create(string value)
			{
				var root = new TestRoot();
				Apply(new TestRootCreated(root, value));
				return root;
			}

			public static TestRoot Create()
			{
				return Create(null);
			}

			protected void OnCreated(TestRootCreated @event)
            {
	            Value = @event.Value;
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
	        public string Value { get; private set; }

	        public TestRootCreated(TestRoot target, string value)
                : base(target)
            {
	            Value = value;
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
		public class when_performing_on_factory_created_root : RootOperationScenario
		{
			[Test]
			public void should_create_and_provide_root_in_the_same_unit_of_work()
			{
				var rootId = Guid.Empty;

				OnRoot<TestRoot>.ConstructedBy(TestRoot.Create).Perform((root, ctx) =>
				{
					rootId = root.Id;

					ctx.UnitOfWork.Query<TestRoot>().FindOne(rootId).IsSome.Should().BeFalse();

					root.Update();
				});

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().GetOne(rootId);
					root.Version.Should().Be(2);
				}
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_present_root : RootOperationScenario
		{
			[Test]
			public void should_perform_on_actual_root()
			{
				Guid rootId;

				using (UnitOfWork.Start())
				{
					rootId = TestRoot.Create().Id;
				}

				OnRoot<TestRoot>.IdentifiedBy(rootId).Perform((root, ctx) =>
				{
					root.Id.Should().Be(rootId);
					root.Update();
				});

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().GetOne(rootId);
					root.Version.Should().Be(2);
				}
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_root_using_simplified_delegate_contract : RootOperationScenario
		{
			[Test]
			public void should_work_identical_to_extended_version()
			{
				var rootId = Guid.Empty;

				OnRoot<TestRoot>.ConstructedBy(TestRoot.Create).Perform(root =>
				{
					rootId = root.Id;
					root.Update();
				});

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().GetOne(rootId);
					root.Version.Should().Be(2);
				}
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_non_present_root : RootOperationScenario
		{
			[Test]
			public void should_not_throw()
			{
				Action invalidAction = () => OnRoot<TestRoot>.IdentifiedBy(Guid.NewGuid()).Perform((root, ctx) => { });

				invalidAction.ShouldNotThrow();
			}
		}

/*		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_non_present_root_with_missing_action_set : RootOperationScenario
		{
			[Test]
			public void should_perform_missing_action_only()
			{
				var result = "";
				OnRoot<TestRoot>
					.IdentifiedBy(Guid.NewGuid())
					.IfAbsent(() => result += "missed")
					.Perform(_ => result += "ok");

				result.Should().Be("missed");
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_present_root_with_missing_action_set : RootOperationScenario
		{
			[Test]
			public void should_perform_missing_action_only()
			{
				var result = "";
				OnRoot<TestRoot>
					.ConstructedBy(TestRoot.Create)
					.IfAbsent(() => result += "missed")
					.Perform(_ => result += "ok");

				result.Should().Be("ok");
			}
		}*/

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_null_identified_root : RootOperationScenario
		{
			[Test]
			public void should_throw()
			{
				Action invalidAction = () => OnRoot<TestRoot>.IdentifiedBy(null).Perform((root, ctx) => { });

				invalidAction.ShouldThrow<ArgumentNullException>();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_querying_on_root : RootOperationScenario
		{
			[Test]
			public void should_perform_updates_and_return_value()
			{
				Guid rootId;

				using (UnitOfWork.Start())
				{
					rootId = TestRoot.Create().Id;
				}

				var result =
				OnRoot<TestRoot>.IdentifiedBy(rootId).Request(root =>
				{
					root.Id.Should().Be(rootId);
					root.Update();
					return root.Version;
				});

				result.Should().Be(2);

				result =
				OnRoot<TestRoot>.IdentifiedBy(rootId).Request((root, ctx) =>
				{
					root.Id.Should().Be(rootId);
					root.Update();
					return root.Version;
				});

				result.Should().Be(3);

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().GetOne(rootId);
					root.Version.Should().Be(3);
				}
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_performing_on_present_acquired_root : RootOperationScenario
		{
			[Test]
			public void should_perform_on_actual_root()
			{
				Guid root1Id;
				Guid root2Id;
				Guid root3Id;

				using (UnitOfWork.Start())
				{
					root1Id = TestRoot.Create("aaa").Id;
					root2Id = TestRoot.Create("bbb").Id;
					root3Id = TestRoot.Create("ccc").Id;
				}

				OnRoot<TestRoot>.AcquiredBy(q => q.First(t => t.Value == "bbb"))
					.Perform(root => root.Update());

				using (var uow = UnitOfWork.Start())
				{
					var root1 = uow.Query<TestRoot>().GetOne(root1Id);
					var root2 = uow.Query<TestRoot>().GetOne(root2Id);
					var root3 = uow.Query<TestRoot>().GetOne(root3Id);

					root1.Version.Should().Be(1);
					root2.Version.Should().Be(2);
					root3.Version.Should().Be(1);
				}
			}
		}




    }
    // ReSharper restore InconsistentNaming

}
