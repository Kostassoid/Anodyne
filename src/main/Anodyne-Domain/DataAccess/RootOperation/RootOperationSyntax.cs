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
		private readonly Func<IUnitOfWork, T> _rootAquireFunc;
//		private Action _missedAction = () => { };

		public RootOperationSyntax(Func<IUnitOfWork, T> rootAquireFunc)
		{
			_rootAquireFunc = rootAquireFunc;
		}

/*		private bool ValidateRoot(T root)
		{
			if (root == null)
			{
				_missedAction();
				return false;
			}

			return true;
		}*/

		public void Perform(Action<T, IRootOperationContext> rootAction)
		{
			using (var uow = UnitOfWork.Start())
			{
				var root = _rootAquireFunc(uow);
				//if (!ValidateRoot(root)) return;

				var context = new RootOperationContext(uow);
				rootAction(root, context);
			}
		}

		public void Perform(Action<T> rootAction)
		{
			using (var uow = UnitOfWork.Start())
			{
				var root = _rootAquireFunc(uow);
//				if (!ValidateRoot(root)) return;

				rootAction(root);
			}
		}

		public TResult Request<TResult>(Func<T, IRootOperationContext, TResult> rootFunc)
		{
			using (var uow = UnitOfWork.Start())
			{
				var root = _rootAquireFunc(uow);
//				if (!ValidateRoot(root)) return default(TResult);

				var context = new RootOperationContext(uow);
				return rootFunc(root, context);
			}
		}

		public TResult Request<TResult>(Func<T, TResult> rootFunc)
		{
			using (var uow = UnitOfWork.Start())
			{
				var root = _rootAquireFunc(uow);
//				if (!ValidateRoot(root)) return default(TResult);

				return rootFunc(root);
			}
		}

/*		public RootOperationSyntax<T> IfAbsent(Action missedAction)
		{
			_missedAction = missedAction;
			return this;
		}*/
	}
}