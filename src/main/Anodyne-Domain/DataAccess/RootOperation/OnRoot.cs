﻿// Copyright 2011-2013 Anodyne.
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
	using System.Linq;
	using Common.CodeContracts;
	using Base;

	public static class OnRoot<T> where T : class, IAggregateRoot
	{
		public static RootOperationSyntax<T> IdentifiedBy(object id)
		{
			Requires.NotNull(id, "id");

			return new RootOperationSyntax<T>(uow => uow.Query<T>().FindOne(id).ValueOrDefault);
		}

		public static RootOperationSyntax<T> AcquiredBy(Func<IQueryable<T>, T> aquireFunc)
		{
			Requires.NotNull(aquireFunc, "aquireFunc");

			return new RootOperationSyntax<T>(uow => aquireFunc(uow.Query<T>().All()));
		}

		public static RootOperationSyntax<T> ConstructedBy(Func<T> rootFactoryFunc)
		{
			Requires.NotNull(rootFactoryFunc, "rootFactoryFunc");

			return new RootOperationSyntax<T>(_ => rootFactoryFunc());
		}
	}
}