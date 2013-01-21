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
using System.Collections.Generic;

namespace Kostassoid.Anodyne.Node.Dependency.Registration
{
    using Internal;

    public static class Binding
    {
        public static ISingleBindingSyntax<TService> For<TService>() where TService : class
        {
            return new SingleBindingSyntax<TService>();
        }

        public static ISingleBindingSyntax For(Type service)
        {
            return new SingleBindingSyntax(service);
        }

        public static IMultipleBindingSyntax Use(IEnumerable<Type> services)
        {
            return new MultipleBindingSyntax(services);
        }
    }
}