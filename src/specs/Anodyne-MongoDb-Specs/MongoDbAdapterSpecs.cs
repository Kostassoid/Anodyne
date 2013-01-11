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
using Kostassoid.Anodyne.DataAccess.Domain;
using Kostassoid.Anodyne.MongoDb.Specs.Domain;
using MongoDB.Driver;
using NUnit.Framework;

namespace Kostassoid.Anodyne.MongoDb.Specs
{
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
                using (var uow = new UnitOfWork())
                {
                    var collection = (uow.DomainDataSession.DataSession.NativeSession as MongoDatabase).GetCollection<TestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
                }
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_collection_by_derived_root_type : MongoDbScenario
        {
            [Test]
            public void should_return_collection_with_name_as_base_root_name()
            {
                using (var uow = new UnitOfWork())
                {
                    var collection = (uow.DomainDataSession.DataSession.NativeSession as MongoDatabase).GetCollection<DerivedTestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
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
                using (new UnitOfWork())
                {
                    createdRoot = TestRoot.Create("test data");
                }

                using (var uow = new UnitOfWork())
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

                using (var uow = new UnitOfWork())
                {
                    var database = (MongoDatabase)uow.DomainDataSession.DataSession.NativeSession;

                    database.CollectionExists("TestRoot").Should().BeFalse();

                    database.EnsureCappedCollectionExists<TestRoot>(10);

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

                using (new UnitOfWork())
                {
                    TestRoot.Create("boo");
                    TestRoot.Create("hoo");
                }

                using (var uow = new UnitOfWork())
                {
                    var database = (MongoDatabase)uow.DomainDataSession.DataSession.NativeSession;

                    database.EnsureCappedCollectionExists<TestRoot>(10);

                    var stats = database.GetCollection<TestRoot>().GetStats();

                    stats.IsCapped.Should().BeTrue();
                    stats.StorageSize.Should().BeInRange(mb10, mb10 + 8192); //TODO: get exact metadata size?

                    uow.Query<TestRoot>().Count().Should().Be(2);
                }
            }
        }



    }
    // ReSharper restore InconsistentNaming

}
