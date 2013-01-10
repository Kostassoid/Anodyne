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

using System;

namespace Kostassoid.Anodyne.Node.Dependency
{
    public class SingleBinding : LifestyleBasedBinding
    {
        public Type Service { get; protected set; }
        public IImplementationResolver Resolver { get; protected set; }
        public string Named { get; protected set; }

        public SingleBinding(Type service)
        {
            Service = service;
        }

        public void SetResolver(IImplementationResolver resolver)
        {
            Resolver = resolver;
        }

        public void SetName(string name)
        {
            Named = name;
        }
    }
}