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
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class OptionSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_creating_none
        {
            [Test]
            public void it_should_be_in_valid_state()
            {
                var option = Option<string>.None;

                option.IsNone.Should().BeTrue();
                option.IsSome.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_none_of_value_type
        {
            [Test]
            public void it_should_be_in_valid_state()
            {
                var option = Option<int>.None;

                option.IsNone.Should().BeTrue();
                option.IsSome.Should().BeFalse();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_some
        {
            [Test]
            public void it_should_be_in_valid_state()
            {
                var option = Option<string>.Some("zzz");

                option.IsNone.Should().BeFalse();
                option.IsSome.Should().BeTrue();
                option.Value.Should().Be("zzz");
				(option as IOption).ValueObject.Should().Be("zzz");
			}
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_some_of_value_type
        {
            [Test]
            public void it_should_be_in_valid_state()
            {
                var option = Option<int>.Some(666);

                option.IsNone.Should().BeFalse();
                option.IsSome.Should().BeTrue();
                option.Value.Should().Be(666);
				(option as IOption).ValueObject.Should().Be(Int32.Parse("666"));
			}
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_some_using_null_value
        {
            [Test]
            public void should_throw()
            {
                Action action = () => Option<string>.Some(null);

                action.ShouldThrow<ArgumentNullException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_trying_to_get_value_of_none
        {
            [Test]
            public void should_throw()
            {
                var option = Option<string>.None;

				#pragma warning disable 168
                option.Invoking(o => { var x = o.Value; }).ShouldThrow<NotSupportedException>();
				#pragma warning restore 168
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_from_null
        {
            [Test]
            public void it_should_be_none()
            {
                string nullObject = null;

				// ReSharper disable ExpressionIsAlwaysNull
                var option = nullObject.AsOption();
				// ReSharper restore ExpressionIsAlwaysNull

                option.Should().BeOfType<None<string>>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_from_default_value_type
        {
            [Test]
            public void it_should_be_some()
            {
                const int defaultObject = default(int);

                var option = defaultObject.AsOption();

                option.Should().BeOfType<Some<int>>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_from_initialized_object
        {
            [Test]
            public void it_should_be_some()
            {
                const string someObject = "zzz";

                var option = someObject.AsOption();

                option.Should().BeOfType<Some<string>>();
                option.Value.Should().Be("zzz");
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_implicitly_from_initialized_object
        {
            [Test]
            public void it_should_be_some()
            {
                const string someObject = "zzz";

                Option<string> option = someObject;

                option.Should().BeOfType<Some<string>>();
                option.Value.Should().Be("zzz");
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_implicitly_from_null
        {
            [Test]
            public void it_should_be_none()
            {
                string nullObject = null;

				// ReSharper disable ExpressionIsAlwaysNull
                Option<string> option = nullObject;

                option.Should().BeOfType<None<string>>();
				// ReSharper restore ExpressionIsAlwaysNull
			}
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_some_to_object_explicitly
        {
            [Test]
            public void should_assign_the_value()
            {
                const string someObject = "zzz";

                Option<string> option = someObject;

                var anotherObject = (string)option;

                anotherObject.Should().Be(someObject);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_none_to_object_explicitly
        {
            [Test]
            public void should_throw()
            {
                string someObject = null;

				// ReSharper disable ExpressionIsAlwaysNull
                Option<string> option = someObject;

				#pragma warning disable 168
                option.Invoking(o => { var x = (string)o; }).ShouldThrow<NotSupportedException>();
				#pragma warning restore 168
				// ReSharper restore ExpressionIsAlwaysNull
			}
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_of_option
        {
            [Test]
            public void should_throw()
            {
                var someObject = "zzz".AsOption();

				#pragma warning disable 168
                someObject.Invoking(o => { var x = o.AsOption(); }).ShouldThrow<InvalidOperationException>();
				#pragma warning restore 168
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
