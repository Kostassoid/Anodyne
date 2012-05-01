// Copyright 2011-2012 Anodyne.
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

namespace Kostassoid.Anodyne.DataAccess.Operations
{
    using Common;
    using Domain.Base;
    using global::System;
    using global::System.Linq;
    using global::System.Linq.Expressions;

    public interface IRepository<TRoot> where TRoot : class, IAggregateRoot
    {
        TRoot this[object key] { get; }
        TRoot GetOne(object key);
        Option<TRoot> FindOne(object key);
        IQueryable<TRoot> All();
        bool Exists(object key);
        long Count(Expression<Func<TRoot, bool>> criteria);
        long Count();
    }
}