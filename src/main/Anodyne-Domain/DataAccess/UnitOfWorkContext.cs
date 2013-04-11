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
	using Common;
	using Common.CodeContracts;
	using Base;
	using Events;
    using Exceptions;
    using Operations;
    using Policy;
	using Wiring;

    internal class UnitOfWorkContext : IUnitOfWork
    {
		public IDomainDataSession Session { get; protected set; }
		public StaleDataPolicy StaleDataPolicy { get; protected set; }
		public Option<IUnitOfWork> Parent { get; protected set; }

		public IUnitOfWork Root { get { return UnitOfWork.Root.ValueOrDefault; } }
		public bool IsRoot
        {
            get { return this == Root; }
        }

        public bool IsCompleted { get; protected set; }
        public bool IsCancelled { get; protected set; }
		public bool IsDisposed { get; protected set; }
		public bool IsFinished { get { return IsCompleted || IsCancelled; } }

		internal UnitOfWorkContext RootContext { get { return (UnitOfWorkContext)UnitOfWork.Root.ValueOrDefault; } }
		internal event Action WhenCompleted = () => { };
        public event Action Completed
        {
            add { RootContext.WhenCompleted += value; }
			remove { RootContext.WhenCompleted -= value; }
        }

        internal event Action WhenFailed = () => { };
        public event Action Failed
        {
			add { RootContext.WhenFailed += value; }
			remove { RootContext.WhenFailed -= value; }
        }

        internal event Action WhenCancelled = () => { };
        public event Action Cancelled
        {
			add { RootContext.WhenCancelled += value; }
			remove { RootContext.WhenCancelled -= value; }
        }

		internal UnitOfWorkContext(IUnitOfWork parent)
		{
			Parent = parent.AsOption();
			StaleDataPolicy = parent.StaleDataPolicy;
			Session = parent.Session;
		}

		internal UnitOfWorkContext(IDomainDataSession session, StaleDataPolicy staleDataPolicy)
		{
			Parent = Option<IUnitOfWork>.None;
			StaleDataPolicy = staleDataPolicy;
			Session = session;
        }

        protected void AssertIfFinished()
        {
            Assumes.True(!IsFinished, "This UnitOfWork is finished");
        }

        public void Cancel()
        {
            AssertIfFinished();

            IsCancelled = true;

            if (!IsRoot) return;

            Session.ForgetChanges();

            EventBus.Publish(new UnitOfWorkCancelled(this));
            WhenCancelled();
        }

        public void Complete()
        {
            AssertIfFinished();

            IsCompleted = true;

            if (!IsRoot) return;

            EventBus.Publish(new UnitOfWorkCompleting(this));
            var changeSet = Session.SaveChanges(StaleDataPolicy);
            if (changeSet.StaleDataDetected)
            {
				EventBus.Publish(new UnitOfWorkFailed(this, changeSet));
				WhenFailed();

	            if (StaleDataPolicy == StaleDataPolicy.Strict)
	            {
		            throw new StaleDataException(changeSet.StaleData,
		                                         "Some aggregates weren't saved due to stale data (version mismatch)");
	            }

				return;
            }

            EventBus.Publish(new UnitOfWorkCompleted(this, changeSet));
            WhenCompleted();
        }

        public virtual void Dispose()
        {
            if (IsDisposed) return;

            try
            {
                if (!IsFinished)
                    Complete();
            }
            finally
            {
                if (IsRoot)
                {
                    EventBus.Publish(new UnitOfWorkDisposing(this));
                    Session.Dispose();
                }

				if (Parent.IsSome && IsCancelled)
					Parent.Value.Cancel();

				UnitOfWork.Finish(this);

				IsDisposed = true;
				GC.SuppressFinalize(this);
			}
        }

        ~UnitOfWorkContext()
        {
            if (!IsDisposed)
                throw new InvalidOperationException("UnitOfWork must be properly disposed!");
        }

        public IRepository<TRoot> Query<TRoot>() where TRoot : class, IAggregateRoot
        {
            AssertIfFinished();

            return new Repository<TRoot>(Session.DataSession);
        }

        public IQueryable<TRoot> AllOf<TRoot>() where TRoot : class, IAggregateRoot
        {
            AssertIfFinished();

            return Query<TRoot>().All();
        }

        public TOp Using<TOp>() where TOp : class, IDomainOperation
        {
            AssertIfFinished();

            return UnitOfWork.OperationResolver.Get<TOp>();
        }
    }
}