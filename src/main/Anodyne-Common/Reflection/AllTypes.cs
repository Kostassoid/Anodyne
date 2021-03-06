﻿// Copyright 2011-2013 Anodyne.
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
using System.Linq;
using System.Reflection;

namespace Kostassoid.Anodyne.Common.Reflection
{
    public class AllTypes
    {
        public static IEnumerable<Type> BasedOn<T>(IEnumerable<Assembly> from = null)
        {
            return BasedOn(typeof (T), from);
        }

        public static IEnumerable<Type> BasedOn(Type baseType, IEnumerable<Assembly> from = null)
        {
            var lookinAssemblies = from ?? From.AllAssemblies();

            return lookinAssemblies
                .SelectMany(a => a.GetTypes())
                .Where(baseType.IsAssignableFrom);
        }
    }
}