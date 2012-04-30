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

namespace Kostassoid.Anodyne.System
{
    using Common.Extentions;
    using Configuration;
    using Subsystem;
    using global::System.Collections.Generic;
    using global::System.Linq;

    public abstract class AnodyneSystem
    {
        private readonly IConfiguration _configuration = new SystemConfiguration();

        private IConfigurationSettings Cfg { get { return _configuration as IConfigurationSettings; } }

        public SystemState State { get; private set; }

        public bool CanBeStarted { get { return State != SystemState.Started; } }
        public bool CanBeStopped { get { return State != SystemState.Stopped; } }
        public bool MustBeConfigured { get { return !(_configuration as IConfigurationBuilder).IsValid; } }

        public abstract void OnConfigure(IConfiguration configuration);
        public virtual void OnStart() {}
        public virtual void OnShutdown() {}

        private IList<ISubsystem> _subsystems = new List<ISubsystem>();

        public void Start()
        {
            if (!CanBeStarted) return;

            if (MustBeConfigured)
                OnConfigure(_configuration);

            Cfg.Container.GetAll<IBootstrapper>()
                .OrderByDescending(b => b.Priority)
                .ForEach(b => b.Perform(Cfg));

            _subsystems = Cfg.Container.GetAll<ISubsystem>();
            _subsystems.ForEach(s => s.Start());

            OnStart();

            State = SystemState.Started;
        }

        public void Shutdown()
        {
            if (!CanBeStopped) return;

            OnShutdown();

            _subsystems.ForEach(s => s.Stop());

            State = SystemState.Stopped;
        }

    }
}