using System;
using FakeItEasy;
using NUnit.Framework;

namespace Kostassoid.Anodyne.Wiring.Specs
{
    // ReSharper disable InconsistentNaming
    public class SubscriptionSpecs
    {
        public class TestEvent : IEvent
        {
        }

        [TestFixture]
        [Category("Unit")]
        public class when_subscribing_one_on_one_with_handler_object
        {
            [Test]
            public void should_bind_event_to_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn<TestEvent>().With(handler);
                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustHaveHappened();

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_subscribing_one_on_one_with_action
        {
            [Test]
            public void should_bind_event_to_handler()
            {
                var handled = new bool();
                var action = new Action<TestEvent>(_ => { handled = true; });

                EventRouter.ReactOn<TestEvent>().With(action);
                EventRouter.Fire(new TestEvent());

                Assert.That(handled, Is.True);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_subscribing_one_on_one_with_predicate
        {
            [Test]
            [Ignore("Work in progress")]
            public void should_bind_event_to_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn<TestEvent>().When(_ => false).With(handler);

                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();

            }
        }


    }
    // ReSharper restore InconsistentNaming

}
