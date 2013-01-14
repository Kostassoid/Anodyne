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

    public delegate bool ConfigurationPredicate(INodeInstance instance);

    public interface IConfiguration : ISyntax
    {
        void RunIn(RuntimeMode runtimeMode);

        void DefineSystemNamespaceAs(string systemNamespace);

        void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction);

        void ConfigureUsing<TConfiguration>(ConfigurationPredicate when = null) where TConfiguration : IConfigurationAction;
        void ConfigureUsing(Action<INodeInstance> configurationAction, ConfigurationPredicate when = null);

        void OnStartupPerform<TStartup>(ConfigurationPredicate when = null) where TStartup : IStartupAction;
        void OnStartupPerform(Action<INodeInstance> startupAction, ConfigurationPredicate when = null);

        void OnShutdownPerform<TShutdown>(ConfigurationPredicate when = null) where TShutdown : IShutdownAction;
        void OnShutdownPerform(Action<INodeInstance> shutdownAction, ConfigurationPredicate when = null);

        void RegisterSubsystem<TSubsystem>() where TSubsystem : ISubsystem;
    }
}