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

namespace Kostassoid.Anodyne.Windsor.Specs
{
    using System;
    using Anodyne.Specs.Shared;
    using Castle.Core;
    using FluentAssertions;
    using NUnit.Framework;
    using Node.Dependency;

    // ReSharper disable InconsistentNaming
    public class WindsorAdaptorSpecs
    {
        public class WindsorScenario
        {
            protected IContainer Container;

            public WindsorScenario()
            {
                IntegrationContext.Init();

                Container = IntegrationContext.System.Instance.Container;
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

        [TestFixture]
        [Category("Unit")]
        public class when_registering_single_service_using_single_type : WindsorScenario
        {
            [Test]
            public void should_be_available_from_container_by_interface()
            {
                Container.For<IBoo>().Use<Boo>();

                Container.Has<IBoo>().Should().BeTrue();
                Container.Get<IBoo>().Should().BeOfType<Boo>();
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_registering_using_transient_lifestyle : WindsorScenario
        {
            [Test]
            public void should_be_registered_as_transient()
            {
                Container.For<IBoo>().Use<Boo>(Lifestyle.Transient);

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
                Container.For<IBoo>().Use<Boo>(Lifestyle.Transient);

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
                Container.For<IBoo>().Use<Boo>(Lifestyle.Unmanaged);

                var boo = Container.Get<IBoo>();

                Container.Release(boo);

                boo.IsDisposed.Should().BeFalse();
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
