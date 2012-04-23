using System;
using NUnit.Framework;

namespace Kostassoid.Anodyne.Common.Specs
{
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

                Assert.That(option.IsNone, Is.True);
                Assert.That(option.IsSome, Is.False);
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

                Assert.That(option.IsNone, Is.False);
                Assert.That(option.IsSome, Is.True);
                Assert.That(option.Value, Is.EqualTo("zzz"));
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
                Assert.That(() => { var x = option.Value; }, Throws.TypeOf<NotSupportedException>());
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

                var option = nullObject.AsOption();

                Assert.That(option is None<string>, Is.True);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_from_initialized_object
        {
            [Test]
            public void it_should_be_some()
            {
                var someObject = "zzz";

                var option = someObject.AsOption();

                Assert.That(option is Some<string>, Is.True);
                Assert.That(option.Value, Is.EqualTo(someObject));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_option_implicitly_from_initialized_object
        {
            [Test]
            public void it_should_be_some()
            {
                var someObject = "zzz";

                Option<string> option = someObject;

                Assert.That(option is Some<string>, Is.True);
                Assert.That(option.Value, Is.EqualTo(someObject));
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

                Option<string> option = nullObject;

                Assert.That(option is None<string>, Is.True);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_some_to_object_explicitly
        {
            [Test]
            public void should_assign_the_value()
            {
                var someObject = "zzz";

                Option<string> option = someObject;

                var anotherObject = (string)option;

                Assert.That(anotherObject, Is.EqualTo(someObject));
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

                Option<string> option = someObject;

                Assert.That(() => { var anotherObject = (string) option; }, Throws.TypeOf<NotSupportedException>());
            }
        }



    }
    // ReSharper restore InconsistentNaming

}
