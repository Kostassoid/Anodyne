namespace Kostassoid.Anodyne.Common.Measure
{
    public static class Int64Ex
    {
        public static DigitalStorageSize Bytes(this long number)
        {
            return new DigitalStorageSize(number);
        }

        public static DigitalStorageSize Kilobytes(this long number)
        {
            return new DigitalStorageSize(number * 1024);
        }

        public static DigitalStorageSize Megabytes(this long number)
        {
            return new DigitalStorageSize(number * 1024 * 1024);
        }
    }
}