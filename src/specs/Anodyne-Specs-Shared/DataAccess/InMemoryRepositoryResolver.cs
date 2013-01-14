using Kostassoid.Anodyne.DataAccess;
using Kostassoid.Anodyne.Domain.Base;
using Kostassoid.Anodyne.Domain.DataAccess;
using Kostassoid.Anodyne.Domain.DataAccess.Operations;

namespace Kostassoid.Anodyne.Specs.Shared.DataAccess
{
    public class InMemoryRepositoryResolver : IRepositoryResolver
    {
        public IRepository<TRoot> Get<TRoot>(IDataSession dataSession) where TRoot : class, IAggregateRoot
        {
            return new InMemoryRepository<TRoot>(((InMemoryDataSession)dataSession).Roots);
        }
    }
}