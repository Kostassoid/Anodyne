namespace Kostassoid.Anodyne.Common.Measure
{
    public static class Int32Ex
    {
        public static DigitalStorageSize Bytes(this int number)
        {
            return new DigitalStorageSize(number);
        }

        public static DigitalStorageSize Kilobytes(this int number)
        {
            return new DigitalStorageSize(number * 1024);
        }

        public static DigitalStorageSize Megabytes(this int number)
        {
            return new DigitalStorageSize(number * 1024 * 1024);
        }
    }
}