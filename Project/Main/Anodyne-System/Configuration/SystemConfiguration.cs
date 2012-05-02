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

namespace Kostassoid.Anodyne.System.Configuration
{
    using Anodyne.DataAccess;
    using Dependency;
    using Logging;
    using Wcf;

    public class SystemConfiguration : IConfiguration, IConfigurationBuilder, ISystemConfiguration
    {
        private RuntimeMode _runtimeMode;
        RuntimeMode ISystemConfiguration.RuntimeMode { get { return _runtimeMode; } }

        private IContainer _container;
        IContainer ISystemConfiguration.Container { get { return _container; } }

        private ILoggerAdapter _loggerAdapter;
        ILoggerAdapter ISystemConfiguration.Logger { get { return _loggerAdapter; } }

        IDataAccessProvider ISystemConfiguration.DataAccess { get { return _container.Get<IDataAccessProvider>(); } }

        private IWcfServiceProvider _wcfServiceProvider;
        IWcfServiceProvider ISystemConfiguration.WcfServiceProvider { get { return _wcfServiceProvider; } }

        public SystemConfiguration()
        {
            _runtimeMode = RuntimeMode.Production;
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

        void IConfigurationBuilder.SetWcfServiceProvider(IWcfServiceProvider wcfServiceProvider)
        {
            _wcfServiceProvider = wcfServiceProvider;
        }

        public void RunIn(RuntimeMode runtimeMode)
        {
            _runtimeMode = runtimeMode;
        }

        private string GetTypeUniqueName<T>(string prefix)
        {
            return prefix + "-" + typeof (T).Name;
        }

        public void OnStartupPerform<TStartup>() where TStartup : IStartupAction
        {
            _container.For<IStartupAction>().Use<TStartup>(Lifestyle.Singleton, GetTypeUniqueName<TStartup>("Startup"));
        }

        public void OnShutdownPerform<TShutdown>() where TShutdown : IShutdownAction
        {
            _container.For<IShutdownAction>().Use<TShutdown>(Lifestyle.Singleton, GetTypeUniqueName<TShutdown>("Shutdown"));
        }
    }
}