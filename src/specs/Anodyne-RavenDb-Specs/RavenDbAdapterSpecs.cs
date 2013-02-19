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

namespace Kostassoid.Anodyne.RavenDb.Specs
{
    using System.Linq;
    using Anodyne.Domain.DataAccess;
    using Domain;
    using FluentAssertions;
    using NUnit.Framework;
    using System.Collections.Generic;
    using ReadModel;

    // ReSharper disable InconsistentNaming
    public class RavenDbAdapterSpecs
    {
        public class RavenDbScenario
        {
            public RavenDbScenario()
            {
                IntegrationContext.Init();
            }
        }

        [TestFixture]
        [Category("Integration")]
        public class when_getting_saved_root_by_id : RavenDbScenario
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
        public class when_getting_saved_root_by_id_using_data_context : RavenDbScenario
        {
            [TestFixtureSetUp]
            public void SetUp()
            {
                using (var session = IntegrationContext.DataContext.GetSession())
                {
                    session.SaveOne(SimpleQueryRoot.Create("boo"));
                    session.SaveOne(SimpleQueryRoot.Create("foo"));
                    session.SaveOne(SimpleQueryRoot.Create("zoo"));
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
    }
    // ReSharper restore InconsistentNaming

}
