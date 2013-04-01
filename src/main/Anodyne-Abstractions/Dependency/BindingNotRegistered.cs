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

	public class BindingNotRegisteredException : Exception
	{
		private const string MessageForUnnamedService = "Binding for service '{0}' couldn't be found in container. Check inner exception for more details.";
		private const string MessageForNamedService = "Binding for service '{0}' with name '{1}' couldn't be found in container. Check inner exception for more details.";

		public Type Service { get; private set; }

		public BindingNotRegisteredException(Type service, Exception providerException)
			: base(string.Format(MessageForUnnamedService, service.FullName), providerException)
		{
			Service = service;
		}

		public BindingNotRegisteredException(Type service, string name, Exception providerException)
			: base(string.Format(MessageForNamedService, service.FullName, name), providerException)
		{
			Service = service;
		}
	}
}