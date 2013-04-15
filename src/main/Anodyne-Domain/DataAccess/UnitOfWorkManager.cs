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
	using Common;
	using Common.ExecutionContext;
	using Common.Reflection;
	using Operations;
    using Policy;
    using Domain.Events;
	using Wiring;

    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private const string RootContextKey = "root-unit-of-work";
		private const string HeadContextKey = "head-unit-of-work";

        private Action _unsubscribeAction;
        public bool IsSubscribed { get { return _unsubscribeAction != null; } }

		public IUnitOfWorkFactory Factory { get; private set; }
        public IOperationResolver OperationResolver { get; private set; }
		public DataAccessPolicy Policy { get; set; }

		public Option<IUnitOfWork> Root
		{
			get { return Context.FindAs<IUnitOfWork>(RootContextKey); }
			private set { Context.Set(RootContextKey, value); }
		}

		public Option<IUnitOfWork> Head
		{
			get { return Context.FindAs<IUnitOfWork>(HeadContextKey); }
			private set { Context.Set(HeadContextKey, value); }
		}

        public UnitOfWorkManager(IUnitOfWorkFactory factory, IOperationResolver operationResolver)
        {
            Factory = factory;
            OperationResolver = operationResolver;

            Policy = new DataAccessPolicy();
            SubscribeToAggregateEvents();
        }

        private void SubscribeToAggregateEvents()
        {
            UnsubscribeFromAggregateEvents();

            _unsubscribeAction = EventBus
                .SubscribeTo()
                .AllBasedOn<IUncommitedEvent>(From.AllAssemblies())
                .With(Handle, Priority.Exact(1000));
        }

        private void UnsubscribeFromAggregateEvents()
        {
            if (IsSubscribed)
            {
                _unsubscribeAction();
                _unsubscribeAction = null;
            }
        }

        public IUnitOfWork Start(StaleDataPolicy? staleDataPolicy = null)
        {
	        var newUnitOfWork = Head.IsSome
				? Factory.Build(Head.Value)
				: Factory.Build(staleDataPolicy.HasValue ? staleDataPolicy.Value : Policy.StaleDataPolicy);

            ((IUnitOfWorkEx) newUnitOfWork).Manager = this;

			Head = newUnitOfWork.AsOption();

			if (Root.IsNone)
		        Root = Head;

	        return newUnitOfWork;
        }

		public void Finish(IUnitOfWork unitOfWork)
		{
			if (unitOfWork != Head.ValueOrDefault)
				throw new InvalidOperationException("Unable to close intermediate UnitOfWork. Dispose the head first.");

			Head = unitOfWork.Parent;

			if (Head.IsNone)
			{
				Root = Option<IUnitOfWork>.None;
			}
		}

		public void Handle(IUncommitedEvent ev)
		{
			if (Policy.ReadOnly)
				throw new InvalidOperationException("You can't mutate AggregateRoots in ReadOnly mode.");

			if (Head.IsSome && !Head.Value.IsFinished)
			{
				Head.Value.Session.Handle(ev);
			}
			else
			{
				throw new InvalidOperationException("There's no active UnitOfWork.");
			}
		}

        public void Dispose()
        {
            while (Head.IsSome)
            {
                Finish(Head.Value);
            }

            UnsubscribeFromAggregateEvents();
        }

        ~UnitOfWorkManager()
        {
            Dispose();
        }
    }
}