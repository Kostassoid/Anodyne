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

namespace Kostassoid.Anodyne.Node.Specs
{
    using System;
    using Configuration;
    using FluentAssertions;
    using NUnit.Framework;
    using Windsor;

    // ReSharper disable InconsistentNaming
    public class NodeSpecs
    {

        public class TestNode : Node
        {
            public int ConfigurationCounter { get; set; }
            public int StartupCounter { get; set; }
            public int ShutdownCounter { get; set; }

            private void InternalConfigure(INodeConfiguration nodeConfiguration)
            {
                ConfigurationCounter++;
            }

            private void InternalStartup(INodeConfiguration nodeConfiguration)
            {
                StartupCounter++;
            }

            private void InternalShutdown(INodeConfiguration nodeConfiguration)
            {
                ShutdownCounter++;
            }

            public override void OnConfigure(INodeConfigurator nodeConfigurator)
            {
                nodeConfigurator.UseWindsorContainer();

                nodeConfigurator.ConfigureUsing(InternalConfigure);
                nodeConfigurator.OnStartupPerform(InternalStartup);
                nodeConfigurator.OnShutdownPerform(InternalShutdown);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class newly_created_node
        {
            private TestNode _node;

            [TestFixtureSetUp]
            public void SetUp()
            {
                _node = new TestNode();
            }

            [Test]
            public void should_be_in_undetermined_mode()
            {
                _node.Invoking(n => n.IsIn(RuntimeMode.Production)).ShouldThrow<InvalidOperationException>();
            }

            [Test]
            public void should_not_be_configured()
            {
                _node.ConfigurationCounter.Should().Be(0);
            }

            [Test]
            public void should_not_be_initialized()
            {
                _node.StartupCounter.Should().Be(0);
                _node.ShutdownCounter.Should().Be(0);
            }

            [Test]
            public void should_be_stopped()
            {
                _node.State.Should().Be(InstanceState.Stopped);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_starting_node
        {
            private TestNode _node;

            [TestFixtureSetUp]
            public void SetUp()
            {
                _node = new TestNode();
                _node.Start();
            }

            [Test]
            public void should_be_in_production_mode()
            {
                _node.IsIn(RuntimeMode.Production).Should().BeTrue();
            }

            [Test]
            public void should_be_configured()
            {
                _node.ConfigurationCounter.Should().Be(1);
            }

            [Test]
            public void should_be_initialized()
            {
                _node.StartupCounter.Should().Be(1);
                _node.ShutdownCounter.Should().Be(0);
            }

            [Test]
            public void should_be_started()
            {
                _node.State.Should().Be(InstanceState.Started);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_stopping_node
        {
            private TestNode _node;

            [TestFixtureSetUp]
            public void SetUp()
            {
                _node = new TestNode();
                _node.Start();
                _node.Shutdown();
            }

            [Test]
            public void should_be_configured()
            {
                _node.ConfigurationCounter.Should().Be(1);
            }

            [Test]
            public void should_be_shut_down()
            {
                _node.StartupCounter.Should().Be(1);
                _node.ShutdownCounter.Should().Be(1);
            }

            [Test]
            public void should_be_stopped()
            {
                _node.State.Should().Be(InstanceState.Stopped);
            }
        }

        [TestFixture]
        [Category("Unit")]
        public class when_restarting_node
        {
            private TestNode _node;

            [TestFixtureSetUp]
            public void SetUp()
            {
                _node = new TestNode();
                _node.Start();
                _node.Shutdown();
                _node.Start();
            }

            [Test]
            public void should_be_configured_once()
            {
                _node.ConfigurationCounter.Should().Be(1);
            }

            [Test]
            public void should_be_initialized()
            {
                _node.StartupCounter.Should().Be(2);
                _node.ShutdownCounter.Should().Be(1);
            }

            [Test]
            public void should_be_started()
            {
                _node.State.Should().Be(InstanceState.Started);
            }
        }


    }
    // ReSharper restore InconsistentNaming

}
