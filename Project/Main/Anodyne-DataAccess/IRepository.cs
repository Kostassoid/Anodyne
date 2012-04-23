using System;
using System.Linq;
using System.Linq.Expressions;
using Kostassoid.Anodyne.Common;
using Kostassoid.Anodyne.Domain.Base;

namespace Kostassoid.Anodyne.DataAccess
{
    public interface IRepository<TEntity> where TEntity : class, IAggregateRoot
    {
        TEntity this[object key] { get; }
        TEntity Get(object key);
        Option<TEntity> FindBy(object key);
        IQueryable<TEntity> All();
        bool Exists(object key);
        long Count(Expression<Func<TEntity, bool>> criteria);
        long Count();
    }
}