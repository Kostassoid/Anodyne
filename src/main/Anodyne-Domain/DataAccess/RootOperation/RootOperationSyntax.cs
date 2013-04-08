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

namespace Kostassoid.Anodyne.Domain.DataAccess.RootOperation
{
	using System;
	using Base;
	using Common;

	public class RootOperationSyntax<T> : ISyntax where T : class, IAggregateRoot
	{
		private readonly Func<UnitOfWork, T> _rootAquireFunc;

		public RootOperationSyntax(Func<UnitOfWork, T> rootAquireFunc)
		{
			_rootAquireFunc = rootAquireFunc;
		}

		public void Perform(Action<T, IRootOperationContext> rootAction)
		{
			using (var uow = new UnitOfWork())
			{
				var root = _rootAquireFunc(uow);
				var context = new RootOperationContext(uow);

				rootAction(root, context);
			}
		}

		public void Perform(Action<T> rootAction)
		{
			using (var uow = new UnitOfWork())
			{
				var root = _rootAquireFunc(uow);
				rootAction(root);
			}
		}
	}
}