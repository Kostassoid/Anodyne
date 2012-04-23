namespace Kostassoid.Anodyne.Domain.Base
{
    public class Entity<TKey> : IEntity
    {
        public TKey Id { get; protected set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() != GetType()) return false;

            return ((Entity<TKey>)obj).Id.Equals(Id);
        }

        public override int GetHashCode()
        {
            return Id.Equals(default(TKey)) ? base.GetHashCode() : Id.GetHashCode();
        }
    }
}