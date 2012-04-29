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

namespace Kostassoid.Anodyne.Common.Tools
{
    using System;
    using System.Collections.Generic;

    public static class MemoizedFunc
    {
        public static Func<TResult> From<TResult>(Func<TResult> func)
        {
            return func.AsMemoized();
        }

        public static Func<TSource, TReturn> From<TSource, TReturn>(Func<TSource, TReturn> func)
        {
            return func.AsMemoized();
        }

        public static Func<TSource1, TSource2, TReturn> From<TSource1, TSource2, TReturn>(Func<TSource1, TSource2, TReturn> func)
        {
            return func.AsMemoized();
        }

        public static Func<TSource1, TSource2, TSource3, TReturn> From<TSource1, TSource2, TSource3, TReturn>(Func<TSource1, TSource2, TSource3, TReturn> func)
        {
            return func.AsMemoized();
        }

        public static Func<TReturn> AsMemoized<TReturn>(this Func<TReturn> func)
        {
            object cache = null;
            return () =>
            {
                if (cache == null)
                    cache = func();

                return (TReturn)cache;
            };
        }

        public static Func<TSource, TReturn> AsMemoized<TSource, TReturn>(this Func<TSource, TReturn> func)
        {
            var cache = new Dictionary<TSource, TReturn>();
            return s =>
            {
                lock(func)
                if (!cache.ContainsKey(s))
                {
                    cache[s] = func(s);
                }
                return cache[s];
            };
        }

        public static Func<TSource1, TSource2, TReturn> AsMemoized<TSource1, TSource2, TReturn>(this Func<TSource1, TSource2, TReturn> func)
        {
            var cache = new Dictionary<Tuple<TSource1, TSource2>, TReturn>();
            return (s1, s2) =>
            {
                var key = new Tuple<TSource1, TSource2>(s1, s2);

                lock (func)
                if (!cache.ContainsKey(key))
                {
                    cache[key] = func(s1, s2);
                }
                return cache[key];
            };
        }

        public static Func<TSource1, TSource2, TSource3, TReturn> AsMemoized<TSource1, TSource2, TSource3, TReturn>(this Func<TSource1, TSource2, TSource3, TReturn> func)
        {
            var cache = new Dictionary<Tuple<TSource1, TSource2, TSource3>, TReturn>();
            return (s1, s2, s3) =>
            {
                var key = new Tuple<TSource1, TSource2, TSource3>(s1, s2, s3);

                lock (func)
                if (!cache.ContainsKey(key))
                {
                    cache[key] = func(s1, s2, s3);
                }
                return cache[key];
            };
        }


    }
}