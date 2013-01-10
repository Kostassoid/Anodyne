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

namespace Kostassoid.Anodyne.Common.ExecutionContext
{
    using System;
    using System.ComponentModel;

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

        public static T GetAs<T>(string name)
        {
            var found = Get(name);
            if (!(found is T))
                throw new InvalidOperationException(string.Format("Value of '{0}' is of type {1}, but {2} was expected", name, found.GetType().Name, typeof (T).Name));

            return (T)found;
        }

        public static Option<object> Find(string name)
        {
            return _provider.Find(name).AsOption();
        }

        public static Option<T> FindAs<T>(string name)
        {
            var found = Find(name);
            if (found.IsNone || !(found.Value is T)) return new None<T>();

            return (T)(found.Value).AsOption();
        }

        public static void Release(string name)
        {
            _provider.Release(name);
        }
    }
}