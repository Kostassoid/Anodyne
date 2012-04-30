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
    using Dependency;
    using Logging;

    public class SystemConfiguration : IConfiguration, IConfigurationBuilder, IConfigurationSettings
    {
        private RuntimeMode _runtimeMode;
        RuntimeMode IConfigurationSettings.RuntimeMode { get { return _runtimeMode; } }

        private IContainer _container;
        IContainer IConfigurationSettings.Container { get { return _container; } }

        private ILoggerAdapter _loggerAdapter;
        ILoggerAdapter IConfigurationSettings.Logger { get { return _loggerAdapter; } }

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

        public void RunIn(RuntimeMode runtimeMode)
        {
            _runtimeMode = runtimeMode;
        }
    }
}