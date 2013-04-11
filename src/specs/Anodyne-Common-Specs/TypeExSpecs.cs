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
	using System.Collections.Generic;
	using System.Linq;
	using FluentAssertions;
    using NUnit.Framework;
	using Reflection;

	// ReSharper disable InconsistentNaming
    public class TypeExSpecs
    {
        public class SomeGeneric<T>
        {
			public T Value { get; set; }

			public IList<string> Log = new List<string>();

			public void OnBase(BaseParam param)
			{
				Log.Add("b");
			}

			public void OnWtf(BaseParam param, int addOn)
			{ }

			public int GetWtf(BaseParam param)
			{
				return 13;
			}
        }

		public class AnotherGeneric<T> : SomeGeneric<T>
		{
		}

		public class Something : SomeGeneric<string>
		{
			protected void OnConcrete1(ConcreteParam1 param)
			{
				Log.Add("1");
			}

			private void OnConcrete2(ConcreteParam2 param)
			{
				Log.Add("2");
			}
		}

		public class Anotherthing : AnotherGeneric<string>
		{ }

		public class BaseParam
		{ }

		public class ConcreteParam1 : BaseParam
		{ }

		public class ConcreteParam2 : BaseParam
		{ }


		[TestFixture]
		[Category("Unit")]
		public class when_checking_if_generic_type_is_raw_generic
		{
			[Test]
			public void should_match_only_for_direct_parent_generics()
			{
				var genericType = typeof(SomeGeneric<string>);
				genericType.IsRawGeneric(typeof(SomeGeneric<>)).Should().BeTrue();
				genericType.IsRawGeneric(typeof(SomeGeneric<string>)).Should().BeFalse();
				genericType.IsRawGeneric(typeof(AnotherGeneric<>)).Should().BeFalse();
				genericType.IsRawGeneric(typeof(Something)).Should().BeFalse();

				genericType = typeof(AnotherGeneric<string>);
				genericType.IsRawGeneric(typeof(SomeGeneric<>)).Should().BeFalse();
				genericType.IsRawGeneric(typeof(SomeGeneric<string>)).Should().BeFalse();
				genericType.IsRawGeneric(typeof(AnotherGeneric<>)).Should().BeTrue();
				genericType.IsRawGeneric(typeof(Something)).Should().BeFalse();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_checking_if_concrete_non_generic_type_is_raw_generic
		{
			[Test]
			public void should_never_match()
			{
				var concreteType = typeof(Something);
				concreteType.IsRawGeneric(typeof(SomeGeneric<>)).Should().BeFalse();

				concreteType = typeof(Anotherthing);
				concreteType.IsRawGeneric(typeof(AnotherGeneric<>)).Should().BeFalse();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_checking_if_concrete_non_generic_type_is_subclass_of_raw_generic
		{
			[Test]
			public void should_match_for_any_generic_parents()
			{
				var concreteType = typeof(Something);
				concreteType.IsSubclassOfRawGeneric(typeof(SomeGeneric<>)).Should().BeTrue();
				concreteType.IsSubclassOfRawGeneric(typeof(AnotherGeneric<>)).Should().BeFalse();

				concreteType = typeof(Anotherthing);
				concreteType.IsSubclassOfRawGeneric(typeof(SomeGeneric<>)).Should().BeTrue();
				concreteType.IsSubclassOfRawGeneric(typeof(AnotherGeneric<>)).Should().BeTrue();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_searching_for_handler_methods_using_polymorphic_matching
		{
			[Test]
			public void should_return_methods_accepting_param_and_assignable_types()
			{
				var methods = typeof(Anotherthing).FindMethodHandlers(typeof(ConcreteParam1), true).ToList();
				methods.Should().HaveCount(1);
				methods.First().Name.Should().Be("OnBase");

				methods = typeof(Something).FindMethodHandlers(typeof(ConcreteParam1), true).ToList();
				methods.Should().HaveCount(2);
				methods.Should().Contain(m => m.Name == "OnBase");
				methods.Should().Contain(m => m.Name == "OnConcrete1");
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_searching_for_handler_methods_using_non_polymorphic_matching
		{
			[Test]
			public void should_return_methods_accepting_concrete_param_type_only()
			{
				var methods = typeof(Anotherthing).FindMethodHandlers(typeof(ConcreteParam1), false).ToList();
				methods.Should().BeEmpty();

				methods = typeof(Something).FindMethodHandlers(typeof(ConcreteParam1), false).ToList();
				methods.Should().HaveCount(1);
				methods.Should().Contain(m => m.Name == "OnConcrete1");

				methods = typeof(Something).FindMethodHandlers(typeof(ConcreteParam2), false).ToList();
				methods.Should().HaveCount(1);
				methods.Should().Contain(m => m.Name == "OnConcrete2");
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_building_handler_delegate_using_correct_method_info
		{
			[Test]
			public void should_return_handler_for_specified_method()
			{
				var handlers = typeof(Something)
					.FindMethodHandlers(typeof(ConcreteParam1), true)
					.Select(m => TypeEx.BuildMethodHandler(m, typeof(ConcreteParam1)))
					.ToList();

				handlers.Should().NotContainNulls();

				var testSubject = new Something();
				handlers.ForEach(h => h(testSubject, new ConcreteParam1()));

				testSubject.Log.Should().BeEquivalentTo(new object[] { "b", "1" });
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_calling_handler_delegate_using_incompatible_type
		{
			[Test]
			public void should_throw_invalid_cast_exception()
			{
				var handler = typeof(Something)
					.FindMethodHandlers(typeof(ConcreteParam1), false)
					.Select(m => TypeEx.BuildMethodHandler(m, typeof(ConcreteParam1)))
					.First();

				var testSubject = new Something();

				handler.Invoking(h => h(testSubject, new BaseParam())).ShouldThrow<InvalidCastException>();

				handler.Invoking(h => h(testSubject, "ololo")).ShouldThrow<InvalidCastException>();

				handler.Invoking(h => h(testSubject, 13)).ShouldThrow<InvalidCastException>();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_calling_handler_delegate_using_null_param
		{
			[Test]
			public void should_not_throw()
			{
				var handler = typeof(Something)
					.FindMethodHandlers(typeof(ConcreteParam1), false)
					.Select(m => TypeEx.BuildMethodHandler(m, typeof(ConcreteParam1)))
					.First();

				var testSubject = new Something();

				handler.Invoking(h => h(testSubject, null)).ShouldNotThrow();

				testSubject.Log.Should().BeEquivalentTo(new object[] { "1" });
			}
		}



	}
    // ReSharper restore InconsistentNaming
}
