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

namespace Kostassoid.Anodyne.DataAccess
{
    using System.Linq;

    public interface IDataSession : IDataSessionEx, IDisposable
    {
        IQueryable<T> Query<T>() where T : class, IPersistableRoot;
        T FindOne<T>(object id) where T : class, IPersistableRoot;
        IPersistableRoot FindOne(Type type, object id);
        void SaveOne(IPersistableRoot o);
        void RemoveOne(Type type, object id);
    }
}