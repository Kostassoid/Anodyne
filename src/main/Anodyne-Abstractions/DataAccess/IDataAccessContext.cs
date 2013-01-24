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
    using System;
    using System.Linq;

    /// <summary>
    /// Allows to work with single DataSession within execution Context (for ex. thread, or HttpContext).
    /// </summary>
    public interface IDataAccessContext : IDisposable
    {
        /// <summary>
        /// Get IQueryable for persistable entity.
        /// </summary>
        /// <typeparam name="T">Type of persistable entity.</typeparam>
        /// <returns>IQueryable for required entity type.</returns>
        IQueryable<T> Query<T>() where T : class, IPersistableRoot;
        /// <summary>
        /// Get DataSession for current context (open new if needed).
        /// </summary>
        /// <returns>Open DataSession.</returns>
        IDataSession GetCurrentSession();
        /// <summary>
        /// Close DataSession associated with current context.
        /// </summary>
        void CloseCurrentSession();
    }
}