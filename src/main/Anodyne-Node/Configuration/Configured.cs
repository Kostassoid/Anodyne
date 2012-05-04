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

namespace Kostassoid.Anodyne.Node.Configuration
{
    using global::System;
    using global::System.Configuration;

    public static class Configured
    {
        public class Specification
        {
            public bool Default { get; protected set; }
            public string Value { get; protected set; }

            public Specification()
            {
                Default = true;
            }

            public Specification(string value)
            {
                Default = false;
                Value = value;
            }
        }

        public class ValueSelector
        {
            public string AppSettings(string key)
            {
                return ConfigurationManager.AppSettings[key];
            }
            public Tuple<string, string> AppSettings(string key1, string key2)
            {
                return new Tuple<string, string>(
                    ConfigurationManager.AppSettings[key1],
                    ConfigurationManager.AppSettings[key2]);
            }
            public Tuple<string, string, string> AppSettings(string key1, string key2, string key3)
            {
                return new Tuple<string, string, string>(
                    ConfigurationManager.AppSettings[key1],
                    ConfigurationManager.AppSettings[key2],
                    ConfigurationManager.AppSettings[key3]);
            }
        }

        public static Specification AsDefault { get { return new Specification(); }}
        public static ValueSelector From { get { return new ValueSelector(); }}
    }
}