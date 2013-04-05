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
	using Abstractions.DataAccess;
    using Common;
    using Common.CodeContracts;
    using Common.ExecutionContext;
    using Common.Reflection;
    using Base;
    using Events;
    using Exceptions;
    using Operations;
    using Policy;
    using Domain.Events;
    using Wiring;

    //TODO: refactor this ugly pile of code
    public class UnitOfWork : IUnitOfWorkEx, IDisposable
    {
        private const string HeadContextKey = "head-unit-of-work";

        private static IDataSessionFactory _dataSessionFactory;
        private static IOperationResolver _operationResolver;
        private static DataAccessPolicy _policy = new DataAccessPolicy();

        private static bool _eventHandlersAreSet;

        private readonly string _contextKey = HeadContextKey;
        private readonly UnitOfWork _parent;

        public UnitOfWork Head { get; protected set; }

        public IDomainDataSession DomainDataSession { get; protected set; }

        private readonly StaleDataPolicy _staleDataPolicy = StaleDataPolicy.Strict;

        public static Option<UnitOfWork> Current
        {
            get { return Context.FindAs<UnitOfWork>(HeadContextKey); }
            protected set { Context.Set(HeadContextKey, value.ValueOrDefault); }
        }

        public bool IsRoot
        {
            get { return _parent == null; }
        }

        public bool IsCompleted { get; protected set; }
        public bool IsCancelled { get; protected set; }
        public bool IsFinished { get { return IsCompleted || IsCancelled; } }

        public bool IsDisposed
        {
            get { return Context.Find(_contextKey).IsNone; }
        }

        public static bool IsConfigured { get { return _dataSessionFactory != null; } }

        public static void SetDataSessionFactory(IDataSessionFactory dataSessionFactory)
        {
            _dataSessionFactory = dataSessionFactory;
        }

        public static void SetOperationResolver(IOperationResolver operationResolver)
        {
            _operationResolver = operationResolver;
        }

        public static void EnforcePolicy(DataAccessPolicy policy)
        {
            _policy = policy;
        }

        public UnitOfWork(StaleDataPolicy? staleDataPolicy = null)
        {
            lock (HeadContextKey) EnsureAggregateEventHandlersAreSet();

            if (Current.IsSome)
            {
                _parent = Current.Value;
                Head = Current.Value.Head;
                DomainDataSession = _parent.DomainDataSession;

                _contextKey = String.Format("{0}-{1}", _contextKey, Guid.NewGuid().ToString("N"));
            }
            else
            {
                DomainDataSession = new DomainDataSession(_dataSessionFactory.Open());
                if (DomainDataSession == null)
                    throw new Exception("Unable to create IDataSession (bad configuration?)");

                Head = this;
            }

            _staleDataPolicy = staleDataPolicy.HasValue
                ? staleDataPolicy.Value
                : _policy.StaleDataPolicy;

            Context.Set(_contextKey, this);
            Current = this;
        }

        protected static void EnsureAggregateEventHandlersAreSet()
        {
            if (_eventHandlersAreSet) return;
            _eventHandlersAreSet = true;

            EventBus
                .SubscribeTo()
                .AllBasedOn<IAggregateEvent>(From.AllAssemblies())
                .With(e =>
                {
                    if (_policy.ReadOnly)
                        throw new InvalidOperationException("You can't mutate AggregateRoots in ReadOnly mode.");

                    if (Current.IsSome && !Current.Value.IsFinished)
                    {
                        ((UnitOfWork) Current).DomainDataSession.Handle(e);
                    }
                    else
                    {
                        throw new InvalidOperationException("There's no active UnitOfWork.");
                    }
                }, Priority.Exact(1000));
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
        }

        public void Complete()
        {
            AssertIfFinished();

            IsCompleted = true;

            if (!IsRoot) return;

            EventBus.Publish(new UnitOfWorkCompleting(this));
            var changeSet = DomainDataSession.SaveChanges(_staleDataPolicy);
            EventBus.Publish(new UnitOfWorkCompleted(this, changeSet));

            if (changeSet.StaleDataDetected && _staleDataPolicy == StaleDataPolicy.Strict)
                throw new StaleDataException(changeSet.StaleData, "Some aggregates weren't saved due to stale data (version mismatch)");
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

                Context.Release(_contextKey);
                Current = _parent;

                if (Current.IsSome && IsCancelled)
                    Current.Value.Cancel();
            }
        }

        ~UnitOfWork()
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

            return _operationResolver.Get<TOp>();
        }

        public void MarkAsDeleted<TRoot>(TRoot entity) where TRoot : class, IAggregateRoot
        {
            AssertIfFinished();

            DomainDataSession.MarkAsDeleted(entity);
        }

    }
}