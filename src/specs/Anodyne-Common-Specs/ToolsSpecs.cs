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
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Tools;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ToolsSpecs
    {

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids
        {
            [Test]
            public void they_should_be_unique()
            {
                var generatedSet = Enumerable.Range(0, 100).Select(_ => SeqGuid.NewGuid()).ToList();

                generatedSet.Should().OnlyHaveUniqueItems();
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

                SeqGuid.ToDateTime(generatedGuid).Should().BeCloseTo(now);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_generating_many_seq_guids_async
        {
            [Test]
            public void they_should_be_unique()
            {
                var generatedSet = new ConcurrentBag<Guid>();
                var tasks = Enumerable.Range(0, 1000)
                    .Select(_ => Task.Factory.StartNew(() => generatedSet.Add(SeqGuid.NewGuid())))
                    .ToArray();

                Task.WaitAll(tasks);

                generatedSet.Should().HaveCount(1000);
                generatedSet.Should().OnlyHaveUniqueItems();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_without_params
        {

            [Test]
            public void should_return_cached_value()
            {
                var counter = 0;
                Func<int> func = () =>
                {
                    counter++;
                    return 13;
                };

                var memoizedFunc = func.AsMemoized();

                memoizedFunc().Should().Be(13);
                memoizedFunc().Should().Be(13);
                memoizedFunc().Should().Be(13);

                counter.Should().Be(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_one_param
        {

            [Test]
            public void should_return_cached_values()
            {
                var counter = 0;
                Func<int, int> func = x =>
                {
                    counter++;
                    return x * x;
                };

                var memoizedFunc = func.AsMemoized();

                memoizedFunc(5).Should().Be(25);
                memoizedFunc(4).Should().Be(16);
                memoizedFunc(4).Should().Be(16);
                memoizedFunc(5).Should().Be(25);
                memoizedFunc(4).Should().Be(16);
                memoizedFunc(5).Should().Be(25);

                counter.Should().Be(2);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_two_params
        {

            [Test]
            public void should_return_cached_values()
            {
                var counter = 0;
                Func<int, int, int> func = (x, y) =>
                {
                    counter++;
                    return x * y;
                };

                var memoizedFunc = func.AsMemoized();

                memoizedFunc(2, 3).Should().Be(6);
                memoizedFunc(2, 5).Should().Be(10);
                memoizedFunc(4, 3).Should().Be(12);
                memoizedFunc(2, 3).Should().Be(6);
                memoizedFunc(2, 5).Should().Be(10);
                memoizedFunc(4, 3).Should().Be(12);

                counter.Should().Be(3);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_memoized_function_with_three_params
        {

            [Test]
            public void should_return_cached_values()
            {
                var counter = 0;
                Func<int, int, int, int> func = (x, y, z) =>
                {
                    counter++;
                    return x * y * z;
                };

                var memoizedFunc = func.AsMemoized();

                memoizedFunc(2, 3, 4).Should().Be(24);
                memoizedFunc(2, 3, 5).Should().Be(30);
                memoizedFunc(2, 4, 3).Should().Be(24);
                memoizedFunc(2, 3, 5).Should().Be(30);
                memoizedFunc(2, 3, 4).Should().Be(24);
                memoizedFunc(2, 4, 3).Should().Be(24);

                counter.Should().Be(3);
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
