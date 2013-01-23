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
    using Configuration;

    /// <summary>
    /// Represents Node in distributed system. Should be one instance per AppDomain.
    /// </summary>
    public interface INode
    {
        /// <summary>
        /// Node instance configuration. Available after Node has been configured (normally after first Start).
        /// </summary>
        INodeConfiguration Configuration { get; }

        /// <summary>
        /// Node instance state.
        /// </summary>
        InstanceState State { get; }

        /// <summary>
        /// Node can be started using Start().
        /// </summary>
        bool CanBeStarted { get; }
        /// <summary>
        /// Node can be stopped using Shutdown().
        /// </summary>
        bool CanBeStopped { get; }
        /// <summary>
        /// Node instance has been configured successfully.
        /// </summary>
        bool IsConfigured { get; }

        /// <summary>
        /// Called when Node needs to be configured.
        /// </summary>
        /// <param name="c">Configuration builder</param>
        void OnConfigure(INodeConfigurator c);

        /// <summary>
        /// Called as the last step in startup process for any last minute startup actions.
        /// </summary>
        void OnStart();

        /// <summary>
        /// Called as the first step in shutdown process, usually to make special preparations before shutdown.
        /// </summary>
        void OnShutdown();

        /// <summary>
        /// Notifies when Node configuration builder is ready, just before actual Configuration is ready. Allows for any last minute configuration actions.
        /// </summary>
        event Action<INodeConfigurator> ConfigurationIsReady;
        /// <summary>
        /// Notifies when Node has been successfully started.
        /// </summary>
        event Action<Node> Started;
        /// <summary>
        /// Notifies when Node has been successfully shut down.
        /// </summary>
        event Action<Node> Stopped;
        /// <summary>
        /// Notifies before Node starts.
        /// </summary>
        event Action<Node> Starting;
        /// <summary>
        /// Notifies before Node shuts down.
        /// </summary>
        event Action<Node> Stopping;

        /// <summary>
        /// Check if Node instance is in specified runtime mode.
        /// </summary>
        /// <param name="runtimeMode">Runtime mode.</param>
        /// <returns></returns>
        bool IsIn(RuntimeMode runtimeMode);

        /// <summary>
        /// Configure Node instance (if needed) and start it, performing all OnStartup actions.
        /// </summary>
        void Start();

        /// <summary>
        /// Shutdown Node instance, performing all OnShutdown actions.
        /// </summary>
        void Shutdown();
    }
}