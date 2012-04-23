using System;
using System.Collections.Generic;
using System.Linq;

namespace Kostassoid.Anodyne.Common.Extentions
{
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