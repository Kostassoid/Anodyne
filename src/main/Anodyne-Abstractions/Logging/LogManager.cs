// Copyright 2011-2013 Anodyne.
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

namespace Kostassoid.Anodyne.Abstractions.Logging
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Logging manager.
    /// </summary>
    public static class LogManager
    {
        private static ILoggerAdapter _adapter = new NullLoggerAdapter();
        /// <summary>
        /// Current logger adapter instance.
        /// </summary>
        public static ILoggerAdapter Adapter
        {
            get { return _adapter; }
        }

        /// <summary>
        /// Get Logger for current class.
        /// </summary>
        /// <returns></returns>
        public static ILog GetCurrentClassLogger()
        {
            var frame = new StackFrame(1, false);
            var method = frame.GetMethod();
            var declaringType = method.DeclaringType;
            return Adapter.GetLogger(declaringType);
        }

        /// <summary>
        /// Set logger adapter.
        /// </summary>
        /// <param name="adapter">Logger adapter instance.</param>
        public static void SetAdapter(ILoggerAdapter adapter)
        {
            _adapter = adapter;
        }

        /// <summary>
        /// Get logger for specific type.
        /// </summary>
        /// <typeparam name="T">Type for which logger is required.</typeparam>
        /// <returns>Logger for specified type.</returns>
        public static ILog GetLogger<T>()
        {
            return Adapter.GetLogger(typeof(T));
        }

        /// <summary>
        /// Get logger for specific type.
        /// </summary>
        /// <returns>Logger for specified type.</returns>
        public static ILog GetLogger(Type type)
        {
            return Adapter.GetLogger(type);
        }

        /// <summary>
        /// Get logger with any string as a source.
        /// </summary>
        /// <returns>Logger.</returns>
        public static ILog GetLogger(string name)
        {
            return Adapter.GetLogger(name);
        }
         
    }
}