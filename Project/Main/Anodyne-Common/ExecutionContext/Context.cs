using System;
using System.ComponentModel;

namespace Kostassoid.Anodyne.Common.ExecutionContext
{
    public class Context
    {
        private static IContextProvider _provider = new NormalContextProvider();

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static void SetProvider(IContextProvider provider)
        {
            _provider = provider;
        }

        public static void Set(string name, object value)
        {
            _provider.Set(name, value);
        }

        public static object Get(string name)
        {
            var found = _provider.Find(name);
            if (found == null)
                throw new InvalidOperationException(string.Format("Value of '{0}' cannot be found", name));

            return found;
        }

        public static T GetAs<T>(string name) where T : class
        {
            var found = Get(name);
            if (found as T == null)
                throw new InvalidOperationException(string.Format("Value of '{0}' is of type {1}, but {2} was expected", name, found.GetType().Name, typeof(T).Name));

            return found as T;
        }

        public static Option<object> Find(string name)
        {
            return _provider.Find(name).AsOption();
        }

        public static Option<T> FindAs<T>(string name) where T : class
        {
            var found = Find(name);
            if (found.IsNone) return new None<T>();

            return (found.Value as T).AsOption();
        }

        public static void Release(string name)
        {
            _provider.Release(name);
        }
         
    }
}