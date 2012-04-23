using Kostassoid.Anodyne.Domain.Events;

namespace Kostassoid.Anodyne.DataAccess.Events
{
    public class UnitOfWorkCompletingEvent : IDomainEvent
    {
        public UnitOfWork UnitOfWork { get; protected set; }
        public UnitOfWorkCompletingEvent(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}