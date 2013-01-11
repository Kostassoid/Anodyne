using Kostassoid.Anodyne.DataAccess;
using Kostassoid.Anodyne.DataAccess.Domain;
using Kostassoid.Anodyne.DataAccess.Domain.Operations;
using Kostassoid.Anodyne.Domain.Base;

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