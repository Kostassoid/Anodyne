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

using Kostassoid.Anodyne.Domain.DataAccess.Policy;

namespace Kostassoid.Anodyne.Node.Configuration
{
    using Common;
    using System;
    using Subsystem;

    /// <summary>
    /// Allows to check special configuration-time conditions for action to run.
    /// </summary>
    /// <param name="configuration">Node configuration.</param>
    /// <returns></returns>
    public delegate bool ConfigurationPredicate(INodeConfiguration configuration);

    /// <summary>
    /// Node configuration builder.
    /// </summary>
    public interface INodeConfigurator : ISyntax
    {
        /// <summary>
        /// Current Node configuration.
        /// </summary>
        INodeConfiguration Configuration { get; }

        /// <summary>
        /// Define Node runtime mode.
        /// </summary>
        /// <param name="runtimeMode"></param>
        void RunIn(RuntimeMode runtimeMode);

        /// <summary>
        /// Override system (project) namespace for resolving system-specific types.
        /// </summary>
        /// <param name="systemNamespace">System namespace.</param>
        void DefineSystemNamespaceAs(string systemNamespace);

        /// <summary>
        /// Define default Data Action policy for working with domain entities (UnitOfWork).
        /// </summary>
        /// <param name="policyAction">Data access policy configurator.</param>
        void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction);

        /// <summary>
        /// Add configuration action, which should be performed once on Node startup.
        /// </summary>
        /// <typeparam name="TConfiguration">Concrete ConfigurationAction implementation.</typeparam>
        /// <param name="when">Optional configuration-time predicate.</param>
        void ConfigureUsing<TConfiguration>(ConfigurationPredicate when = null) where TConfiguration : IConfigurationAction;

        /// <summary>
        /// Add configuration action, which should be performed once on Node startup.
        /// </summary>
        /// <param name="configurationAction">Delegate to be performed upon Node configuration.</param>
        /// <param name="when">Optional configuration-time predicate.</param>
        void ConfigureUsing(Action<INodeConfiguration> configurationAction, ConfigurationPredicate when = null);

        /// <summary>
        /// Add startup action, which should be performed every time on Node startup.
        /// </summary>
        /// <typeparam name="TStartup">Concrete StartupAction implementation.</typeparam>
        /// <param name="when">Optional configuration-time predicate.</param>
        void OnStartupPerform<TStartup>(ConfigurationPredicate when = null) where TStartup : IStartupAction;
        /// <summary>
        /// Add startup action, which should be performed every time on Node startup.
        /// </summary>
        /// <param name="startupAction">Delegate to be performed upon Node startup.</param>
        /// <param name="when">Optional configuration-time predicate.</param>
        void OnStartupPerform(Action<INodeConfiguration> startupAction, ConfigurationPredicate when = null);

        /// <summary>
        /// Add shutdown action, which should be performed every time on Node shutdown.
        /// </summary>
        /// <typeparam name="TShutdown">Concrete ShutdownAction implementation.</typeparam>
        /// <param name="when">Optional configuration-time predicate.</param>
        void OnShutdownPerform<TShutdown>(ConfigurationPredicate when = null) where TShutdown : IShutdownAction;
        /// <summary>
        /// Add shutdown action, which should be performed every time on Node shutdown.
        /// </summary>
        /// <param name="shutdownAction">Delegate to be performed upon Node shutdown.</param>
        /// <param name="when">Optional configuration-time predicate.</param>
        void OnShutdownPerform(Action<INodeConfiguration> shutdownAction, ConfigurationPredicate when = null);

        /// <summary>
        /// Register high-level Subsystem.
        /// </summary>
        /// <typeparam name="TSubsystem">Subsystem implementation.</typeparam>
        void RegisterSubsystem<TSubsystem>() where TSubsystem : ISubsystem;
    }
}