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

namespace Kostassoid.Anodyne.Wiring.Specs
{
    using System;
    using FakeItEasy;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class SubscriptionSpecs
    {
        public class TestEvent : IEvent
        {
        }

        public class DerivedTestEvent : TestEvent
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
        public class when_firing_derived_event_with_base_event_subscribed
        {
            [Test]
            public void should_not_call_base_event_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn<TestEvent>().With(handler);
                EventRouter.Fire(new DerivedTestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_derived_event_with_base_event_subscribed_additionally
        {
            [Test]
            public void should_call_base_event_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn<TestEvent>().With(handler);
                EventRouter.ReactOn<DerivedTestEvent>().With(handler);

                EventRouter.Fire(new DerivedTestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_derived_event_subscribed_using_this_assembly
        {
            [Test]
            public void should_call_base_event_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn().AllBasedOn<TestEvent>().FromThisAssembly().With(handler);
                EventRouter.Fire(new DerivedTestEvent());
                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_derived_event_subscribed_using_this_assembly_with_predicate
        {
            [Test]
            public void should_call_handler_only_for_allowed_event()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn().AllBasedOn<TestEvent>().FromThisAssembly().Where(t => t.Name.Contains("Derived")).With(handler);
                EventRouter.Fire(new DerivedTestEvent());
                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Once);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_derived_event_when_subscribed_using_named_assembly
        {
            [Test]
            public void should_call_base_event_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn().AllBasedOn<TestEvent>().From(p => p.Contains("Wiring-Specs")).With(handler);

                EventRouter.Fire(new DerivedTestEvent());
                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustHaveHappened(Repeated.Exactly.Twice);

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_event_with_unsubscribed_handler
        {
            [Test]
            public void should_not_call_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                var unsubscribe = EventRouter.ReactOn<TestEvent>().With(handler);
                unsubscribe();

                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();
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
        public class when_firing_subscribed_event_with_false_predicate
        {
            [Test]
            public void should_not_call_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventRouter.ReactOn<TestEvent>().When(_ => false).With(handler);

                EventRouter.Fire(new TestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();

            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_unsubscribing_from_subscription_made_by_convention
        {
            [Test]
            public void should_unsubscribe_from_all_sources()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                var unsubscribe = EventRouter.ReactOn().AllBasedOn<TestEvent>().FromThisAssembly().With(handler);
                unsubscribe();

                EventRouter.Fire(new TestEvent());
                EventRouter.Fire(new DerivedTestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();
            }
        }



    }
    // ReSharper restore InconsistentNaming

}
