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

using Kostassoid.Anodyne.Domain.DataAccess;
using Kostassoid.Anodyne.Domain.DataAccess.Policy;
using Kostassoid.Anodyne.Node.Dependency.Registration;

namespace Kostassoid.Anodyne.Node.Configuration
{
    using System.Reflection;
    using Anodyne.DataAccess;
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

        private IWcfAdapter _wcfAdapter;
        IWcfAdapter INodeInstance.WcfAdapter { get { return _wcfAdapter; } }

        private string _systemNamespace;
        string INodeInstance.SystemNamespace { get { return _systemNamespace; } }

        public event Action OnContainerReady = () => { };

        public NodeInstance()
        {
            _runtimeMode = RuntimeMode.Production;
            _systemNamespace = DetectSystemNamespace();

            (this as IConfigurationBuilder).SetLoggerAdapter(new NullLoggerAdapter());
        }

        private static string DetectSystemNamespace()
        {
            var assembly = Assembly.GetEntryAssembly(); // could be null during tests run
            return assembly != null ? assembly.FullName.Substring(0, assembly.FullName.IndexOfAny(new[] { '.', '-', '_' })) : "";
        }

        bool IConfigurationBuilder.IsValid
        {
            get { return _container != null; }
        }

        void IConfigurationBuilder.SetContainerAdapter(IContainer container)
        {
            if (_container != null)
                throw new InvalidOperationException("Container adapter is already set to " + _container.GetType().Name);

            _container = container;

            OnContainerReady();
        }

        void IConfigurationBuilder.SetLoggerAdapter(ILoggerAdapter loggerAdapter)
        {
            _loggerAdapter = loggerAdapter;
            LogManager.Adapter = loggerAdapter;
        }

        void IConfigurationBuilder.SetWcfServiceProvider(IWcfAdapter wcfAdapter)
        {
            _wcfAdapter = wcfAdapter;
        }

        public void RunIn(RuntimeMode runtimeMode)
        {
            _runtimeMode = runtimeMode;
        }

        public void DefineSystemNamespaceAs(string systemNamespace)
        {
            _systemNamespace = systemNamespace;
        }

        public void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction)
        {
            var dataPolicy = new DataAccessPolicy();
            policyAction(dataPolicy);

            UnitOfWork.EnforcePolicy(dataPolicy);
        }

        private bool CanContinue(ConfigurationPredicate when)
        {
            return when == null || when(this);
        }

        public void ConfigureUsing<TConfiguration>(ConfigurationPredicate when) where TConfiguration : IConfigurationAction
        {
            if (!CanContinue(when)) return;
            Activator.CreateInstance<TConfiguration>().OnConfigure(this);
        }

        public void ConfigureUsing(Action<INodeInstance> configurationAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            configurationAction(this);
        }

        private static string GetTypeUniqueName<T>(string prefix)
        {
            return prefix + "-" + typeof (T).Name;
        }

        public void OnStartupPerform<TStartup>(ConfigurationPredicate when) where TStartup : IStartupAction
        {
            if (!CanContinue(when)) return;
            _container.Put(Binding.For<IStartupAction>().Use<TStartup>().With(Lifestyle.Singleton).Named(GetTypeUniqueName<TStartup>("Startup")));
        }

        public void OnStartupPerform(Action<INodeInstance> startupAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            _container.Put(Binding.For<IStartupAction>().Use(() => new StartupActionWrapper(startupAction)).With(Lifestyle.Singleton).Named("Startup-" + SeqGuid.NewGuid()));
        }

        public void OnShutdownPerform<TShutdown>(ConfigurationPredicate when) where TShutdown : IShutdownAction
        {
            if (!CanContinue(when)) return;
            _container.Put(Binding.For<IShutdownAction>().Use<TShutdown>().With(Lifestyle.Singleton).Named(GetTypeUniqueName<TShutdown>("Shutdown")));
        }

        public void OnShutdownPerform(Action<INodeInstance> shutdownAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            _container.Put(Binding.For<IShutdownAction>().Use(() => new ShutdownActionWrapper(shutdownAction)).With(Lifestyle.Singleton).Named("Shutdown-" + SeqGuid.NewGuid()));
        }

        public void RegisterSubsystem<TSubsystem>() where TSubsystem : ISubsystem
        {
            _container.Put(Binding.For<ISubsystem>().Use<TSubsystem>().With(Lifestyle.Singleton));
        }
    }
}