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
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ObjectFactorySpecs
    {
        public class Foo
        {
            public int Field1 { get; protected set; }
            public string Field2 { get; protected set; }

            public Foo()
            {
                Field1 = 2;
                Field2 = "Hell";
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_building_objects_with_same_data
        {
            [Test]
            public void objects_should_be_equal()
            {

                var o1 = new Foo();
                var o2 = ObjectFactory.Build<Foo>();

                o1.Field1.Should().Be(o2.Field1);
                o1.Field2.Should().Be(o2.Field2);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class creation_of_many_objects
        {
            [Test]
            [Explicit("Doesn't play nice with test coverage tools in CI environment")]
            public void should_not_be_much_slower_than_using_new_operator()
            {
                const int HowMuchTimesSlowerIsOk = 5;

                const int ObjectsCount = 1000000;

                var started = DateTime.Now;
                var count = ObjectsCount;
                while (count-- > 0)
                {
                    new Foo();
                }
                var speedNew = (DateTime.Now - started).TotalMilliseconds;

                ObjectFactory.Build<Foo>();

                started = DateTime.Now;
                count = ObjectsCount;
                while (count-- > 0)
                {
                    ObjectFactory.Build<Foo>();
                }
                var speedFactory = (DateTime.Now - started).TotalMilliseconds;

                speedFactory.Should().BeLessThan(speedNew * HowMuchTimesSlowerIsOk);
            }
        }


    }
    // ReSharper restore InconsistentNaming
}