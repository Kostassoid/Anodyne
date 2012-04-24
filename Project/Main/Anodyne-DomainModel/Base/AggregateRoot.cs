namespace Kostassoid.Anodyne.Domain.Base
{
    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
    {
        virtual public object IdObject { get { return Id; } }
        virtual public int Version { get; protected set; }

        virtual public int NewVersion()
        {
            return Version++;
        }
    }
}