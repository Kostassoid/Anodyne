using System;
using System.ComponentModel;

namespace Kostassoid.Anodyne.Common
{
    public static class SystemTime
    {
        private static readonly ITimeController Controller = new InternalTimeController();

        public static DateTime Now
        {
            get { return Controller.CurrentDateTime; }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ITimeController TimeController
        {
            get { return Controller; }
        }

        #region Nested type: InternalTimeController

        private class InternalTimeController : ITimeController
        {
            private readonly static Func<DateTime> DefaultSystemDateFunc = () => DateTime.Now;

            private Func<DateTime> _dateFunc = DefaultSystemDateFunc;

            #region ITimeController Members

            public DateTime CurrentDateTime
            {
                get { return _dateFunc().ToUniversalTime(); }
            }

            public void Customize(Func<DateTime> func)
            {
                _dateFunc = func;
            }

            public void SetDate(DateTime date)
            {
                var whnStd = DefaultSystemDateFunc();
                Func<DateTime> func = () => date + (DefaultSystemDateFunc() - whnStd);
                _dateFunc = func;
            }

            public void SetFrozenDate(DateTime date)
            {
                _dateFunc = () => date;
            }

            public void Reset()
            {
                _dateFunc = DefaultSystemDateFunc;
            }

            #endregion
        }

        #endregion
    }

    public interface ITimeController
    {
        DateTime CurrentDateTime { get; }
        void Customize(Func<DateTime> dateFunc);
        void SetDate(DateTime date);
        void SetFrozenDate(DateTime date);
        void Reset();
    }
}