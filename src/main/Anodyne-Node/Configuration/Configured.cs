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

namespace Kostassoid.Anodyne.Node.Configuration
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Provides configuration parameters values in fluent (dsl) fashion.
    /// </summary>
    public static class Configured
    {
        /// <summary>
        /// Configuration parameter value.
        /// </summary>
        public class ConfigurationParamValue
        {
            /// <summary>
            /// Should use default value for this parameter.
            /// </summary>
            public bool Default { get; protected set; }
            /// <summary>
            /// Configuration parameter value
            /// </summary>
            public string Value { get; protected set; }

            internal ConfigurationParamValue()
            {
                Default = true;
            }

            internal ConfigurationParamValue(string value)
            {
                Default = false;
                Value = value;
            }
        }

        /// <summary>
        /// Extracts configuration values from common sources.
        /// </summary>
        public class ValueSelector
        {
            /// <summary>
            /// Extract value from AppSettings section of config file.
            /// </summary>
            /// <param name="key">AppSettings key, as specified in config file.</param>
            /// <returns></returns>
            public string AppSettings(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }
            /// <summary>
            /// Extract two values from AppSettings section of config file.
            /// </summary>
            /// <param name="key1">AppSettings key, as specified in config file.</param>
            /// <param name="key2">AppSettings key, as specified in config file.</param>
            /// <returns>Tuple containing both values, or null for non existing keys.</returns>
            public Tuple<string, string> AppSettings(string key1, string key2)
            {
                return new Tuple<string, string>(
                    ConfigurationManager.AppSettings[key1],
                    ConfigurationManager.AppSettings[key2]);
            }
            /// <summary>
            /// Extract three values from AppSettings section of config file.
            /// </summary>
            /// <param name="key1">AppSettings key, as specified in config file.</param>
            /// <param name="key2">AppSettings key, as specified in config file.</param>
            /// <param name="key3">AppSettings key, as specified in config file.</param>
            /// <returns>Tuple containing both values, or null for non existing keys.</returns>
            public Tuple<string, string, string> AppSettings(string key1, string key2, string key3)
            {
                return new Tuple<string, string, string>(
                    ConfigurationManager.AppSettings[key1],
                    ConfigurationManager.AppSettings[key2],
                    ConfigurationManager.AppSettings[key3]);
            }
        }

        /// <summary>
        /// Use default value for this parameter.
        /// </summary>
        public static ConfigurationParamValue AsDefault { get { return new ConfigurationParamValue(); }}
        /// <summary>
        /// Extract value from common source.
        /// </summary>
        public static ValueSelector From { get { return new ValueSelector(); }}
    }
}