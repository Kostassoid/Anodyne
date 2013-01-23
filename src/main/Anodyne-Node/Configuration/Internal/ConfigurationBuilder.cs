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

namespace Kostassoid.Anodyne.Node.Configuration.Internal
{
    using Abstractions.DataAccess;
    using Abstractions.Dependency;
    using Abstractions.Dependency.Registration;
    using Abstractions.Wcf;
    using Domain.DataAccess;
    using Domain.DataAccess.Policy;
    using System.Reflection;
    using Common.Tools;
    using Subsystem;
    using System;

    internal class ConfigurationBuilder : INodeConfigurator, INodeConfiguratorEx
    {
        private readonly NodeConfiguration _configuration;

        public INodeConfiguration Configuration { get { return _configuration; } }

        public ConfigurationBuilder()
        {
            _configuration = new NodeConfiguration();

            RunIn(RuntimeMode.Production);
            DefineSystemNamespaceAs(DetectSystemNamespace());
        }

        private static string DetectSystemNamespace()
        {
            var assembly = Assembly.GetEntryAssembly(); // could be null during tests run
            return assembly != null ? assembly.FullName.Substring(0, assembly.FullName.IndexOfAny(new[] { '.', '-', '_' })) : "";
        }

        void INodeConfiguratorEx.SetContainerAdapter(IContainer container)
        {
            if (_configuration.Container != null)
                throw new InvalidOperationException("Container adapter is already set to " + _configuration.Container.GetType().Name);

            _configuration.Container = container;
        }

        void INodeConfiguratorEx.SetWcfProxyFactory(IWcfProxyFactory wcfProxyFactory)
        {
            _configuration.WcfProxyFactory = wcfProxyFactory;
        }

        void INodeConfiguratorEx.SetDataAccessProvider(IDataAccessProvider dataAccessProvider)
        {
            _configuration.DataAccess = dataAccessProvider;
        }

        public void RunIn(RuntimeMode runtimeMode)
        {
            _configuration.RuntimeMode = runtimeMode;
        }

        public void DefineSystemNamespaceAs(string systemNamespace)
        {
            _configuration.SystemNamespace = systemNamespace;

            //AssemblyPreloader.Preload(From.AllFilesIn(".").Where(f => f.Extension == ".dll" && f.Name.StartsWith(systemNamespace)));
        }

        public void UseDataAccessPolicy(Action<DataAccessPolicy> policyAction)
        {
            var dataPolicy = new DataAccessPolicy();
            policyAction(dataPolicy);

            UnitOfWork.EnforcePolicy(dataPolicy);
        }

        private bool CanContinue(ConfigurationPredicate when)
        {
            return when == null || when(_configuration);
        }

        public void ConfigureUsing<TConfiguration>(ConfigurationPredicate when) where TConfiguration : IConfigurationAction
        {
            if (!CanContinue(when)) return;
            EnsureContainerIsSet();

            _configuration.Container.Put(
                Binding.For<IConfigurationAction>()
                .Use<TConfiguration>().With(Lifestyle.Unmanaged)
                .Named(GetTypeUniqueName<TConfiguration>("Configuration")));
        }

        public void ConfigureUsing(Action<INodeConfiguration> configurationAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            EnsureContainerIsSet();

            _configuration.Container.Put(
                Binding.For<IConfigurationAction>()
                .Use(() => new ConfigurationActionWrapper(configurationAction))
                .With(Lifestyle.Unmanaged)
                .Named("Configuration-" + SeqGuid.NewGuid()));
        }

        private static string GetTypeUniqueName<T>(string prefix)
        {
            return prefix + "-" + typeof (T).Name;
        }

        public void OnStartupPerform<TStartup>(ConfigurationPredicate when) where TStartup : IStartupAction
        {
            if (!CanContinue(when)) return;
            EnsureContainerIsSet();

            _configuration.Container.Put(Binding.For<IStartupAction>().Use<TStartup>().With(Lifestyle.Singleton).Named(GetTypeUniqueName<TStartup>("Startup")));
        }

        public void OnStartupPerform(Action<INodeConfiguration> startupAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            EnsureContainerIsSet();

            _configuration.Container.Put(Binding.For<IStartupAction>().Use(() => new StartupActionWrapper(startupAction)).With(Lifestyle.Unmanaged).Named("Startup-" + SeqGuid.NewGuid()));
        }

        public void OnShutdownPerform<TShutdown>(ConfigurationPredicate when) where TShutdown : IShutdownAction
        {
            if (!CanContinue(when)) return;
            EnsureContainerIsSet();

            _configuration.Container.Put(Binding.For<IShutdownAction>().Use<TShutdown>().With(Lifestyle.Unmanaged).Named(GetTypeUniqueName<TShutdown>("Shutdown")));
        }

        public void OnShutdownPerform(Action<INodeConfiguration> shutdownAction, ConfigurationPredicate when)
        {
            if (!CanContinue(when)) return;
            _configuration.Container.Put(Binding.For<IShutdownAction>().Use(() => new ShutdownActionWrapper(shutdownAction)).With(Lifestyle.Unmanaged).Named("Shutdown-" + SeqGuid.NewGuid()));
        }

        public void RegisterSubsystem<TSubsystem>() where TSubsystem : ISubsystem
        {
            _configuration.Container.Put(Binding.For<ISubsystem>().Use<TSubsystem>().With(Lifestyle.Unmanaged));
        }

        private void EnsureConfigurationIsValid()
        {
            EnsureContainerIsSet();
        }

        private void EnsureContainerIsSet()
        {
            if (_configuration.Container == null)
                throw new InvalidOperationException("Node container should be configured first");
        }

        public INodeConfiguration Build()
        {
            EnsureConfigurationIsValid();

            return _configuration;
        }
    }
}