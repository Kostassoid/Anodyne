namespace Kostassoid.Anodyne.Common.Tools
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Globalization;

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
            var cache = new Dictionary<string, TReturn>();
            return (s1, s2) =>
            {
                var key = s1.GetHashCode().ToString(CultureInfo.InvariantCulture)
                    + s2.GetHashCode().ToString(CultureInfo.InvariantCulture);

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
            var cache = new Dictionary<string, TReturn>();
            return (s1, s2, s3) =>
            {
                var key = s1.GetHashCode().ToString(CultureInfo.InvariantCulture)
                    + s2.GetHashCode().ToString(CultureInfo.InvariantCulture)
                    + s3.GetHashCode().ToString(CultureInfo.InvariantCulture);

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