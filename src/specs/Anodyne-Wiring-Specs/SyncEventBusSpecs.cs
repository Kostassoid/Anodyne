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
    using System.Diagnostics;
    using System.Text;
    using Common;
    using Common.Reflection;
    using FakeItEasy;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class SyncEventBusSpecs
    {
        public class TestEvent : IEvent
        {
        }

        public class DerivedTestEvent : TestEvent
        {
        }

        public class TestHandler
        {

            public int Fired1 { get; set; }
            public int Fired2 { get; set; }

            public void SomeMethod(TestEvent ev)
            {
                Fired1++;
            }

            protected void Handle(DerivedTestEvent ev)
            {
                Fired2++;
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_subscribing_one_on_one_with_handler_object
        {
            [Test]
            public void should_bind_event_to_handler()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventBus.SubscribeTo<TestEvent>().With(handler);
                EventBus.Publish(new TestEvent());

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

                EventBus.SubscribeTo<TestEvent>().With(handler);
                EventBus.Publish(new DerivedTestEvent());

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

                EventBus.SubscribeTo<TestEvent>().With(handler);
                EventBus.SubscribeTo<DerivedTestEvent>().With(handler);

                EventBus.Publish(new DerivedTestEvent());

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

                EventBus.SubscribeTo().AllBasedOn<TestEvent>().With(handler);
                EventBus.Publish(new DerivedTestEvent());
                EventBus.Publish(new TestEvent());

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

                EventBus.SubscribeTo().AllBasedOn<TestEvent>(From.ThisAssembly).Where(t => t.Name.Contains("Derived")).With(handler);
                EventBus.Publish(new DerivedTestEvent());
                EventBus.Publish(new TestEvent());

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

                EventBus.SubscribeTo().AllBasedOn<TestEvent>(From.Assemblies(a => a.FullName.Contains("Wiring-Specs"))).With(handler);

                EventBus.Publish(new DerivedTestEvent());
                EventBus.Publish(new TestEvent());

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

                var unsubscribe = EventBus.SubscribeTo<TestEvent>().With(handler);
                unsubscribe();

                EventBus.Publish(new TestEvent());

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

                EventBus.SubscribeTo<TestEvent>().With(action);
                EventBus.Publish(new TestEvent());

                handled.Should().BeTrue();
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

                EventBus.SubscribeTo<TestEvent>().When(_ => false).With(handler);

                EventBus.Publish(new TestEvent());

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

                var unsubscribe = EventBus.SubscribeTo().AllBasedOn<TestEvent>(From.ThisAssembly).With(handler);
                unsubscribe();

                EventBus.Publish(new TestEvent());
                EventBus.Publish(new DerivedTestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_event_after_resetting_event_router
        {
            [Test]
            public void should_not_call_handlers()
            {
                var handler = A.Fake<IHandlerOf<TestEvent>>();

                EventBus.SubscribeTo().AllBasedOn<TestEvent>(From.ThisAssembly).With(handler);

                EventBus.Reset();

                EventBus.Publish(new TestEvent());
                EventBus.Publish(new DerivedTestEvent());

                A.CallTo(() => handler.Handle(A<TestEvent>.Ignored)).MustNotHaveHappened();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_events_using_strict_matching_subscription
        {
            [Test]
            public void should_call_handlers_once()
            {
                var handler = new TestHandler();

                EventBus.Reset();
                EventBus
                    .SubscribeTo()
                    .AllBasedOn<TestEvent>(From.ThisAssembly)
                    .With<TestHandler>(EventMatching.Strict)
                    .As(_ => handler);

                EventBus.Publish(new TestEvent());
                EventBus.Publish(new DerivedTestEvent());

                handler.Fired1.Should().Be(1);
                handler.Fired2.Should().Be(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_events_using_all_matching_subscription
        {
            [Test]
            public void should_call_base_event_handler_each_time()
            {
                var handler = new TestHandler();

                EventBus.Reset();
                EventBus
                    .SubscribeTo()
                    .AllBasedOn<TestEvent>(From.ThisAssembly)
                    .With<TestHandler>(EventMatching.All)
                    .As(_ => handler);

                EventBus.Publish(new TestEvent());
                EventBus.Publish(new DerivedTestEvent());

                handler.Fired1.Should().Be(2);
                handler.Fired2.Should().Be(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_measuring_speed_of_handler_invocations
        {
            [Test]
            [Explicit("For performance experiments")]
            public void should_be_fast()
            {
                EventBus.Reset();

                var handler = new TestHandler();

                EventBus.Reset();
                EventBus
                    .SubscribeTo()
                    .AllBasedOn<TestEvent>(From.ThisAssembly)
                    .With<TestHandler>(EventMatching.Strict)
                    .As(_ => handler);


                var stopwatch = new Stopwatch();
                stopwatch.Start();

                const int eventsCount = 1000000;

                var i = eventsCount;
                while (i --> 0)
                {
                    EventBus.Publish(new TestEvent());
                    EventBus.Publish(new DerivedTestEvent());
                }

                stopwatch.Stop();

                var speed = eventsCount * 2 / stopwatch.Elapsed.TotalSeconds;

                handler.Fired1.Should().Be(eventsCount);
                handler.Fired2.Should().Be(eventsCount);

                speed.Should().BeGreaterOrEqualTo(1000000); //impossible (yet) mark by design, not an actual test
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_events_using_different_priority
        {
            [Test]
            public void should_call_handlers_in_correct_order()
            {
                var builder = new StringBuilder();
                Action<IEvent> handler1 = (ev => builder.Append("a"));
                Action<IEvent> handler2 = (ev => builder.Append("b"));
                Action<IEvent> handler3 = (ev => builder.Append("c"));

                EventBus.Reset();
                EventBus.SubscribeTo<TestEvent>().With(handler1, Priority.Normal);
                EventBus.SubscribeTo<TestEvent>().With(handler2, Priority.Critical);
                EventBus.SubscribeTo<TestEvent>().With(handler3, Priority.High);

                EventBus.Publish(new TestEvent());

                builder.ToString().Should().Be("bca");
            }
        }




    }
    // ReSharper restore InconsistentNaming

}
