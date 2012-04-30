namespace Kostassoid.Anodyne.Log4Net
{
    using System.Logging;
    using global::System;

    public class Log4NetLoggerAdapter : ILoggerAdapter
    {
        public Log4NetLoggerAdapter()
        {
        }

        public ILog GetLogger(Type type)
        {
            return new Log4NetLog(type.Name);
        }

        public ILog GetLogger(string source)
        {
            return new Log4NetLog(source);
        }
    }
}