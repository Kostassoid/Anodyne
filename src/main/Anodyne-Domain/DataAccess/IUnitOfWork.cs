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

namespace Kostassoid.Anodyne.Domain.DataAccess
{
	using System;
	using System.Linq;
	using Base;
	using Common;
	using Operations;
	using Policy;

	public interface IUnitOfWork : IDisposable
    {
		Option<IUnitOfWork> Parent { get; }
		IUnitOfWork Root { get; }
        IDomainDataSession Session { get; }
		StaleDataPolicy StaleDataPolicy { get; }

        bool IsRoot { get; }

        bool IsCompleted { get; }
        bool IsCancelled { get; }
        bool IsFinished { get; }

	    event Action Completed;
	    event Action Failed;
	    event Action Cancelled;

	    void Complete();
	    void Cancel();

	    IRepository<TRoot> Query<TRoot>() where TRoot : class, IAggregateRoot;
	    IQueryable<TRoot> AllOf<TRoot>() where TRoot : class, IAggregateRoot;
	    TOp Using<TOp>() where TOp : class, IDomainOperation;
    }
}