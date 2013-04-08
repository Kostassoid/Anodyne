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
	using Common.CodeContracts;
	using Base;
	using Events;
    using Exceptions;
    using Operations;
    using Policy;
	using Wiring;

    public class UnitOfWorkInstance : IUnitOfWork
    {
		public IDomainDataSession DomainDataSession { get; protected set; }
		public StaleDataPolicy StaleDataPolicy { get; protected set; }
		public IUnitOfWork Parent { get; protected set; }

		public IUnitOfWork Root { get { return UnitOfWork.Root.ValueOrDefault; } }
		public bool IsRoot
        {
            get { return this == Root; }
        }

        public bool IsCompleted { get; protected set; }
        public bool IsCancelled { get; protected set; }
		public bool IsDisposed { get; protected set; }
		public bool IsFinished { get { return IsCompleted || IsCancelled; } }

		internal UnitOfWorkInstance RootInstance { get { return (UnitOfWorkInstance)UnitOfWork.Root.ValueOrDefault; } }
		internal event Action WhenCompleted = () => { };
        public event Action Completed
        {
            add { RootInstance.WhenCompleted += value; }
			remove { RootInstance.WhenCompleted -= value; }
        }

        internal event Action WhenFailed = () => { };
        public event Action Failed
        {
			add { RootInstance.WhenFailed += value; }
			remove { RootInstance.WhenFailed -= value; }
        }

        internal event Action WhenCancelled = () => { };
        public event Action Cancelled
        {
			add { RootInstance.WhenCancelled += value; }
			remove { RootInstance.WhenCancelled -= value; }
        }

		internal UnitOfWorkInstance(IUnitOfWork parent)
		{
			Parent = parent;
			StaleDataPolicy = parent.StaleDataPolicy;
			DomainDataSession = parent.DomainDataSession;
		}

		internal UnitOfWorkInstance(IDomainDataSession session, StaleDataPolicy staleDataPolicy)
        {
			StaleDataPolicy = staleDataPolicy;
			DomainDataSession = session;
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

            DomainDataSession.ForgetChanges();

            EventBus.Publish(new UnitOfWorkCancelled(this));
            WhenCancelled();
        }

        public void Complete()
        {
            AssertIfFinished();

            IsCompleted = true;

            if (!IsRoot) return;

            EventBus.Publish(new UnitOfWorkCompleting(this));
            var changeSet = DomainDataSession.SaveChanges(StaleDataPolicy);
            if (changeSet.StaleDataDetected && StaleDataPolicy == StaleDataPolicy.Strict)
            {
                EventBus.Publish(new UnitOfWorkFailed(this, changeSet));
                WhenFailed();
                //TODO: make exception optional or obsolete?
                throw new StaleDataException(changeSet.StaleData, "Some aggregates weren't saved due to stale data (version mismatch)");
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
                    DomainDataSession.Dispose();
                }

				if (Parent != null && IsCancelled)
					Parent.Cancel();

				UnitOfWork.Close(this);

				IsDisposed = true;
            }

			GC.SuppressFinalize(this);
        }

        ~UnitOfWorkInstance()
        {
            if (!IsDisposed)
                throw new InvalidOperationException("UnitOfWork must be properly disposed!");
        }

        public IRepository<TRoot> Query<TRoot>() where TRoot : class, IAggregateRoot
        {
            AssertIfFinished();

            return new Repository<TRoot>(DomainDataSession.DataSession);
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

        public void MarkAsDeleted<TRoot>(TRoot entity) where TRoot : class, IAggregateRoot
        {
            AssertIfFinished();

            DomainDataSession.MarkAsDeleted(entity);
        }

    }
}