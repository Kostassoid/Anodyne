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

namespace Kostassoid.Anodyne.Node
{
    using System.Collections.Generic;
    using Common.Extentions;
    using Configuration;
    using Subsystem;

    public abstract class Node
    {
        private readonly IConfiguration _configuration = new NodeInstance();

        public INodeInstance Instance { get { return _configuration as INodeInstance; } }

        public InstanceState State { get; private set; }

        public bool CanBeStarted { get { return State != InstanceState.Started; } }
        public bool CanBeStopped { get { return State != InstanceState.Stopped; } }
        public bool MustBeConfigured { get { return !((IConfigurationBuilder) _configuration).IsValid; } }

        public abstract void OnConfigure(IConfiguration configuration);
        public virtual void OnStart() {}
        public virtual void OnShutdown() {}

        public bool IsIn(RuntimeMode runtimeMode)
        {
            return Instance.RuntimeMode == runtimeMode;
        }

        private IList<ISubsystem> _subsystems = new List<ISubsystem>();

        public void Start()
        {
            if (!CanBeStarted) return;

            //TODO: move
            if (MustBeConfigured)
                OnConfigure(_configuration);

            Instance.Container.GetAll<IStartupAction>().ForEach(b => b.OnStartup(Instance));

            _subsystems = Instance.Container.GetAll<ISubsystem>();
            _subsystems.ForEach(s => s.Start());

            OnStart();

            State = InstanceState.Started;
        }

        public void Shutdown()
        {
            if (!CanBeStopped) return;

            Instance.Container.GetAll<IShutdownAction>().ForEach(b => b.OnShutdown(Instance));

            OnShutdown();

            _subsystems.ForEach(s => s.Stop());

            State = InstanceState.Stopped;
        }

    }
}