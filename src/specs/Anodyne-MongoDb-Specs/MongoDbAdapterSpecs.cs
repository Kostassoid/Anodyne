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

using Kostassoid.Anodyne.DataAccess;
using FluentAssertions;
using Kostassoid.Anodyne.MongoDb.Specs.Domain;
using MongoDB.Driver;
using NUnit.Framework;

namespace Kostassoid.Anodyne.MongoDb.Specs
{
    // ReSharper disable InconsistentNaming
    public class MongoDbAdapterSpecs
    {
        public class UnitOfWorkScenario
        {
            public UnitOfWorkScenario()
            {
                IntegrationContext.Init();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_collection_by_base_root_type : UnitOfWorkScenario
        {
            [Test]
            public void should_return_collection_with_name_as_type_name()
            {
                using (var uow = new UnitOfWork())
                {
                    var collection = ((MongoDatabase)uow.DataSession.As<IDataSessionEx>().NativeSession).GetCollection<TestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
                }

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_collection_by_derived_root_type : UnitOfWorkScenario
        {
            [Test]
            public void should_return_collection_with_name_as_base_root_name()
            {
                using (var uow = new UnitOfWork())
                {
                    var collection = ((MongoDatabase)uow.DataSession.As<IDataSessionEx>().NativeSession).GetCollection<DerivedTestRoot>();

                    collection.Should().NotBeNull();
                    collection.Name.Should().Be(typeof(TestRoot).Name);
                }

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_saved_root_by_id : UnitOfWorkScenario
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



    }
    // ReSharper restore InconsistentNaming

}
