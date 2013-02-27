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

namespace Kostassoid.Anodyne.Abstractions.DataAccess.Internal
{
    using Common;
    using Common.ExecutionContext;
    using Common.Tools;

    internal class DefaultDataAccessContext : IDataAccessContext
    {
        private const string ContextKeyFormat = "DataAccessContext-{0}";

        private readonly IDataAccessProvider _dataAccessProvider;
        private readonly string _contextKey;

        public DefaultDataAccessContext(IDataAccessProvider dataAccessProvider)
        {
            _dataAccessProvider = dataAccessProvider;
            _contextKey = string.Format(ContextKeyFormat, SeqGuid.NewGuid().ToString("n"));
        }

        public bool HasOpenSession { get { return Context.FindAs<IDataSession>(_contextKey).IsSome; } }

        public IDataSession GetSession()
        {
            var session = Context.FindAs<IDataSession>(_contextKey);
            if (session.IsNone)
            {
                session = _dataAccessProvider.SessionFactory.Open().AsOption();
                Context.Set(_contextKey, session.Value);
            }
            return session.Value;
        }

        public void CloseSession()
        {
            if (HasOpenSession)
                Context.Release(_contextKey);
        }

        public virtual void Dispose()
        {
            CloseSession();
        }
    }
}