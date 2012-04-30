﻿// Copyright 2011-2012 Anodyne.
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
    using Anodyne.Specs.Shared;
    using Common.Tools;
    using Domain.Base;
    using Domain.Events;
    using NUnit.Framework;

    // ReSharper disable InconsistentNaming
    public class UnitOfWorkSpecs
    {
        public class UnitOfWorkScenario
        {
            public UnitOfWorkScenario()
            {
                IntegrationContext.Init();
            }
        }

        public class TestRoot : AggregateRoot<Guid>
        {
            protected TestRoot()
            {
                Id = SeqGuid.NewGuid();
            }

            public static TestRoot Create()
            {
                var root = new TestRoot();
                root.Apply(new TestRootCreated(root));
                return root;
            }

            protected void OnCreated(TestRootCreated @event)
            {
                
            }
            
        }

        public class TestRootCreated : AggregateEvent<TestRoot,EmptyEventData>
        {
            public TestRootCreated(TestRoot aggregate) : base(aggregate, new EmptyEventData())
            {
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_working_with_root_from_one_unit_of_work : UnitOfWorkScenario
        {
            [Test]
            public void should_save_root_and_update_version()
            {
                Guid rootId;
                using (var uow = new UnitOfWork())
                {
                    rootId = TestRoot.Create().Id;
                }

                using (var uow = new UnitOfWork())
                {
                    var root = uow.Query<TestRoot>().FindBy(rootId);

                    Assert.That(root.IsSome, Is.True);
                    Assert.That(root.Value.Id, Is.EqualTo(rootId));
                    Assert.That(root.Value.Version, Is.EqualTo(1));
                }
            }
        }

    }
    // ReSharper restore InconsistentNaming

}
