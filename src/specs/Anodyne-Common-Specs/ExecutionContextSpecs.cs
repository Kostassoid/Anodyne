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

namespace Kostassoid.Anodyne.Common.Specs
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Anodyne.Specs.Shared.DataGeneration;
    using ExecutionContext;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ExecutionContextSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_getting_existing_value_of_object
        {
            [Test]
            public void should_return_stored_value()
            {
                Context.Set("test", "zzz");

                Context.Get("test").Should().Be("zzz");
                Context.GetAs<string>("test").Should().Be("zzz");
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_existing_value_of_value_type
        {
            [Test]
            public void should_return_stored_value()
            {
                Context.Set("test", 123);

                Context.Get("test").Should().Be(123);
                Context.GetAs<int>("test").Should().Be(123);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_resetting_value
        {
            [Test]
            public void old_value_should_be_replaced()
            {
                Context.Set("test", "zzz");
                Context.Set("test", 123);

                Context.Get("test").Should().Be(123);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_non_existing_value
        {
            [Test]
            public void should_throw()
            {
                Context.Set("test", "zzz");

                Action action = () => Context.Get("testzzz");
                action.ShouldThrow<InvalidOperationException>();

                action = () => Context.GetAs<string>("testzzz");
                action.ShouldThrow<InvalidOperationException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_finding_non_existing_value
        {
            [Test]
            public void should_return_none()
            {
                Context.Set("test", "zzz");

                Context.Find("testzzz").IsNone.Should().BeTrue();
                Context.FindAs<string>("testzzz").IsNone.Should().BeTrue();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_finding_non_existing_value_of_value_type
        {
            [Test]
            public void should_return_none()
            {
                Context.FindAs<int>("testzzz").IsNone.Should().BeTrue();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_finding_existing_value
        {
            [Test]
            public void should_return_some()
            {
                Context.Set("test", "zzz");

                Context.Find("test").Value.Should().Be("zzz");
                Context.FindAs<string>("test").Value.Should().Be("zzz");
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_finding_existing_value_of_value_type
        {
            [Test]
            public void should_return_some()
            {
                Context.Set("test", 123);

                Context.Find("test").Value.Should().Be(123);
                Context.FindAs<int>("test").Value.Should().Be(123);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_finding_existing_value_of_invalid_type
        {
            [Test]
            public void should_return_none()
            {
                Context.Set("test", 123);

                Context.FindAs<string>("test").IsNone.Should().BeTrue();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_existing_value_of_invalid_type
        {
            [Test]
            public void should_throw()
            {
                Context.Set("test", 123);

                Action action = () => Context.GetAs<string>("test");

                action.ShouldThrow<InvalidOperationException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_released_value
        {
            [Test]
            public void should_throw()
            {
                Context.Set("test", "zzz");
                Context.Release("test");

                Action action = () => Context.Get("test");
                action.ShouldThrow<InvalidOperationException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_working_with_the_same_keys_in_different_threads
        {
            [Test]
            public void should_isolate_values()
            {
                var tasks = Enumerable.Range(0, 50)
                    .Select(i => Task.Factory.StartNew(() =>
                            {
                                var randomValue = Imagine.Any.Int();
                                Context.Set("test", randomValue);

                                Thread.Sleep(Imagine.Any.Int(0, 20)); // a little bit of chaos

                                var foundValue = (int) Context.Get("test");

                                foundValue.Should().Be(randomValue);
                            }))
                    .ToArray();

                Task.WaitAll(tasks);
            }
        }

		[TestFixture]
		[Category("Unit")]
		public class when_setting_null_value
		{
			[Test]
			public void should_release_value()
			{
				Action gettingSomeValue = () => Context.Get("test");

				Context.Set("test", "zzz");
				gettingSomeValue.ShouldNotThrow();

				Context.Set("test", null);
				gettingSomeValue.ShouldThrow<InvalidOperationException>();
			}
		}

        [TestFixture]
        [Category("Unit")]
        public class when_setting_some_option_value
        {
            [Test]
            public void should_put_unboxed_value()
            {
                var value = "zzz".AsOption();

                Context.Set("test", value);

                var actualValue = Context.Get("test");

                actualValue.Should().BeOfType<String>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_setting_custom_provider
        {
            [Test]
            public void should_use_new_provider()
            {
                var provider = A.Fake<IContextProvider>();
                A.CallTo(() => provider.Find("boo")).Returns("xxx");

                Context.FindAs<string>("boo").ValueOrDefault.Should().BeNull();

                Context.SetProvider(provider);

                Context.FindAs<string>("boo").ValueOrDefault.Should().Be("xxx");
            }

            [TearDown]
            public void TearDown()
            {
                Context.SetProvider(new DefaultContextProvider());
            }
        }


    }
    // ReSharper restore InconsistentNaming

}
