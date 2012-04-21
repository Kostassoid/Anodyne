using System;

namespace Kostassoid.Anodyne.Common.ExecutionContext
{
    public class Context
    {
        private static IContextProvider _provider = new NormalContextProvider();

        public static void SetProvider(IContextProvider provider)
        {
            _provider = provider;
        }

        public void Set(string name, object value)
        {
            _provider.Set(name, value);
        }

        public object Get(string name)
        {
            var found = _provider.Find(name);
            if (found == null)
                throw new InvalidOperationException(string.Format("Value of '{0}' cannot be found", name));

            return found;
        }

        public T GetAs<T>(string name) where T : class
        {
            return Get(name) as T;
        }

        public Option<object> Find(string name)
        {
            return _provider.Find(name).AsOption();
        }

        public Option<T> FindAs<T>(string name) where T : class
        {
            var found = Find(name);
            if (found.IsNone) return new None<T>();

            return (found.Value as T).AsOption();
        }

        public void Release(string name)
        {
            _provider.Release(name);
        }
         
    }
}