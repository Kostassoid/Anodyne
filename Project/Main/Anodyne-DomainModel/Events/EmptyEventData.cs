namespace Kostassoid.Anodyne.Domain.Events
{
    public class EmptyEventData : EventData
    {
        public override int GetHashCode()
        {
            return 0;
        }

        public override bool Equals(object obj)
        {
            return true;
        }
    }
}