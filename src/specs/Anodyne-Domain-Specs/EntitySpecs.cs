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

namespace Kostassoid.Anodyne.Domain.Specs
{
    using System;
    using Base;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class EntitySpecs
    {
        public class SimpleEntity : Entity
        {
            public int X { get; set; }
        }

        public class BaseEntity : Entity<int>
        {
            public int X { get; set; }

            public BaseEntity(int id, int x)
            {
                Id = id;
                X = x;
            }
        }

        public class SubEntity1 : BaseEntity
        {
            public SubEntity1(int id, int x, int y) : base(id, x)
            {
                Y = y;
            }

            public int Y { get; set; }
        }

        public class SubEntity2 : BaseEntity
        {
            public SubEntity2(int id, int x, int z) : base(id, x)
            {
                Z = z;
            }

            public int Z { get; set; }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_id_object_of_entites_directly_derived_from_untyped_Entity
        {
            [Test]
            public void should_throw()
            {
                var entity = new SimpleEntity();

                entity.As<IEntity>().Invoking(e => { var x = e.IdObject; }).ShouldThrow<NotSupportedException>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_entities_directly_derived_from_untyped_entity
        {
            [Test]
            public void should_throw()
            {
                var entity1 = new SimpleEntity();
                var entity2 = new SimpleEntity();

                // ReSharper disable ReturnValueOfPureMethodIsNotUsed
                entity1.Invoking(e => e.Equals(entity2)).ShouldThrow<NotSupportedException>();
                // ReSharper restore ReturnValueOfPureMethodIsNotUsed
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_getting_id_object_of_entites_derived_from_typed_Entity
        {
            [Test]
            public void should_return_correct_id_object()
            {
                var entity = new SubEntity1(666, 1, 2);

                entity.As<IEntity>().IdObject.Should().Be(666);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_entities_with_same_types_and_ids_but_different_data
        {
            [Test]
            public void should_be_equal()
            {
                var entity1 = new SubEntity1(666, 1, 2);
                var entity2 = new SubEntity1(666, 10, 20);

                entity1.Should().Be(entity2);
                entity1.GetHashCode().Should().Be(entity2.GetHashCode());
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_entities_with_same_ids_but_different_types
        {
            SubEntity1 entity1;
            SubEntity2 entity2;

            [SetUp]
            public void Given()
            {
                entity1 = new SubEntity1(666, 1, 2);
                entity2 = new SubEntity2(666, 1, 2);
            }

            [Test]
            public void should_not_be_equal()
            {
                entity1.Should().NotBe(entity2);
            }

            [Test]
            public void hash_codes_should_still_be_equal()
            {
                entity1.GetHashCode().Should().Be(entity2.GetHashCode());
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_comparing_entities_with_same_data_but_different_ids
        {
            [Test]
            public void should_not_be_equal()
            {
                var entity1 = new SubEntity1(666, 1, 2);
                var entity2 = new SubEntity1(333, 1, 2);

                entity1.Should().NotBe(entity2);
                entity1.GetHashCode().Should().NotBe(entity2.GetHashCode());
            }
        }
    }
    // ReSharper restore InconsistentNaming

}
