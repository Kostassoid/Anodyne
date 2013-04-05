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

namespace Kostassoid.Anodyne.Abstractions.Dependency
{
    using System;
    using System.Collections.Generic;
    using Common.Extentions;

	/// <summary>
    /// Binding configuration for multiple components registration.
    /// </summary>
    public class MultipleBinding : LifecycleBasedBinding
    {
        /// <summary>
        /// Components service types.
        /// </summary>
        public IEnumerable<Type> Services { get; private set; }
        /// <summary>
        /// Should use implementation type as service type.
        /// </summary>
        internal bool? BindAsSelf { get; private set; }
		/// <summary>
		/// Optional service types.
		/// </summary>
		public ISet<Type> BindTo { get; private set; }

		internal MultipleBinding(IEnumerable<Type> services)
        {
            Services = services;
            BindTo = new HashSet<Type>();
        }

        internal void AsSelf()
        {
            BindAsSelf = true;
        }

		internal void ForwardTo<TService>() where TService : class
		{
			BindTo.Add(typeof(TService));
		}

		internal void ForwardTo(params Type[] services)
		{
			services.ForEach(t => BindTo.Add(t));
		}
	}
}