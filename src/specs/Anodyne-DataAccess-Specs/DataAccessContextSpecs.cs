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

namespace Kostassoid.Anodyne.DataAccess.Specs
{
    using Abstractions.DataAccess;
    using FluentAssertions;
    using NUnit.Framework;
    using FakeItEasy;
    using Abstractions.DataAccess.Internal;


    // ReSharper disable InconsistentNaming
    public class DataAccessContextSpecs
    {
        public abstract class DataAccessContextScenario
        {
            protected IDataAccessProvider Provider;
            protected IDataAccessContext DataContext;

            protected DataAccessContextScenario()
            {
                Provider = A.Fake<IDataAccessProvider>();
                DataContext = new DefaultDataAccessContext(Provider);

                A.CallTo(() => Provider.SessionFactory.Open())
                 .ReturnsLazily(_ => A.Dummy<IDataSession>());
                
            }

            [TearDown]
            public void TearDown()
            {
                DataContext.CloseSession();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_default_DataAccessContext_is_created : DataAccessContextScenario
        {
            [Test]
            public void it_should_not_have_open_session()
            {
                DataContext.HasOpenSession.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_session_using_default_DataAccessContext_first_time : DataAccessContextScenario
        {
            [Test]
            public void should_return_new_session_and_remember_its_state()
            {
                var session = DataContext.GetSession();

                DataContext.HasOpenSession.Should().BeTrue();

                session.Should().NotBeNull();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_session_using_default_DataAccessContext_several_times_without_closing : DataAccessContextScenario
        {
            [Test]
            public void should_return_the_same_session()
            {
                var session1 = DataContext.GetSession();
                var session2 = DataContext.GetSession();
                var session3 = DataContext.GetSession();

                DataContext.HasOpenSession.Should().BeTrue();

                session1.Should().Be(session2).And.Be(session3);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_closing_session_using_default_DataAccessContext_without_open_session : DataAccessContextScenario
        {
            [Test]
            public void should_do_nothing()
            {
                DataContext.HasOpenSession.Should().BeFalse();

                DataContext.CloseSession();

                DataContext.HasOpenSession.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_closing_session_using_default_DataAccessContext_with_open_session : DataAccessContextScenario
        {
            [Test]
            public void should_close_session_and_free_context_storage()
            {
                DataContext.GetSession();

                DataContext.CloseSession();

                DataContext.HasOpenSession.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_reopening_session_using_default_DataAccessContext : DataAccessContextScenario
        {
            [Test]
            public void should_return_new_session()
            {
                var session1 = DataContext.GetSession();

                DataContext.CloseSession();

                var session2 = DataContext.GetSession();

                session1.Should().NotBe(session2);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_disposing_default_DataAccessContext_with_open_session : DataAccessContextScenario
        {
            [Test]
            public void should_close_session_and_free_context_storage()
            {
                IDataAccessContext context;
                using (context = new DefaultDataAccessContext(Provider))
                {
                    context.GetSession();
                }

                context.HasOpenSession.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_multiple_instances_of_default_DataAccessContext : DataAccessContextScenario
        {
            [Test]
            public void they_should_use_separate_sessions()
            {
                IDataAccessContext context1 = new DefaultDataAccessContext(Provider);
                IDataAccessContext context2 = new DefaultDataAccessContext(Provider);

                context1.HasOpenSession.Should().BeFalse();
                context2.HasOpenSession.Should().BeFalse();

                var session1 = context1.GetSession();
                context1.HasOpenSession.Should().BeTrue();
                context2.HasOpenSession.Should().BeFalse();

                var session2 = context2.GetSession();
                context1.HasOpenSession.Should().BeTrue();
                context2.HasOpenSession.Should().BeTrue();

                session1.Should().NotBe(session2);

                context1.CloseSession();
                context1.HasOpenSession.Should().BeFalse();
                context2.HasOpenSession.Should().BeTrue();

                context2.CloseSession();
                context1.HasOpenSession.Should().BeFalse();
                context2.HasOpenSession.Should().BeFalse();
            }
        }


    }
    // ReSharper restore InconsistentNaming

}
