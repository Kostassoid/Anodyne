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

namespace Kostassoid.Anodyne.Common.Extentions
{
    using System;
    using System.Collections.Generic;

    public static class EnumerableEx
    {
        public static IEnumerable<T> SelectDeep<T>(
            this IEnumerable<T> source, Func<T, IEnumerable<T>> selector)
        {
            foreach (T item in source)
            {
                yield return item;
                foreach (T subItem in SelectDeep(selector(item), selector))
                {
                    yield return subItem;
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source) action(item);
        }
    }
}