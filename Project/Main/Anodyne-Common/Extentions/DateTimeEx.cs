using System;

namespace Kostassoid.Anodyne.Common.Extentions
{
    public static class DateTimeEx
    {
        private const int PrecisionInMilliseconds = 1000;
        public static bool SoftEquals(this DateTime dateTime, DateTime compareTo, int precision = PrecisionInMilliseconds)
        {
            return Math.Abs((dateTime - compareTo).TotalMilliseconds) < precision;
        }

        public static DateTime StripMilliseconds(this DateTime dateTime)
        {
            return dateTime.AddMilliseconds(-dateTime.Millisecond).AddTicks(-dateTime.Ticks);
        }
    }
}