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

namespace Kostassoid.Anodyne.Common.Specs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using Kostassoid.Anodyne.Common.Tools;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ToolsSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids
        {

            [Test]
            public void they_should_be_sequental_and_unique()
            {
                var generatedSet = new List<Guid>();

                var i = 100;
                while (i-- > 0)
                {
                    generatedSet.Add(SeqGuid.NewGuid());
                }

                Assert.That(new HashSet<Guid>(generatedSet).Count, Is.EqualTo(100));
                Assert.That(generatedSet.OrderBy(g => g), Is.EquivalentTo(generatedSet));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_date_from_seq_guid
        {

            [Test]
            public void should_be_the_same_as_generation_date()
            {
                var now = SystemTime.Now;
                var generatedGuid = SeqGuid.NewGuid();

                Assert.That((SeqGuid.ToDateTime(generatedGuid) - now).TotalMilliseconds, Is.LessThan(20));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids_async
        {
            [Test]
            [Ignore("They're not unique, and .NET Guid is not unique, life's a pain")]
            public void they_should_be_unique()
            {
                var generatedSet = new List<Guid>();
                var tasks = new List<Task>();

                var i = 1000;
                while (i --> 0)
                {
                    tasks.Add(Task.Factory.StartNew(() => generatedSet.Add(SeqGuid.NewGuid())));
                }

                Task.WaitAll(tasks.ToArray());

                Assert.That(new HashSet<Guid>(generatedSet).Count, Is.EqualTo(1000));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_without_params
        {

            [Test]
            public void should_return_cached_value()
            {
                Func<int> func = () =>
                {
                    Thread.Sleep(50);
                    return 13;
                };

                var memoizedFunc = func.AsMemoized();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Assert.That(memoizedFunc(), Is.EqualTo(13));
                Assert.That(memoizedFunc(), Is.EqualTo(13));
                Assert.That(memoizedFunc(), Is.EqualTo(13));

                stopwatch.Stop();

                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_one_param
        {

            [Test]
            public void should_return_cached_values()
            {
                Func<int, int> func = x =>
                {
                    Thread.Sleep(30);
                    return x * x;
                };

                var memoizedFunc = func.AsMemoized();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Assert.That(memoizedFunc(5), Is.EqualTo(25));
                Assert.That(memoizedFunc(4), Is.EqualTo(16));
                Assert.That(memoizedFunc(4), Is.EqualTo(16));
                Assert.That(memoizedFunc(5), Is.EqualTo(25));
                Assert.That(memoizedFunc(4), Is.EqualTo(16));
                Assert.That(memoizedFunc(5), Is.EqualTo(25));

                stopwatch.Stop();

                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(100));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_two_params
        {

            [Test]
            public void should_return_cached_values()
            {
                Func<int, int, int> func = (x, y) =>
                {
                    Thread.Sleep(30);
                    return x * y;
                };

                var memoizedFunc = func.AsMemoized();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Assert.That(memoizedFunc(2, 3), Is.EqualTo(6));
                Assert.That(memoizedFunc(2, 5), Is.EqualTo(10));
                Assert.That(memoizedFunc(4, 3), Is.EqualTo(12));
                Assert.That(memoizedFunc(2, 3), Is.EqualTo(6));
                Assert.That(memoizedFunc(2, 5), Is.EqualTo(10));
                Assert.That(memoizedFunc(4, 3), Is.EqualTo(12));

                stopwatch.Stop();

                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(120));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_three_params
        {

            [Test]
            public void should_return_cached_values()
            {
                Func<int, int, int, int> func = (x, y, z) =>
                {
                    Thread.Sleep(30);
                    return x * y * z;
                };

                var memoizedFunc = func.AsMemoized();

                var stopwatch = new Stopwatch();
                stopwatch.Start();

                Assert.That(memoizedFunc(2, 3, 4), Is.EqualTo(24));
                Assert.That(memoizedFunc(2, 3, 5), Is.EqualTo(30));
                Assert.That(memoizedFunc(2, 4, 3), Is.EqualTo(24));
                Assert.That(memoizedFunc(2, 3, 5), Is.EqualTo(30));
                Assert.That(memoizedFunc(2, 3, 4), Is.EqualTo(24));
                Assert.That(memoizedFunc(2, 4, 3), Is.EqualTo(24));

                stopwatch.Stop();

                Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(120));
            }
        }


    }
    // ReSharper restore InconsistentNaming

}
