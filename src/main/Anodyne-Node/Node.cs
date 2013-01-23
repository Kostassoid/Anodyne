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
    using Configuration.Internal;
    using Subsystem;

    /// <summary>
    /// Represents Node in distributed system. Should be one instance per AppDomain.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Node instance configuration. Available after Node has been configured (normally after first Start).
        /// </summary>
        public INodeConfiguration Configuration { get; private set; }

        /// <summary>
        /// Node instance state.
        /// </summary>
        public InstanceState State { get; private set; }

        /// <summary>
        /// Node can be started using Start().
        /// </summary>
        public bool CanBeStarted { get { return State != InstanceState.Started; } }
        /// <summary>
        /// Node can be stopped using Shutdown().
        /// </summary>
        public bool CanBeStopped { get { return State != InstanceState.Stopped; } }
        /// <summary>
        /// Node instance has been configured successfully.
        /// </summary>
        public bool IsConfigured { get; private set; }

        /// <summary>
        /// Called when Node needs to be configured.
        /// </summary>
        /// <param name="c">Configuration builder</param>
        public abstract void OnConfigure(INodeConfigurator c);
        /// <summary>
        /// Called as the last step in startup process for any last minute startup actions.
        /// </summary>
        public virtual void OnStart() {}
        /// <summary>
        /// Called as the first step in shutdown process, usually to make special preparations before shutdown.
        /// </summary>
        public virtual void OnShutdown() { }

        /// <summary>
        /// Notifies when Node configuration builder is ready, just before actual Configuration is ready. Allows for any last minute configuration actions.
        /// </summary>
        public event Action<INodeConfigurator> ConfigurationIsReady = c => { };
        /// <summary>
        /// Notifies when Node has been successfully started.
        /// </summary>
        public event Action<Node> Started = s => { };
        /// <summary>
        /// Notifies when Node has been successfully shut down.
        /// </summary>
        public event Action<Node> Stopped = s => { };
        /// <summary>
        /// Notifies before Node starts.
        /// </summary>
        public event Action<Node> Starting = s => { };
        /// <summary>
        /// Notifies before Node shuts down.
        /// </summary>
        public event Action<Node> Stopping = s => { };

        /// <summary>
        /// Check if Node instance is in specified runtime mode.
        /// </summary>
        /// <param name="runtimeMode">Runtime mode.</param>
        /// <returns></returns>
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

            OnConfigure(configurationBuilder);
            ConfigurationIsReady(configurationBuilder);
            Configuration = configurationBuilder.Build();

            Configuration.Container.GetAll<IConfigurationAction>().ForEach(b => b.OnConfigure(Configuration));

            IsConfigured = true;
        }

        /// <summary>
        /// Configure Node instance (if needed) and start it, performing all OnStartup actions.
        /// </summary>
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

        /// <summary>
        /// Shutdown Node instance, performing all OnShutdown actions.
        /// </summary>
        public void Shutdown()
        {
            if (!CanBeStopped) return;

            Stopping(this);

            OnShutdown();

            _subsystems.ForEach(s => s.Stop());

            Configuration.Container.GetAll<IShutdownAction>().ForEach(b => b.OnShutdown(Configuration));

            State = InstanceState.Stopped;

            Stopped(this);
        }

    }
}