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

namespace Kostassoid.Anodyne.Node
{
    using System;
    using System.Collections.Generic;
    using Common.Extentions;
    using Configuration;
    using Subsystem;

    public abstract class Node
    {
        public INodeConfiguration Configuration { get; private set; }

        public InstanceState State { get; private set; }

        public bool CanBeStarted { get { return State != InstanceState.Started; } }
        public bool CanBeStopped { get { return State != InstanceState.Stopped; } }
        public bool IsConfigured { get; private set; }

        public abstract void OnConfigure(INodeConfigurator nodeConfigurator);
        public virtual void OnStart() {}
        public virtual void OnShutdown() {}

        public event Action<INodeConfigurator> BeforeConfiguration = s => { };
        public event Action<INodeConfigurator> AfterConfiguration = s => { };
        public event Action<Node> Started = s => { };
        public event Action<Node> Stopped = s => { };
        public event Action<Node> Starting = s => { };
        public event Action<Node> Stopping = s => { };

        public bool IsIn(RuntimeMode runtimeMode)
        {
            RequireNodeIsConfigured();
            return Configuration.RuntimeMode == runtimeMode;
        }

        private void RequireNodeIsConfigured()
        {
            if (!IsConfigured)
                throw new InvalidOperationException("Node is not configured. Use Start() to configure and initialize.");
        }

        private IList<ISubsystem> _subsystems = new List<ISubsystem>();

        private void EnsureNodeIsConfigured()
        {
            if (IsConfigured) return;

            var configurationBuilder = new ConfigurationBuilder();

            BeforeConfiguration(configurationBuilder);
            OnConfigure(configurationBuilder);
            AfterConfiguration(configurationBuilder);

            Configuration = configurationBuilder.Build();
            IsConfigured = true;
        }

        public void Start()
        {
            if (!CanBeStarted) return;

            EnsureNodeIsConfigured();

            Starting(this);

            Configuration.Container.GetAll<IStartupAction>().ForEach(b => b.OnStartup(Configuration));

            _subsystems = Configuration.Container.GetAll<ISubsystem>();
            _subsystems.ForEach(s => s.Start());

            OnStart();

            State = InstanceState.Started;

            Started(this);
        }

        public void Shutdown()
        {
            if (!CanBeStopped) return;

            Stopping(this);

            Configuration.Container.GetAll<IShutdownAction>().ForEach(b => b.OnShutdown(Configuration));

            OnShutdown();

            _subsystems.ForEach(s => s.Stop());

            State = InstanceState.Stopped;

            Stopped(this);
        }

    }
}