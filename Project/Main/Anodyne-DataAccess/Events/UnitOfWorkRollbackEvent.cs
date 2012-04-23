using Kostassoid.Anodyne.Domain.Events;

namespace Kostassoid.Anodyne.DataAccess.Events
{
    public class UnitOfWorkRollbackEvent : IDomainEvent
    {
        public UnitOfWork UnitOfWork { get; protected set; }
        public UnitOfWorkRollbackEvent(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}