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

namespace Kostassoid.Anodyne.Windsor.Specs
{
    using System;
    using Abstractions.Dependency.Registration;
    using Common.Reflection;
    using Abstractions.Dependency;
    using Anodyne.Specs.Shared;
    using Castle.Core;
    using FluentAssertions;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class WindsorAdapterSpecs
    {
        public class WindsorScenario
        {
            protected IContainer Container;

            public WindsorScenario()
            {
                IntegrationContext.Init();

                Container = IntegrationContext.System.Configuration.Container;
            }
        }

        public interface IBoo : IDisposable
        {
            bool IsDisposed { get; }
        }

        public class Boo : IBoo
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

        public class AnotherBoo : IBoo
        {
            public bool IsDisposed { get; private set; }
            public void Dispose()
            {
                IsDisposed = true;
            }
        }

		public class Xoo
		{
			public int SomeNumber { get; private set; }
			public string SomeString { get; set; }
			public IBoo SomeBoo { get; private set; }
			public IBoo AnyBoo { get; private set; }

			public Xoo(int someNumber, IBoo someBoo, IBoo anyBoo)
			{
				SomeNumber = someNumber;
				SomeBoo = someBoo;
				AnyBoo = anyBoo;
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_registering_single_service_using_single_type : WindsorScenario
		{
			[Test]
			public void should_be_available_from_container_by_interface()
			{
				Container.Put(Binding.For<IBoo>().Use<Boo>());

				Container.Has<IBoo>().Should().BeTrue();
				Container.Get<IBoo>().Should().BeOfType<Boo>();

				Container.Has(typeof(IBoo)).Should().BeTrue();
				Container.Get(typeof(IBoo)).Should().BeOfType<Boo>();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_resolving_objects_registered_with_valid_name : WindsorScenario
		{
			[Test]
			public void should_resolve_using_name()
			{
				Container.Put(Binding.For<IBoo>().Use<Boo>());
				Container.Put(Binding.For<IBoo>().Use<AnotherBoo>().Named("Special"));

				Container.Has<IBoo>().Should().BeTrue();
				Container.Has("Special").Should().BeTrue();
				Container.Get<IBoo>().Should().BeOfType<Boo>();
				Container.Get<IBoo>("Special").Should().BeOfType<AnotherBoo>();

				Container.Has(typeof(IBoo)).Should().BeTrue();
				Container.Get(typeof(IBoo)).Should().BeOfType<Boo>();
				Container.Get(typeof(IBoo), "Special").Should().BeOfType<AnotherBoo>();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_resolving_objects_using_invalid_name : WindsorScenario
		{
			[Test]
			public void should_throw()
			{
				Container.Put(Binding.For<IBoo>().Use<AnotherBoo>().Named("Special"));

				Container.Has("Ololo").Should().BeFalse();

				Container.Invoking(c => c.Get<IBoo>("Ololo")).ShouldThrow<Castle.MicroKernel.ComponentNotFoundException>();
				Container.Invoking(c => c.Get(typeof(IBoo), "Ololo")).ShouldThrow<Castle.MicroKernel.ComponentNotFoundException>();
			}
		}

		[TestFixture]
        [Category("Unit")]
        public class when_registering_using_transient_lifestyle : WindsorScenario
        {
            [Test]
            public void should_be_registered_as_transient()
            {
                Container.Put(Binding.For<IBoo>().Use<Boo>().With(Lifestyle.Transient));

                Container.OnNative(c => c.Kernel.GetHandler(typeof(IBoo))
                                         .ComponentModel.LifestyleType.Should()
                                         .Be(LifestyleType.Transient));
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_releasing_object_with_transient_lifestyle : WindsorScenario
        {
            [Test]
            public void should_release()
            {
                Container.Put(Binding.For<IBoo>().Use<Boo>().With(Lifestyle.Transient));

                var boo = Container.Get<IBoo>();

                Container.Release(boo);

                boo.IsDisposed.Should().BeTrue();
            }
        }

		[TestFixture]
		[Category("Unit")]
		public class when_releasing_object_with_unmanaged_lifestyle : WindsorScenario
		{
			[Test]
			public void should_no_nothing()
			{
				Container.Put(Binding.For<IBoo>().Use<Boo>().With(Lifestyle.Unmanaged));

				var boo = Container.Get<IBoo>();

				Container.Release(boo);

				boo.IsDisposed.Should().BeFalse();
			}
		}

		[TestFixture]
		[Category("Unit")]
		public class when_resolving_object_registered_as_is : WindsorScenario
		{
			[Test]
			public void should_resolve_by_type()
			{
				Container.Put(Binding.For<Boo>());

				var boo = Container.Get<Boo>();
				boo.Should().NotBeNull();
			}
		}

		[TestFixture]
        [Category("Unit")]
        public class when_registering_multiple_types_as_is : WindsorScenario
        {
            [Test]
            public void each_type_should_be_available_from_container_by_its_type()
            {
                Container.Put(Binding.Use(AllTypes.BasedOn<IBoo>()));

                Container.Has<IBoo>().Should().BeFalse();
                Container.Has<Boo>().Should().BeTrue();
                Container.Has<AnotherBoo>().Should().BeTrue();
                Container.Get<AnotherBoo>().Should().BeOfType<AnotherBoo>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_registering_multiple_types_as_base_interface : WindsorScenario
        {
            [Test]
            public void each_type_should_be_available_from_container_by_base_interface()
            {
                Container.Put(Binding.Use(AllTypes.BasedOn<IBoo>()).As<IBoo>());

                Container.Has<IBoo>().Should().BeTrue();
                Container.Has<Boo>().Should().BeFalse();
                Container.Has<AnotherBoo>().Should().BeFalse();
                Container.Get<IBoo>().Should().BeOfType<Boo>();

                Container.GetAll<IBoo>().Should().HaveCount(2);
				Container.GetAll(typeof(IBoo)).Should().HaveCount(2);
			}
        }

        [TestFixture]
        [Category("Unit")]
        public class when_resolving_single_non_registered_component : WindsorScenario
        {
            [Test]
            public void should_throw()
            {
                Container.Invoking(c => c.Get<Boo>()).ShouldThrow<Exception>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_resolving_all_components_for_non_registered_service : WindsorScenario
        {
            [Test]
            public void should_throw()
            {
                Container.GetAll<Boo>().Should().HaveCount(0);
            }
        }

		[TestFixture]
		[Category("Unit")]
		public class when_resolving_object_with_dependencies_from_app_settings : WindsorScenario
		{
			[Test]
			public void should_resolve_from_app_settings()
			{
				var boo = new Boo();

				Container.Put(Binding.For<AnotherBoo>().Use<AnotherBoo>());
				Container.Put(Binding.For<IBoo>().Use<Boo>());
				Container.Put(Binding.For<IBoo>().UseInstance(boo).Named("SpecialBoo"));
				Container.Put(Binding.For<Xoo>().Use<Xoo>());

				var xoo = Container.Get<Xoo>();

				xoo.Should().NotBeNull();
				xoo.SomeNumber.Should().Be(666);
				xoo.SomeString.Should().Be("Hail Satan!");
				xoo.SomeBoo.Should().Be(boo);
				xoo.AnyBoo.Should().BeOfType<AnotherBoo>();
			}
		}


    }
    // ReSharper restore InconsistentNaming

}
