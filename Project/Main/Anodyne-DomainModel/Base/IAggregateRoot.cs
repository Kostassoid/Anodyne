namespace Kostassoid.Anodyne.Domain.Base
{
    public interface IAggregateRoot : IEntity
    {
        object IdObject { get; }
        int Version { get; }
    }
}