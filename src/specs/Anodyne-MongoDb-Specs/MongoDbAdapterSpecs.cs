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

using FluentAssertions;
using Kostassoid.Anodyne.Domain.DataAccess;
using Kostassoid.Anodyne.MongoDb.Specs.Domain;
using MongoDB.Driver;
using NUnit.Framework;

namespace Kostassoid.Anodyne.MongoDb.Specs
{
    using System.Collections.Generic;
    using System.Linq;
    using Common.Measure;
    using ReadModel;

    // ReSharper disable InconsistentNaming
    public class MongoDbAdapterSpecs
    {
        public class MongoDbScenario
        {
            public MongoDbScenario()
            {
                IntegrationContext.Init();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_collection_by_base_root_type : MongoDbScenario
        {
            [Test]
            public void should_return_collection_with_name_as_type_name()
            {
                using (var uow = UnitOfWork.Start())
                {
                    var collection = (uow.Session.DataSession.NativeSession as MongoDatabase).GetCollection<TestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_collection_by_derived_root_type : MongoDbScenario
        {
/*
            [Test]
            public void should_return_collection_with_name_as_base_root_name()
            {
                using (var uow = UnitOfWork.Start())
                {
                    var collection = (uow.DomainDataSession.DataSession.NativeSession as MongoDatabase).GetCollection<DerivedTestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
                }
            }
*/
            [Test]
            public void should_return_collection_with_name_as_derived_root_name()
            {
                using (var uow = UnitOfWork.Start())
                {
                    var collection = (uow.Session.DataSession.NativeSession as MongoDatabase).GetCollection<DerivedTestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(DerivedTestRoot).Name);
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_saved_root_by_id : MongoDbScenario
        {
            [Test]
            public void should_return_valid_object()
            {
                TestRoot createdRoot;
                using (UnitOfWork.Start())
                {
                    createdRoot = TestRoot.Create("test data");
                }

                using (var uow = UnitOfWork.Start())
                {
                    var foundRoot = uow.Query<TestRoot>().GetOne(createdRoot.Id);

                    foundRoot.Id.Should().Be(createdRoot.Id);
                    foundRoot.Data.Should().Be(createdRoot.Data);
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_creating_capped_collection_from_nothing : MongoDbScenario
        {
            [Test]
            public void should_create_valid_capped_collection()
            {
                const int mb10 = 10 * 1024 * 1024;

                using (var uow = UnitOfWork.Start())
                {
                    var database = (MongoDatabase)uow.Session.DataSession.NativeSession;

                    database.CollectionExists("TestRoot").Should().BeFalse();

                    database.EnsureCappedCollectionExists<TestRoot>(10.Megabytes());

                    database.CollectionExists("TestRoot").Should().BeTrue();

                    var stats = database.GetCollection<TestRoot>().GetStats();

                    stats.IsCapped.Should().BeTrue();
                    stats.StorageSize.Should().BeInRange(mb10, mb10 + 8192); //TODO: get exact metadata size?
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_creating_capped_collection_from_existing_collection : MongoDbScenario
        {
            [Test]
            public void should_convert_collection_to_capped()
            {
                const int mb10 = 10 * 1024 * 1024;

                using (UnitOfWork.Start())
                {
                    TestRoot.Create("boo");
                    TestRoot.Create("hoo");
                }

                using (var uow = UnitOfWork.Start())
                {
                    var database = (MongoDatabase)uow.Session.DataSession.NativeSession;

                    database.EnsureCappedCollectionExists<TestRoot>(10.Megabytes());

                    var stats = database.GetCollection<TestRoot>().GetStats();

                    stats.IsCapped.Should().BeTrue();
                    stats.StorageSize.Should().BeInRange(mb10, mb10 + 8192); //TODO: get exact metadata size?

                    uow.Query<TestRoot>().Count().Should().Be(2);
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_saved_root_by_id_using_data_context : MongoDbScenario
        {
            [TestFixtureSetUp]
            public void SetUp()
            {
                using (var session = IntegrationContext.DataContext.GetSession())
                {
                    session.SaveOne(SimpleQueryRoot.Create("boo"), null);
					session.SaveOne(SimpleQueryRoot.Create("foo"), null);
					session.SaveOne(SimpleQueryRoot.Create("zoo"), null);
                }
            }

            [Test]
            public void should_return_valid_object()
            {
                IList<SimpleQueryRoot> simpleRoots;
                using (var session = IntegrationContext.DataContext.GetSession())
                {
                    simpleRoots = session.Query<SimpleQueryRoot>().ToList();
                }

                simpleRoots.Should().HaveCount(3);
                simpleRoots.Should().Contain(r => r.Data == "boo");
                simpleRoots.Should().Contain(r => r.Data == "foo");
                simpleRoots.Should().Contain(r => r.Data == "zoo");
            }
        }

		[TestFixture]
		[Category("Integration")]
		public class when_updating_roots_using_specific_version : MongoDbScenario
		{
			[Test]
			public void should_fail_if_version_mismatch()
			{
                var root1 = SimpleQueryRoot.Create("boo");
                var root2 = SimpleQueryRoot.Create("foo");

                // emulating some activity
                root2.BumpVersion();
                root1.BumpVersion(); 
                root2.BumpVersion();

				// creating
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					session.SaveOne(root1, 0).Should().BeTrue();
					session.SaveOne(root2, 0).Should().BeTrue();
				}

				// attempting to create again
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					session.SaveOne(root1, 0).Should().BeFalse();
					session.SaveOne(root2, 0).Should().BeFalse();
				}

				// updating
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					root1.BumpVersion();
					root2.BumpVersion();

					session.SaveOne(root1, 1).Should().BeTrue();
					session.SaveOne(root2, 1).Should().BeFalse(); // actual version is 2
				}

				// removing
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					root1.BumpVersion();
					root2.BumpVersion();

                    session.RemoveOne(typeof(SimpleQueryRoot), root1.Id, 3).Should().BeFalse(); // actual version is 2
                    session.RemoveOne(typeof(SimpleQueryRoot), root2.Id, 2).Should().BeTrue();
				}

				// checking
				using (var session = IntegrationContext.DataContext.GetSession())
				{
                    var availableRoots = session.Query<SimpleQueryRoot>().ToList();
					availableRoots.Should().HaveCount(1);
					availableRoots.First().Id.Should().Be(root1.Id);
				}
			}
		}

		[TestFixture]
		[Category("Integration")]
		public class when_updating_roots_using_non_specific_version : MongoDbScenario
		{
			[Test]
			public void should_ignore_version_mismatch()
			{
				var root1 = SimpleQueryRoot.Create("boo");
                var root2 = SimpleQueryRoot.Create("foo");

                // emulating some activity
                root2.BumpVersion();
                root1.BumpVersion();
                root2.BumpVersion();

				// creating
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					session.SaveOne(root1, null).Should().BeTrue();
					session.SaveOne(root2, null).Should().BeTrue();
				}

				// attempting to create again
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					session.SaveOne(root1, null).Should().BeTrue();
					session.SaveOne(root2, null).Should().BeTrue();
				}

				// updating
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					root1.BumpVersion();
					root2.BumpVersion();

					session.SaveOne(root1, null).Should().BeTrue();
					session.SaveOne(root2, null).Should().BeTrue(); 
				}

				// removing
				using (var session = IntegrationContext.DataContext.GetSession())
				{
					root1.BumpVersion();
					root2.BumpVersion();

                    session.RemoveOne(typeof(SimpleQueryRoot), root1.Id, null).Should().BeTrue();
                    session.RemoveOne(typeof(SimpleQueryRoot), root2.Id, null).Should().BeTrue();
				}

				// checking
				using (var session = IntegrationContext.DataContext.GetSession())
				{
                    var availableRoots = session.Query<SimpleQueryRoot>().ToList();
					availableRoots.Should().HaveCount(0);
				}
			}
		}

	}
    // ReSharper restore InconsistentNaming

}
