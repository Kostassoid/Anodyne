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

namespace Kostassoid.Anodyne.Domain.Base
{
    using System.Linq;
    using Common.Extentions;
    using Common.Reflection;
    using Events;
    using Wiring;

    public static class AggregateRootHandlersRegistrator
    {
        private static bool _handlersAreRegistered;
        private static readonly object Locker = new object();

        public static void EnsureRegistration()
        {
            if (_handlersAreRegistered) return; //avoid lock as much as possible
            lock (Locker)
            {
                if (_handlersAreRegistered) return;
                _handlersAreRegistered = true;

                AllTypes.BasedOn<IAggregateRoot>()
                    .Where(r => !r.IsAbstract && !r.IsInterface)
                    .ForEach(r => EventBus.Extentions.BindDomainEvents(r));
            }
        }
    }
}