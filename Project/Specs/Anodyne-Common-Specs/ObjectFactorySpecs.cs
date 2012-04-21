using System;
using NUnit.Framework;

namespace Kostassoid.Anodyne.Common.Specs
{
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

                Assert.AreEqual(o1.Field1, o2.Field1);
                Assert.AreEqual(o1.Field2, o2.Field2);
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
                    var newObject = new Foo();
                }
                var speedNew = (DateTime.Now - started).TotalMilliseconds;

                var coldStartedFoo = ObjectFactory.Build<Foo>();

                started = DateTime.Now;
                count = ObjectsCount;
                while (count-- > 0)
                {
                    var newObject = ObjectFactory.Build<Foo>();
                }
                var speedFactory = (DateTime.Now - started).TotalMilliseconds;

                Assert.Less(speedFactory, speedNew * HowMuchTimesSlowerIsOk);
            }
        }


    }
    // ReSharper restore InconsistentNaming
}