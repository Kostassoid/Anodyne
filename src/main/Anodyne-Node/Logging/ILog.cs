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
// 

namespace Kostassoid.Anodyne.Node.Logging
{
    using global::System;

    public interface ILog
    {
        void Debug(object message);
        void Debug(object message, Exception exception);
        void DebugFormat(string format, params object[] args);
        void DebugFormat(string format, Exception exception, params object[] args);
        void DebugFormat(IFormatProvider formatProvider, string format, params object[] args);
        void DebugFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args);
        void Info(object message);
        void Info(object message, Exception exception);
        void InfoFormat(string format, params object[] args);
        void InfoFormat(string format, Exception exception, params object[] args);
        void InfoFormat(IFormatProvider formatProvider, string format, params object[] args);
        void InfoFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args);
        void Warn(object message);
        void Warn(object message, Exception exception);
        void WarnFormat(string format, params object[] args);
        void WarnFormat(string format, Exception exception, params object[] args);
        void WarnFormat(IFormatProvider formatProvider, string format, params object[] args);
        void WarnFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args);
        void Error(object message);
        void Error(object message, Exception exception);
        void ErrorFormat(string format, params object[] args);
        void ErrorFormat(string format, Exception exception, params object[] args);
        void ErrorFormat(IFormatProvider formatProvider, string format, params object[] args);
        void ErrorFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args);
        void Fatal(object message);
        void Fatal(object message, Exception exception);
        void FatalFormat(string format, params object[] args);
        void FatalFormat(string format, Exception exception, params object[] args);
        void FatalFormat(IFormatProvider formatProvider, string format, params object[] args);
        void FatalFormat(IFormatProvider formatProvider, string format, Exception exception, params object[] args);
    }
}