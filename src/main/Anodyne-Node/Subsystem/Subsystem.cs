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

namespace Kostassoid.Anodyne.Node.Subsystem
{
    using System;
    using log4net;

    /// <summary>
    /// Base subsystem implementation.
    /// </summary>
    public abstract class Subsystem : ISubsystem
    {
        private InstanceState _state = InstanceState.Stopped;
        private readonly ILog _logger;

        /// <summary>
        /// Subsystem logger.
        /// </summary>
        protected virtual ILog Logger
        {
            get { return _logger; }
        }

        /// <summary>
        /// Base subsystem constructor.
        /// </summary>
        protected Subsystem()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        /// <summary>
        /// Subsystem runtime state.
        /// </summary>
        public virtual InstanceState State
        {
            get { return _state; }
            protected set { _state = value; }
        }

        /// <summary>
        /// Start subsystem.
        /// </summary>
        public void Start()
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

        /// <summary>
        /// Stop subsystem.
        /// </summary>
        public void Stop()
        {
            if (_state == InstanceState.Started)
            {
                OnStop();
                _state = InstanceState.Stopped;
                // ReSharper disable PolymorphicFieldLikeEventInvocation
                Stopped(this);
                // ReSharper restore PolymorphicFieldLikeEventInvocation
                Logger.InfoFormat("Subsystem '{0}' stopped successfully.", GetType().Name);
            }
        }

        /// <summary>
        /// Called upon subsystem start.
        /// </summary>
        protected abstract void OnStart();
        /// <summary>
        /// Called upon subsystem stop.
        /// </summary>
        protected abstract void OnStop();
        
        /// <summary>
        /// Notifies when subsystem has been started.
        /// </summary>
        public event Action<ISubsystem> Started = s => {};
        /// <summary>
        /// Notifies when subsystem has been stopped.
        /// </summary>
        public event Action<ISubsystem> Stopped = s => {};
    }
}