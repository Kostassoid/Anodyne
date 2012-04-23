using System;
using Kostassoid.Anodyne.Common;
using Kostassoid.Anodyne.Common.CodeContracts;
using Kostassoid.Anodyne.Common.ExecutionContext;
using Kostassoid.Anodyne.DataAccess.Events;
using Kostassoid.Anodyne.Domain.Base;
using Kostassoid.Anodyne.Wiring;

namespace Kostassoid.Anodyne.DataAccess
{
    public class UnitOfWork: IDisposable
    {
        private static IDataSessionFactory _dataSessionFactory;

        private const string RootContextKey = "unit-of-work";
        private const string HeadContextKey = "head-unit-of-work";

        private readonly string _contextKey = RootContextKey;
        private readonly UnitOfWork _parent;

        public IDataSession DataSession { get; protected set; }

        public static Option<UnitOfWork> Current
        {
            get { return Context.FindAs<UnitOfWork>(HeadContextKey); }
            protected set { Context.Set(HeadContextKey, value); }
        }

        public bool IsRoot { get { return _parent == null; } }
        public bool IsFinished { get; protected set; }
        public bool IsDisposed { get { return Context.Find(_contextKey).IsNone; } }

        public static void SetFactory(IDataSessionFactory dataSessionFactory)
        {
            _dataSessionFactory = dataSessionFactory;
        }

        public UnitOfWork()
        {
            var parentUnitOfWork = Context.FindAs<UnitOfWork>(_contextKey);
            if (parentUnitOfWork.IsSome)
            {
                _parent = parentUnitOfWork.Value;
                DataSession = _parent.DataSession;

                _contextKey = String.Format("{0}-{1}", _contextKey, Guid.NewGuid().ToString("N"));
            }
            else
            {
                DataSession = _dataSessionFactory.OpenSession();
                if (DataSession == null)
                    throw new Exception("Unable to create IDataSession (bad configuration?)");
            }

            Context.Set(_contextKey, this);
            Current = this;

            IsFinished = false;
        }

        protected void AssertIfFinished()
        {
            Assumes.True(!IsFinished, "This UnitOfWork is finished");
        }

        public void Complete()
        {
            AssertIfFinished();

            IsFinished = true;

            if (!IsRoot) return;

            EventRouter.Fire(new UnitOfWorkCompletingEvent(this));
            DataSession.SaveChanges();
        }

        public void Rollback()
        {
            AssertIfFinished();

            IsFinished = true;

            if (!IsRoot) return;

            EventRouter.Fire(new UnitOfWorkRollbackEvent(this));
            DataSession.Rollback();
        }

        public void Dispose()
        {
            if (!IsFinished)
                Complete(); //complete by default

            if (IsDisposed)
                return;

            Context.Release(_contextKey);
            Current = _parent;

            if (IsRoot)
            {
                EventRouter.Fire(new UnitOfWorkDisposingEvent(this));
                DataSession.Dispose();
            }
        }

        ~UnitOfWork()
        {
            if (!IsDisposed)
                throw new InvalidOperationException("UnitOfWork must be properly disposed!");
        }

        public IRepository<TEntity> Query<TEntity>() where TEntity : class, IAggregateRoot
        {
            AssertIfFinished();

            return DataSession.GetRepository<TEntity>();
        }

        public TOp Using<TOp>() where TOp : class, IDataOperation
        {
            AssertIfFinished();

            return DataSession.GetOperation<TOp>();
        }

        public TEntity MarkAsCreated<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot 
        {
            AssertIfFinished();

            DataSession.MarkAsCreated(entity);
            return entity;
        }

        public void MarkAsDeleted<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
        {
            AssertIfFinished();

            DataSession.MarkAsDeleted(entity);
        }

        public void MarkAsUpdated<TEntity>(TEntity entity) where TEntity : class, IAggregateRoot
        {
            AssertIfFinished();

            DataSession.MarkAsUpdated(entity);
        }

    }
}