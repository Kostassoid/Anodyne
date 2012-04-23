using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kostassoid.Anodyne.Common.ExecutionContext;
using Kostassoid.Anodyne.Specs.Shared.DataGeneration;
using NUnit.Framework;

namespace Kostassoid.Anodyne.Common.Specs
{
    // ReSharper disable InconsistentNaming
    public class ExecutionContextSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_setting_new_value
        {
            [Test]
            public void it_should_be_stored_and_available()
            {
                Context.Set("test", "zzz");

                Assert.That(Context.Get("test"), Is.EqualTo("zzz"));
                Assert.That(Context.GetAs<string>("test"), Is.EqualTo("zzz"));
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

                Assert.That((int)Context.Get("test"), Is.EqualTo(123));
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

                Assert.That(() => Context.Get("testzzz"), Throws.InvalidOperationException);
                Assert.That(() => Context.GetAs<string>("testzzz"), Throws.InvalidOperationException);
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

                Assert.That(Context.Find("testzzz").IsNone, Is.True);
                Assert.That(Context.FindAs<string>("testzzz").IsNone, Is.True);
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

                Assert.That(Context.Find("test").Value, Is.EqualTo("zzz"));
                Assert.That(Context.FindAs<string>("test").Value, Is.EqualTo("zzz"));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_released_value
        {
            [Test]
            public void should_return_some()
            {
                Context.Set("test", "zzz");
                Context.Release("test");

                Assert.That(() => Context.Get("test"), Throws.InvalidOperationException);
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
                                Assert.That(foundValue, Is.EqualTo(randomValue));
                            }))
                    .ToArray();

                Task.WaitAll(tasks);
            }
        }




    }
    // ReSharper restore InconsistentNaming

}
