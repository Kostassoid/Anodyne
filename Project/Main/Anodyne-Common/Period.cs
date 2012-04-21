using System;

namespace Kostassoid.Anodyne.Common
{
    [Serializable]
    public class Period
    {
        public DateTime Starting { get; protected set; }
        public DateTime Ending { get; protected set; }

        public Period(DateTime starting, DateTime ending)
        {
            Starting = starting;
            Ending = ending;
        }

        public Period(DateTime starting, TimeSpan lasting)
        {
            Starting = starting;
            Ending = starting + lasting;
        }
    }
}