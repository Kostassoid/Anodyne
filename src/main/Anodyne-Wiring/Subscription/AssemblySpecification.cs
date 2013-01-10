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

namespace Kostassoid.Anodyne.Wiring.Subscription
{
    using System;
    using System.Reflection;
    using Common;

    internal class AssemblySpecification
    {
        public Option<Assembly> This { get; protected set; }
        public Predicate<string> Filter { get; protected set; }

        public AssemblySpecification(Assembly thisAssembly)
        {
            This = thisAssembly;
        }

        public AssemblySpecification(Predicate<string> filter)
        {
            This = new None<Assembly>();
            Filter = filter;
        }
    }
}