namespace Kostassoid.Anodyne.Common.Measure
{
    public class DigitalStorageSize
    {
        public long Bytes { get; private set; }

        public DigitalStorageSize(long bytes)
        {
            Bytes = bytes;
        }
    }
}