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

namespace Kostassoid.Anodyne.Wiring.Specs
{
    using System.Threading;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class AsyncEventBusSpecs
    {
        public class TestEvent : IEvent
        {
        }

        public class DerivedTestEvent : TestEvent
        {
        }

        public class AsyncTestHandler : IHandlerOf<DerivedTestEvent>
        {
            public AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

            public int Fired { get; set; }

            void IHandlerOf<DerivedTestEvent>.Handle(DerivedTestEvent ev)
            {
                Thread.Sleep(200);
                lock (this)
                    Fired++;

                AutoResetEvent.Set();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_firing_using_async_handler
        {
            [Test]
            public void should_call_handler_async()
            {
                var handler = new AsyncTestHandler();
                EventBus.SubscribeTo<DerivedTestEvent>().WithAsync(handler);
                EventBus.Publish(new DerivedTestEvent());

                handler.Fired.Should().Be(0);

                handler.AutoResetEvent.WaitOne(1000);

                handler.Fired.Should().Be(1);
            }
        }


    }
    // ReSharper restore InconsistentNaming

}
