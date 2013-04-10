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
    using System.Threading;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class SystemTimeSpecs
    {
        public class SystemTimeScenario
        {
            public SystemTimeScenario()
            {
                SystemTime.TimeController.Reset();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_default_now_date : SystemTimeScenario
        {
            [Test]
            public void should_return_datetime_now_in_utc()
            {
                var now = SystemTime.Now;

                now.Should().BeCloseTo(DateTime.UtcNow, 100);
                now.Kind.Should().Be(DateTimeKind.Utc);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_now_date_after_freezing : SystemTimeScenario
        {
            [Test]
            public void should_return_frozen_date()
            {
                var fixedDate = new DateTime(2013, 1, 2, 3, 4, 5, 6).ToUniversalTime();

                SystemTime.TimeController.SetFrozenDate(fixedDate);

                SystemTime.Now.Should().Be(fixedDate);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_now_date_after_setting_custom_date : SystemTimeScenario
        {
            [Test]
            public void should_return_set_date_shifted_by_delay_from_set_moment()
            {
                var fixedDate = new DateTime(2013, 1, 2, 3, 4, 5, 6).ToUniversalTime();

                SystemTime.TimeController.SetDate(fixedDate);
                SystemTime.Now.Should().BeCloseTo(fixedDate, 100);

                var actualNow = DateTime.UtcNow;

                Thread.Sleep(500);

                var elapsed = DateTime.UtcNow - actualNow;

                SystemTime.Now.Should().BeCloseTo(fixedDate.Add(elapsed), 100);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_now_date_after_setting_custom_date_func : SystemTimeScenario
        {
            [Test]
            public void should_return_date_provided_by_func()
            {
                var fixedDate = new DateTime(2013, 1, 2, 3, 4, 5, 6).ToUniversalTime();

                SystemTime.TimeController.Customize(() => fixedDate.AddMinutes(123));
                SystemTime.Now.AddMinutes(-123).Should().Be(fixedDate);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_now_date_after_reset : SystemTimeScenario
        {
            [Test]
            public void should_return_actual_now_date()
            {
                var fixedDate = new DateTime(2013, 1, 2, 3, 4, 5, 6).ToUniversalTime();

                SystemTime.TimeController.SetFrozenDate(fixedDate);

                SystemTime.TimeController.Reset();

                SystemTime.Now.Should().BeCloseTo(DateTime.UtcNow, 100);
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
