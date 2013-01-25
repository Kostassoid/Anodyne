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
    using System.Linq;
    using System.Threading.Tasks;
    using Common.Extentions;
    using Common.Tools;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class UnitOfWorkBasicSpecs
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
                Id = @event.Target.Id;
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
        public class when_working_with_root_from_one_unit_of_work : UnitOfWorkScenario
        {
            [Test]
            public void should_save_root_and_update_version()
            {
                Guid rootId;
                using (new UnitOfWork())
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId);

                    root.IsSome.Should().BeTrue();
                    root.Value.Id.Should().Be(rootId);
                    root.Value.Version.Should().Be(1);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_several_roots_within_one_unit_of_work : UnitOfWorkScenario
        {
            [Test]
            public void should_update_all_roots()
            {
                TestRoot root1;
                TestRoot root2;
                using (new UnitOfWork())
                {
                    root1 = TestRoot.Create();
                    root2 = TestRoot.Create();
                    root1.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var foundRoot1 = uow.Query<TestRoot>().FindOne(root1.Id);
                    var foundRoot2 = uow.Query<TestRoot>().FindOne(root2.Id);
                    foundRoot2.Value.Update();
                    foundRoot2.Value.Update();
                    foundRoot2.Value.Update();
                    foundRoot1.Value.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var foundRoot1 = uow.Query<TestRoot>().FindOne(root1.Id);
                    var foundRoot2 = uow.Query<TestRoot>().FindOne(root2.Id);

                    foundRoot1.Value.Id.Should().Be(root1.Id);
                    foundRoot2.Value.Id.Should().Be(root2.Id);
                    foundRoot1.Value.Version.Should().Be(3);
                    foundRoot2.Value.Version.Should().Be(4);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_existing_root : UnitOfWorkScenario
        {
            [Test]
            public void should_update_root_version()
            {
                Guid rootId;
                using (new UnitOfWork())
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(rootId).Value;
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var updatedRoot = uow.Query<TestRoot>().FindOne(rootId).Value;
                    updatedRoot.Version.Should().Be(2);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_root_several_times_in_one_uow : UnitOfWorkScenario
        {
            [Test]
            public void should_update_root_version()
            {
                TestRoot originalRoot;
                using (new UnitOfWork())
                {
                    originalRoot = TestRoot.Create();
                    originalRoot.Update();
                }

                originalRoot.Version.Should().Be(2);

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(originalRoot.Id).Value;
                    root.Update();
                    root.Update();
                    root.Update();
                    root.Version.Should().Be(5);
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindOne(originalRoot.Id).Value;
                    root.Version.Should().Be(5);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_root_and_there_is_a_newer_version : UnitOfWorkScenario
        {
            [Test]
            [ExpectedException(typeof(StaleDataException))]
            public void should_throw_stale_data_exception()
            {
                TestRoot root;
                using (new UnitOfWork())
                {
                    root = TestRoot.Create();
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var anotherRoot = uow.Query<TestRoot>().FindOne(root.Id).Value;
                    anotherRoot.Update();
                }

                using (new UnitOfWork())
                {
                    root.Update();
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_root_and_there_is_a_newer_version_with_ignore_policy : UnitOfWorkScenario
        {
            [Test]
            public void should_overwrite_conflicting_roots()
            {
                TestRoot root;
                using (new UnitOfWork())
                {
                    root = TestRoot.Create();
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var anotherRoot = uow.Query<TestRoot>().FindOne(root.Id).Value;
                    anotherRoot.Update();
                    anotherRoot.Update(); // should be version 4 at this point
                }

                using (new UnitOfWork(StaleDataPolicy.Ignore))
                {
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    root = uow.Query<TestRoot>().FindOne(root.Id).Value;
                    root.Version.Should().Be(3);
                }

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_root_and_there_is_a_newer_version_with_skip_policy : UnitOfWorkScenario
        {
            [Test]
            public void should_skip_conflicting_roots()
            {
                TestRoot root;
                using (new UnitOfWork())
                {
                    root = TestRoot.Create();
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    var anotherRoot = uow.Query<TestRoot>().FindOne(root.Id).Value;
                    anotherRoot.Update();
                    anotherRoot.Update();
                    anotherRoot.Version.Should().Be(4);
                }

                using (new UnitOfWork(StaleDataPolicy.SilentlySkip))
                {
                    root.Update();
                }

                using (var uow = new UnitOfWork())
                {
                    root = uow.Query<TestRoot>().FindOne(root.Id).Value;
                    root.Version.Should().Be(4);
                }

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_working_with_different_objects_of_the_same_root_from_single_unit_of_work : UnitOfWorkScenario
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
                    var root = uow.Query<TestRoot>().FindOne(rootId).Value;
                    root.Update();
                    var anotherRoot = uow.Query<TestRoot>().FindOne(rootId).Value;
                    anotherRoot.Invoking(r => r.Update()).ShouldThrow<ConcurrencyException>();
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_updating_root_from_nested_unit_of_work : UnitOfWorkScenario
        {
            [Test]
            public void should_update_root_version_and_correctly_dispose_unit_of_work()
            {
                Guid rootId;
                using (new UnitOfWork())
                {
                    rootId = TestRoot.Create().Id;
                }

                using (new UnitOfWork())
                {
                    using (var nestedUow = new UnitOfWork())
                    {
                        var root = nestedUow.Query<TestRoot>().FindOne(rootId).Value;
                        root.Update();
                    }
                }

                using (new UnitOfWork())
                {
                    using (var nestedUow = new UnitOfWork())
                    {
                        var root = nestedUow.Query<TestRoot>().FindOne(rootId).Value;
                        root.Update();
                    }
                }

                using (var uow = new UnitOfWork())
                {
                    var updatedRoot = uow.Query<TestRoot>().FindOne(rootId).Value;
                    updatedRoot.Version.Should().Be(3);
                }

                UnitOfWork.Current.IsNone.Should().BeTrue();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_save_many_roots_at_the_same_time : UnitOfWorkScenario
        {
            [Test]
            [MaxTime(1000)]
            public void should_save_all_roots_without_fail()
            {
                const int testingThreadsCount = 100;

                var tasks = Enumerable.Range(0, testingThreadsCount)
                    .Select(_ => Task.Factory.StartNew(() => { using (new UnitOfWork()) { TestRoot.Create();} }))
                    .ToArray();

                Task.WaitAll(tasks);

                using (var uow = new UnitOfWork())
                {
                    uow.Query<TestRoot>().Count().Should().Be(testingThreadsCount);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_update_root_from_one_thread_with_ignore_stale_policy : UnitOfWorkScenario
        {
            [Test]
            [MaxTime(1000)]
            public void should_not_fail()
            {
                const int testingCount = 100;

                Guid id;

                using (new UnitOfWork())
                {
                    id = TestRoot.Create().Id;
                }

                Enumerable.Range(0, testingCount).ForEach(_ =>
                {
                    using (var uow = new UnitOfWork(StaleDataPolicy.Ignore))
                    {
                        uow.Query<TestRoot>().GetOne(id).Update();
                    }
                });

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().GetOne(id);
                    root.Version.Should().Be(testingCount + 1);
                }
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_update_root_from_many_threads_with_ignore_stale_policy : UnitOfWorkScenario
        {
            [Test]
            [MaxTime(1000)]
            public void should_not_fail()
            {
                const int testingThreadsCount = 100;

                Guid id;

                using (new UnitOfWork())
                {
                    id = TestRoot.Create().Id;
                }

                var tasks = Enumerable.Range(0, testingThreadsCount)
                    .Select(_ => Task.Factory.StartNew(() =>
                        {
                            using (var uow = new UnitOfWork(StaleDataPolicy.Ignore)) { uow.Query<TestRoot>().GetOne(id).Update(); }
                        }))
                    .ToArray();

                Task.WaitAll(tasks);

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().GetOne(id);
                    root.Version.Should().BeLessOrEqualTo(testingThreadsCount + 1);
                }
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
