using System;
using Kostassoid.Anodyne.Domain.Base;

namespace Kostassoid.Anodyne.DataAccess
{
    public interface IDataSession : IDisposable
    {
        IRepository<TRoot> GetRepository<TRoot>() where TRoot : class, IAggregateRoot;
        TOp GetOperation<TOp>() where TOp : class, IDataOperation;

        object MarkAsCreated<TRoot>(TRoot entity) where TRoot : class, IAggregateRoot;
        void MarkAsDeleted<TRoot>(TRoot entity) where TRoot : class, IAggregateRoot;
        void MarkAsUpdated<TRoot>(TRoot entity) where TRoot : class, IAggregateRoot;

        void SaveChanges();
        void Rollback();
    }
}