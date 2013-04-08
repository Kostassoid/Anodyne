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
	using Abstractions.DataAccess;
    using Common;
	using Common.ExecutionContext;
    using Common.Reflection;
	using Operations;
    using Policy;
    using Domain.Events;
    using Wiring;

    //TODO: refactor this ugly pile of code
    public static class UnitOfWork
    {
        private const string RootContextKey = "root-unit-of-work";
		private const string HeadContextKey = "head-unit-of-work";

        public static IDataSessionFactory DataSessionFactory { get; internal set; }
        public static IOperationResolver OperationResolver { get; internal set; }
		public static DataAccessPolicy Policy { get; internal set; }

        private static bool _eventHandlersAreSet;

		public static Option<IUnitOfWork> Root
		{
			get { return Context.FindAs<IUnitOfWork>(RootContextKey); }
			set
			{
				if (value.IsSome)
					Context.Set(RootContextKey, value.ValueOrDefault);
				else
					Context.Release(RootContextKey);
			}
		}

		public static Option<IUnitOfWork> Head
		{
			get { return Context.FindAs<IUnitOfWork>(HeadContextKey); }
			set
			{
				if (value.IsSome)
					Context.Set(HeadContextKey, value.Value);
				else
					Context.Release(HeadContextKey);
			}
		}

		public static bool IsConfigured { get { return DataSessionFactory != null && OperationResolver != null; } }

        static UnitOfWork()
        {
            Policy = new DataAccessPolicy();
        }

        public static IUnitOfWork Start(StaleDataPolicy? staleDataPolicy = null)
        {
            lock (HeadContextKey) EnsureAggregateEventHandlersAreSet();

	        var newUnitOfWork = Head.IsSome
				? new UnitOfWorkInstance(Head.Value)
				: new UnitOfWorkInstance(OpenDomainSession(), staleDataPolicy.HasValue ? staleDataPolicy.Value : Policy.StaleDataPolicy);

			Head = newUnitOfWork;

			if (Root.IsNone)
		        Root = newUnitOfWork;

	        return newUnitOfWork;
        }

	    private static void EnsureAggregateEventHandlersAreSet()
        {
            if (_eventHandlersAreSet) return;
            _eventHandlersAreSet = true;

            EventBus
                .SubscribeTo()
                .AllBasedOn<IAggregateEvent>(From.AllAssemblies())
                .With(e =>
                {
                    if (Policy.ReadOnly)
                        throw new InvalidOperationException("You can't mutate AggregateRoots in ReadOnly mode.");

                    if (Head.IsSome && !Head.Value.IsFinished)
                    {
                        Head.Value.DomainDataSession.Handle(e);
                    }
                    else
                    {
                        throw new InvalidOperationException("There's no active UnitOfWork.");
                    }
                }, Priority.Exact(1000));
        }

	    public static IDomainDataSession OpenDomainSession()
	    {
			var session = new DomainDataSession(DataSessionFactory.Open());
			if (session == null)
				throw new Exception("Unable to create IDataSession (bad configuration?)");

		    return session;
	    }

	    internal static void Close(UnitOfWorkInstance unitOfWork)
	    {
			if (unitOfWork != Head.ValueOrDefault)
				throw new InvalidOperationException("Unable to close intermediate UnitOfWork. Dispose the head first.");

		    Head = unitOfWork.Parent.AsOption();

			if (Head.IsNone)
			{
				Root = Option<IUnitOfWork>.None;
			}
	    }
    }
}