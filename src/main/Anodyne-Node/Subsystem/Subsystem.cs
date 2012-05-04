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

namespace Kostassoid.Anodyne.Node.Subsystem
{
    using System;
    using log4net;

    public abstract class Subsystem : ISubsystem
    {
        private InstanceState _state = InstanceState.Stopped;
        private readonly ILog _logger;

        protected virtual ILog Logger
        {
            get { return _logger; }
        }

        protected Subsystem()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public virtual InstanceState State
        {
            get { return _state; }
            protected set { _state = value; }
        }

        public virtual void Start()
        {
            if (_state == InstanceState.Stopped)
            {
                OnStart();
                _state = InstanceState.Started;
                // ReSharper disable PolymorphicFieldLikeEventInvocation
                Started(this);
                // ReSharper restore PolymorphicFieldLikeEventInvocation
                Logger.InfoFormat("Subsystem '{0}' started successfully.", GetType().Name);
            }
        }

        public virtual void Stop()
        {
            if (this._state == InstanceState.Started)
            {
                OnStop();
                this._state = InstanceState.Stopped;
                // ReSharper disable PolymorphicFieldLikeEventInvocation
                Stopped(this);
                // ReSharper restore PolymorphicFieldLikeEventInvocation
                Logger.InfoFormat("Subsystem '{0}' stopped successfully.", GetType().Name);
            }
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        
        public virtual event Action<ISubsystem> Started = s => {};
        public virtual event Action<ISubsystem> Stopped = s => { };
    }
}