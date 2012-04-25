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

namespace Kostassoid.Anodyne.DataAccess
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Common;
    using Domain.Base;

    public interface IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        TEntity this[object key] { get; }
        TEntity Get(object key);
        Option<TEntity> FindBy(object key);
        IQueryable<TEntity> All();
        bool Exists(object key);
        long Count(Expression<Func<TEntity, bool>> criteria);
        long Count();
    }
}