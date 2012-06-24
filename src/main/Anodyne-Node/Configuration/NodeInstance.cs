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

namespace Kostassoid.Anodyne.Node.Configuration
{
    using Anodyne.DataAccess;
    using Anodyne.DataAccess.Policy;
    using Common.Tools;
    using Dependency;
    using Logging;
    using Subsystem;
    using Wcf;
    using System;

    public class NodeInstance : IConfiguration, IConfigurationBuilder, INodeInstance
    {
        private RuntimeMode _runtimeMode;
        RuntimeMode INodeInstance.RuntimeMode { get { return _runtimeMode; } }

        private IContainer _container;
        IContainer INodeInstance.Container { get { return _container; } }

        private ILoggerAdapter _loggerAdapter;
        ILoggerAdapter INodeInstance.Logger { get { return _loggerAdapter; } }

        IDataAccessProvider INodeInstance.DataAccess { get { return _container.Get<IDataAccessProvider>(); } }

        private IWcfServicePublisher _wcfServicePublisher;
        IWcfServicePublisher INodeInstance.WcfServicePublisher { get { return _wcfServicePublisher; } }

        public NodeInstance()
        {
            _runtimeMode = RuntimeMode.Production;

            (this as IConfigurationBuilder).SetLoggerAdapter(new NullLoggerAdapter());
        }

        bool IConfigurationBuilder.IsValid
        {
            get { return _container != null; }
        }

        void IConfigurationBuilder.SetContainerAdapter(IContainer container)
        {
            _container = container;
        }

        void IConfigurationBuilder.SetLoggerAdapter(ILoggerAdapter loggerAdapter)
        {
            _loggerAdapter = loggerAdapter;
            LogManager.Adapter = loggerAdapter;
        }

        void IConfigurationBuilder.SetWcfServiceProvider(IWcfServicePublisher wcfServicePublisher)
        {
            _wcfServicePublisher = wcfServicePublisher;
        }

        public void RunIn(RuntimeMode runtimeMode)
        {
            _runtimeMode = runtimeMode;
        }

        public void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction)
        {
            var dataPolicy = new DataAccessPolicy();
            policyAction(dataPolicy);

            UnitOfWork.EnforcePolicy(dataPolicy);
        }

        private bool ConContinue(ConfigurationPredicate when)
        {
            return when == null || when(this);
        }

        public void ConfigureUsing<TConfiguration>(ConfigurationPredicate when) where TConfiguration : IConfigurationAction
        {
            if (!ConContinue(when)) return;
            Activator.CreateInstance<TConfiguration>().OnConfigure(this);
        }

        public void ConfigureUsing(Action<INodeInstance> configurationAction, ConfigurationPredicate when)
        {
            if (!ConContinue(when)) return;
            configurationAction(this);
        }

        private string GetTypeUniqueName<T>(string prefix)
        {
            return prefix + "-" + typeof (T).Name;
        }

        public void OnStartupPerform<TStartup>(ConfigurationPredicate when) where TStartup : IStartupAction
        {
            if (!ConContinue(when)) return;
            _container.For<IStartupAction>().Use<TStartup>(Lifestyle.Singleton, GetTypeUniqueName<TStartup>("Startup"));
        }

        public void OnStartupPerform(Action<INodeInstance> startupAction, ConfigurationPredicate when)
        {
            if (!ConContinue(when)) return;
            _container.For<IStartupAction>().Use(() => new StartupActionWrapper(startupAction), Lifestyle.Singleton, "Startup-" + SeqGuid.NewGuid());
        }

        public void OnShutdownPerform<TShutdown>(ConfigurationPredicate when) where TShutdown : IShutdownAction
        {
            if (!ConContinue(when)) return;
            _container.For<IShutdownAction>().Use<TShutdown>(Lifestyle.Singleton, GetTypeUniqueName<TShutdown>("Shutdown"));
        }

        public void OnShutdownPerform(Action<INodeInstance> shutdownAction, ConfigurationPredicate when)
        {
            if (!ConContinue(when)) return;
            _container.For<IShutdownAction>().Use(() => new ShutdownActionWrapper(shutdownAction), Lifestyle.Singleton, "Shutdown-" + SeqGuid.NewGuid());
        }

        public void RegisterSubsystem<TSubsystem>() where TSubsystem : ISubsystem
        {
            _container.For<ISubsystem>().Use<TSubsystem>(Lifestyle.Singleton);
        }
    }
}