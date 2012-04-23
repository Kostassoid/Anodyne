using Kostassoid.Anodyne.Domain.Events;

namespace Kostassoid.Anodyne.DataAccess.Events
{
    public class UnitOfWorkDisposingEvent : IDomainEvent
    {
        public UnitOfWork UnitOfWork { get; protected set; }
        public UnitOfWorkDisposingEvent(UnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}