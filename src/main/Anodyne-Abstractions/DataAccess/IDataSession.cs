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
    /// Data session. Usualy represents single connection to DBMS or open ORM session. Normally not thread-safe.
    /// </summary>
    public interface IDataSession : IDataSessionEx, IDisposable
    {
        /// <summary>
        /// Get queryable for specific type.
        /// </summary>
        /// <typeparam name="T">Persistable root type.</typeparam>
        /// <returns>Queryable object for specified type.</returns>
        IQueryable<T> Query<T>() where T : class, IPersistableRoot;
        /// <summary>
        /// Return one entity from data storage by id.
        /// </summary>
        /// <typeparam name="T">Persistable root type.</typeparam>
        /// <param name="id">Entity identity.</param>
        /// <returns>Found entity or null if nothing is found.</returns>
        T FindOne<T>(object id) where T : class, IPersistableRoot;
        /// <summary>
        /// Return one entity from data storage by id.
        /// </summary>
        /// <param name="type">Persistable root type.</param>
        /// <param name="id">Entity identity.</param>
        /// <returns>Found entity or null if nothing is found.</returns>
        IPersistableRoot FindOne(Type type, object id);

	    /// <summary>
	    /// Persist entity. Update if entity with the same id already exists.
	    /// </summary>
	    /// <param name="o">Entity to save.</param>
	    /// <param name="specificVersion">Specific root version to update.</param>
	    bool SaveOne(IPersistableRoot o, long? specificVersion);

        /// <summary>
        /// Remove entity from persistence storage.
        /// </summary>
        /// <param name="type">Persistable root type.</param>
        /// <param name="id">Entity identity.</param>
		/// <param name="specificVersion">Specific root version to remove.</param>
		bool RemoveOne(Type type, object id, long? specificVersion);
    }
}