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

namespace Kostassoid.Anodyne.Abstractions.DataAccess
{
    using System.Linq;
    using Common;
    using Common.ExecutionContext;

    public class DefaultDataAccessContext : IDataAccessContext
    {
        private const string ContextValueName = "DataAccessContext-Session";

        private readonly IDataAccessProvider _dataAccessProvider;

        public DefaultDataAccessContext(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
        }

        public IDataSession GetCurrentSession()
        {
            var session = Context.FindAs<IDataSession>(ContextValueName);
            if (session.IsNone)
            {
                session = _dataAccessProvider.SessionFactory.Open().AsOption();
                Context.Set(ContextValueName, session.Value);
            }
            return session.Value;
        }

        public void CloseCurrentSession()
        {
            if (Context.FindAs<IDataSession>(ContextValueName).IsSome)
                Context.Release(ContextValueName);
        }

        public IQueryable<T> Query<T>() where T : class, IPersistableRoot
        {
            return GetCurrentSession().Query<T>();
        }

        public virtual void Dispose()
        {
            CloseCurrentSession();
        }

        ~DefaultDataAccessContext()
        {
            Dispose();
        }
    }
}