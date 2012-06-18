// Copyright 2011-2012 Anodyne.
//   
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use 
// this file except in compliance with the License. You may obtain a copy of the 
// License at 
// 
//      http://www.apache.org/licenses/LICENSE-2.0 
//  
// Unless required by applicable law or agreed to in writing, software distributed 
// under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR 
// CONDITIONS OF ANY KIND, either express or implied. See the License for the 
// specific language governing permissions and limitations under the License.

namespace Kostassoid.Anodyne.Log4Net
{
    using Node.Logging;
    using System;

    public class Log4NetLog : ILog
    {
        private readonly log4net.ILog _internalLog;

        public Log4NetLog(string source)
        {
            _internalLog = log4net.LogManager.GetLogger(source);
        }

        public void Debug(object message)
        {
            _internalLog.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            _internalLog.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            _internalLog.Debug(string.Format(format, args));
        }

        public void DebugFormat(string format, Exception exception, params object[] args)
        {
            _internalLog.Debug(string.Format(format, args), exception);
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _internalLog.Debug(string.Format(formatProvider, format, args));
        }

        public void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            _internalLog.Debug(string.Format(formatProvider, format, args), exception);
        }

        public void Info(object message)
        {
            _internalLog.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _internalLog.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            _internalLog.Info(string.Format(format, args));
        }

        public void InfoFormat(string format, Exception exception, params object[] args)
        {
            _internalLog.Info(string.Format(format, args), exception);
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _internalLog.Info(string.Format(formatProvider, format, args));
        }

        public void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            _internalLog.Info(string.Format(formatProvider, format, args), exception);
        }

        public void Warn(object message)
        {
            _internalLog.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _internalLog.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            _internalLog.Warn(string.Format(format, args));
        }

        public void WarnFormat(string format, Exception exception, params object[] args)
        {
            _internalLog.Warn(string.Format(format, args), exception);
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _internalLog.Warn(string.Format(formatProvider, format, args));
        }

        public void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            _internalLog.Warn(string.Format(formatProvider, format, args), exception);
        }

        public void Error(object message)
        {
            _internalLog.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _internalLog.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            _internalLog.Error(string.Format(format, args));
        }

        public void ErrorFormat(string format, Exception exception, params object[] args)
        {
            _internalLog.Error(string.Format(format, args), exception);
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _internalLog.Error(string.Format(formatProvider, format, args));
        }

        public void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            _internalLog.Error(string.Format(formatProvider, format, args), exception);
        }

        public void Fatal(object message)
        {
            _internalLog.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _internalLog.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            _internalLog.Fatal(string.Format(format, args));
        }

        public void FatalFormat(string format, Exception exception, params object[] args)
        {
            _internalLog.Fatal(string.Format(format, args), exception);
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, params object[] args)
        {
            _internalLog.Fatal(string.Format(formatProvider, format, args));
        }

        public void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args)
        {
            _internalLog.Fatal(string.Format(formatProvider, format, args), exception);
        }

        public bool IsDebugEnabled { get { return _internalLog.IsDebugEnabled; } }
        public bool IsErrorEnabled { get { return _internalLog.IsErrorEnabled; } }
        public bool IsFatalEnabled { get { return _internalLog.IsFatalEnabled; } }
        public bool IsInfoEnabled { get { return _internalLog.IsInfoEnabled; } }
        public bool IsWarnEnabled { get { return _internalLog.IsWarnEnabled; } }
    }
}