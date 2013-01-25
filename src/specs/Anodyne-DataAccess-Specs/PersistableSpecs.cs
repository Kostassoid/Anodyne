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

namespace Kostassoid.Anodyne.DataAccess.Specs
{
    using System;
    using Abstractions.DataAccess;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class PersistableSpecs
    {
        public class SimpleRoot : PersistableRoot
        {
            public int X { get; set; }
        }

        public class BaseRoot : PersistableRoot<int>
        {
            public int X { get; set; }

            public BaseRoot(int id, int x)
            {
                Id = id;
                X = x;
            }
        }

        public class SubRoot1 : BaseRoot
        {
            public SubRoot1(int id, int x, int y) : base(id, x)
            {
                Y = y;
            }

            public int Y { get; set; }
        }

        public class SubRoot2 : BaseRoot
        {
            public SubRoot2(int id, int x, int z) : base(id, x)
            {
                Z = z;
            }

            public int Z { get; set; }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_id_object_of_root_directly_derived_from_untyped_PersistableRoot
        {
            [Test]
            public void should_throw()
            {
                var root = new SimpleRoot();

                root.As<IPersistableRoot>().Invoking(e => { var x = e.IdObject; }).ShouldThrow<NotSupportedException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_roots_directly_derived_from_untyped_PersistableRoot
        {
            [Test]
            public void should_throw()
            {
                var root1 = new SimpleRoot();
                var root2 = new SimpleRoot();

                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                root1.Invoking(e => e.Equals(root2)).ShouldThrow<NotSupportedException>();
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_id_object_of_roots_derived_from_typed_PersistableRoot
        {
            [Test]
            public void should_return_correct_id_object()
            {
                var entity = new SubRoot1(666, 1, 2);

                entity.As<IPersistableRoot>().IdObject.Should().Be(666);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_roots_with_same_types_and_ids_but_different_data
        {
            [Test]
            public void should_be_equal()
            {
                var entity1 = new SubRoot1(666, 1, 2);
                var entity2 = new SubRoot1(666, 10, 20);

                entity1.Should().Be(entity2);
                entity1.GetHashCode().Should().Be(entity2.GetHashCode());
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_roots_with_same_ids_but_different_types
        {
            SubRoot1 _root1;
            SubRoot2 _root2;

            [SetUp]
            public void Given()
            {
                _root1 = new SubRoot1(666, 1, 2);
                _root2 = new SubRoot2(666, 1, 2);
            }

            [Test]
            public void should_not_be_equal()
            {
                _root1.Should().NotBe(_root2);
            }

            [Test]
            public void hash_codes_should_still_be_equal()
            {
                _root1.GetHashCode().Should().Be(_root2.GetHashCode());
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_roots_with_same_data_but_different_ids
        {
            [Test]
            public void should_not_be_equal()
            {
                var entity1 = new SubRoot1(666, 1, 2);
                var entity2 = new SubRoot1(333, 1, 2);

                entity1.Should().NotBe(entity2);
                entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
