namespace Kostassoid.Anodyne.Domain.Base
{
    public interface IAggregateRoot : IEntity
    {
        int Version { get; }
    }
}