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

namespace Kostassoid.Anodyne.Common.Reflection
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Extentions;

    public static class From
    {
        public static IEnumerable<Assembly> ThisAssembly
        {
            get
            {
                return Assembly.GetCallingAssembly().AsEnumerable();
            }
        }

        public static IEnumerable<Assembly> ExecutingAssembly
        {
            get
            {
                return Assembly.GetExecutingAssembly().AsEnumerable();
            }
        }

        public static IEnumerable<Assembly> AllAssemblies() //todo: rename to AllAppDomainAssemblies ?
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }
       
        public static IEnumerable<FileInfo> AllFilesIn(string path, bool recursively = false)
        {
            return new DirectoryInfo(path)
                .EnumerateFiles("*.*", recursively ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        }

        public static IEnumerable<FileInfo> AllFilesInApplicationFolder()
        {
            var uri = new Uri(Assembly.GetExecutingAssembly().CodeBase);
            return AllFilesIn(Path.GetDirectoryName(uri.LocalPath));
        }
    }
}