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
    using System.Linq;

    using Extentions;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class ExtentionsSpecs
    {
        class Something { }
        class SomethingElse { }

        interface ISomeTypeAccessor
        {
            int Int { get; }
            string String { get; }
            SomeNestedType Nested { get; }
            int[] Array { get; }
        }

        interface ISomeNestedTypeAccessor
        {
            string AnotherString { get; }
            SomeNestedType AnotherNested { get; }
        }

        // ReSharper disable MemberCanBePrivate.Local
        // ReSharper disable FieldCanBeMadeReadOnly.Local
        [Serializable]
        class SomeType : ISomeTypeAccessor
        {
            public int Int { get; protected set; }

            string ISomeTypeAccessor.String { get { return _string; } }
            SomeNestedType ISomeTypeAccessor.Nested { get { return Nested; } }
            int[] ISomeTypeAccessor.Array { get { return _array; } }

            private string _string;
            protected SomeNestedType Nested;
            internal int[] _array;

            public SomeType(string s, SomeNestedType nested, int[] array, int i)
            {
                _string = s;
                Nested = nested;
                _array = array;
                Int = i;
            }
        }

        [Serializable]
        class SomeNestedType : ISomeNestedTypeAccessor
        {
            public string AnotherString { get; set; }

            protected SomeNestedType AnotherNested;

            SomeNestedType ISomeNestedTypeAccessor.AnotherNested { get { return AnotherNested; } }

            public SomeNestedType(SomeNestedType anotherNested, string anotherString)
            {
                AnotherNested = anotherNested;
                AnotherString = anotherString;
            }
        }
        // ReSharper restore FieldCanBeMadeReadOnly.Local
        // ReSharper restore MemberCanBePrivate.Local


        [TestFixture]
        [Category("Unit")]
        public class when_deep_cloning_complex_type
        {
            private ISomeTypeAccessor _source;
            private ISomeTypeAccessor _cloned;
            
            [SetUp]
            public void Given()
            {
                var source = new SomeType("str", new SomeNestedType(new SomeNestedType(null, "str2"), "str3"), new[] { 342, 456, 23}, 666 );
                var cloned = source.DeepClone();

                _source = source;
                _cloned = cloned;
            }

            [Test]
            public void cloned_should_have_the_same_values()
            {
                _cloned.Int.Should().Be(_source.Int);
                _cloned.String.Should().Be(_source.String);
                _cloned.Nested.Should().Be(_source.Nested);
                _cloned.Array.Should().Equal(_source.Array);

                var sourceNested = (ISomeNestedTypeAccessor)_source.Nested;
                var clonedNested = (ISomeNestedTypeAccessor)_cloned.Nested;
                clonedNested.AnotherString.Should().Be(sourceNested.AnotherString);
                clonedNested.AnotherNested.Should().Be(sourceNested.AnotherNested);
            }

            [Test]
            public void cloned_should_not_have_the_same_hash_code()
            {
                _cloned.GetHashCode().Should().NotBe(_source.GetHashCode());
            }

            [Test]
            public void cloned_should_not_be_equal_to_the_source()
            {
                _cloned.Should().NotBe(_source);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_some_object_to_enumerable
        {
            [Test]
            public void should_return_enumerable_with_single_element()
            {
                var some = new Something();

                some.AsEnumerable().Should().HaveCount(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_some_object_to_enumerable_of_another_type
        {
            [Test]
            public void should_return_enumerable_with_no_elements()
            {
                var some = new Something();

                some.AsEnumerable<SomethingElse>().Should().BeEmpty();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_array_to_enumerable
        {
            [Test]
            public void should_still_return_one_element()
            {
                var some = new[] { new Something(), new Something(), new Something() };

                some.AsEnumerable().Should().HaveCount(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_converting_simple_value_to_enumerable
        {
            [Test]
            public void should_return_one_element()
            {
                const int some = 13;

                some.AsEnumerable().Should().HaveCount(1);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_using_for_each_over_enumerable
        {
            [Test]
            public void should_visit_every_element()
            {
                var some = new[] { 2, 3, 4, 5 };

                var result = new List<int>();

                some.ForEach(i => result.Add(i * i));

                result.ShouldBeEquivalentTo(new[] { 4, 9, 16, 25 });
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_selecting_many_elements_of_deep_hieararchy
        {
            class Node
            {
                public int Value { get; set; }
                public IList<Node> Children { get; set; }
            }

            [Test]
            public void should_return_all_elements()
            {
                var graph = new Node
                {
                    Children = new List<Node> {
                        new Node
                            {
                                Value = 30
                            },
                        new Node
                            {
                                Value = 20,
                                Children = new List<Node>
                                    {
                                        new Node
                                            {
                                                Value = 15
                                            },
                                        new Node
                                            {
                                                Value = 35
                                            }
                                    }
                            }
                    }
                };

                var sum = graph.Children.SelectDeep(g => g.Children).Sum(n => n.Value);

                sum.Should().Be(100);
            }
        }





    }
    // ReSharper restore InconsistentNaming

}
