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
        public class when_updating_root_from_nested_unit_of_work : UnitOfWorkScenario
        {
            private Guid rootId;
            private bool completedEventFired;
            private bool cancelledEventFired;

            [TestFixtureSetUp]
            public void SetUp()
            {
                using (UnitOfWork.Start())
                {
                    rootId = TestRoot.Create().Id;
                }

                using (UnitOfWork.Start())
                {
                    using (var nestedUow = UnitOfWork.Start())
                    {
                        nestedUow.Completed += () => { completedEventFired = true; };
                        nestedUow.Cancelled += () => { cancelledEventFired = true; };

                        var root = nestedUow.Query<TestRoot>().GetOne(rootId);
                        root.Update();
                    }
                }
            }

            [Test]
            public void should_update_root_version()
            {
                using (var uow = UnitOfWork.Start())
                {
                    var updatedRoot = uow.Query<TestRoot>().GetOne(rootId);
                    updatedRoot.Version.Should().Be(2);
                }
            }

            [Test]
            public void should_correctly_dispose_unit_of_work()
            {
                UnitOfWork.Head.IsNone.Should().BeTrue();
            }

            [Test]
            public void should_not_fire_cancelled_event()
            {
                cancelledEventFired.Should().BeFalse();
            }

            [Test]
            public void should_fire_completed_event()
            {
                completedEventFired.Should().BeTrue();
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
				using (UnitOfWork.Start())
				{
					rootId = TestRoot.Create().Id;
				}

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().FindOne(rootId);

					root.Value.Update();

					using (var uow2 = UnitOfWork.Start())
					{
						var sameRoot = uow2.Query<TestRoot>().GetOne(rootId);

						sameRoot.Invoking(r => r.Update()).ShouldThrow<ConcurrencyException>();
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
				using (UnitOfWork.Start())
				{
					root1Id = TestRoot.Create().Id;
					root2Id = TestRoot.Create().Id;
				}

				using (var uow = UnitOfWork.Start())
				{
					var root = uow.Query<TestRoot>().FindOne(root1Id);

					root.Value.Update();

					using (var uow2 = UnitOfWork.Start())
					{
						var anotherRoot = uow2.Query<TestRoot>().FindOne(root2Id);

						anotherRoot.Value.Invoking(r => r.Update()).ShouldNotThrow<ConcurrencyException>();
					}

					root.Value.Update();
				}

				using (var uow = UnitOfWork.Start())
				{
					var root1 = uow.Query<TestRoot>().GetOne(root1Id);
					var root2 = uow.Query<TestRoot>().GetOne(root2Id);

					root1.Version.Should().Be(3);
					root2.Version.Should().Be(2);
				}
			}
		}

        [TestFixture]
        [Category("Unit")]
        public class when_mutating_root_after_nested_unit_of_work_was_cancelled : UnitOfWorkScenario
        {
            [Test]
            public void should_throw_invalid_operation_exception()
            {
                using (UnitOfWork.Start())
                {
                    var root = TestRoot.Create();

                    root.Update();

                    using (var nestedUow = UnitOfWork.Start())
                    {
                        nestedUow.Cancel();
                    }

                    root.Invoking(r => r.Update()).ShouldThrow<InvalidOperationException>();
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_cancelling_nested_unit_of_work : UnitOfWorkScenario
        {
            [Test]
            public void all_changes_should_be_lost()
            {
                Guid root1Id;
                Guid root2Id;
                using (UnitOfWork.Start())
                {
                    root1Id = TestRoot.Create().Id;
                    root2Id = TestRoot.Create().Id;
                }

                using (var uow = UnitOfWork.Start())
                {
                    var root1 = uow.Query<TestRoot>().GetOne(root1Id);
                    root1.Update();

                    using (var nestedUow1 = UnitOfWork.Start())
                    {
                        var root2 = nestedUow1.Query<TestRoot>().GetOne(root2Id);
                        root2.Update();

                        using (var nestedUow2 = UnitOfWork.Start())
                        {
                            nestedUow2.Cancel();
                        }
                    }
                }

                using (var uow = UnitOfWork.Start())
                {
                    var root1 = uow.Query<TestRoot>().GetOne(root1Id);
                    var root2 = uow.Query<TestRoot>().GetOne(root2Id);

                    root1.Version.Should().Be(1);
                    root2.Version.Should().Be(1);
                }

            }
        }


	}
    // ReSharper restore InconsistentNaming

}
