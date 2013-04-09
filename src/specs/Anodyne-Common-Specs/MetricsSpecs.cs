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
    using FluentAssertions;
    using Measure;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class MetricsSpecs
    {
        [TestFixture]
        [Category("Unit")]
        public class when_creating_digital_size_from_bytes
        {
            [Test]
            public void should_store_as_is()
            {
                var size = DigitalStorageSize.FromBytes(123456789L);

                size.Bytes.Should().Be(123456789L);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_digital_size_from_kilobytes
        {
            [Test]
            public void should_store_as_bytes()
            {
                var size = DigitalStorageSize.FromKilobytes(123L);

                size.Bytes.Should().Be(123L * 1024);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_digital_size_from_megabytes
        {
            [Test]
            public void should_store_as_bytes()
            {
                var size = DigitalStorageSize.FromKilobytes(123L);

                size.Bytes.Should().Be(123L * 1024);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_same_sized_objects
        {
            [Test]
            public void should_be_equal()
            {
                var size1 = DigitalStorageSize.FromBytes(123L * 1024L);
                var size2 = DigitalStorageSize.FromKilobytes(123L);

                size1.Bytes.Should().Be(size2.Bytes);
                size1.Should().Be(size2);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_using_int32_extension_methods
        {
            [Test]
            public void should_be_equivalent_to_direct_methods()
            {
                var size1 = DigitalStorageSize.FromBytes(123456789L);
                var size2 = 123456789.Bytes();
                size1.Bytes.Should().Be(size2.Bytes);

                size1 = DigitalStorageSize.FromKilobytes(123L);
                size2 = 123.Kilobytes();
                size1.Bytes.Should().Be(size2.Bytes);

                size1 = DigitalStorageSize.FromMegabytes(123L);
                size2 = 123.Megabytes();
                size1.Bytes.Should().Be(size2.Bytes);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_creating_using_int64_extension_methods
        {
            [Test]
            public void should_be_equivalent_to_direct_methods()
            {
                var size1 = DigitalStorageSize.FromBytes(123456789L);
                var size2 = 123456789L.Bytes();
                size1.Bytes.Should().Be(size2.Bytes);

                size1 = DigitalStorageSize.FromKilobytes(123L);
                size2 = 123L.Kilobytes();
                size1.Bytes.Should().Be(size2.Bytes);

                size1 = DigitalStorageSize.FromMegabytes(123L);
                size2 = 123L.Megabytes();
                size1.Bytes.Should().Be(size2.Bytes);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_size_in_different_metrics
        {
            [Test]
            [TestCase(123456789L)]
            [TestCase(1024L)]
            [TestCase(1023L)]
            [TestCase(0L)]
            [TestCase(1L)]
            [TestCase(333L)]
            public void should_return_closest_values_according_to_metric_scale(long bytes)
            {
                var size = DigitalStorageSize.FromBytes(bytes);

                var kb = size.Kilobytes;
                (kb * 1024).Should().BeInRange(size.Bytes - 1024, size.Bytes);

                var mb = size.Megabytes;
                (mb * 1024 * 1024).Should().BeInRange(size.Bytes - (1024 * 1024), size.Bytes);
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
