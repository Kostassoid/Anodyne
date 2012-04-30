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

using Kostassoid.Anodyne.Windsor;

namespace Kostassoid.BlogNote.Host.System
{
    using Anodyne.Log4Net;
    using Anodyne.MongoDb;
    using Anodyne.System;
    using Anodyne.System.Configuration;

    public class BlogNoteSystem : AnodyneSystem
    {
        public override void OnConfigure(IConfiguration c)
        {
            c.UseLog4NetLogger();
            c.UseWindsorContainer();
            c.UseMongoDataAccess(Configured.From.AppSettings("DatabaseServer"), Configured.From.AppSettings("DatabaseName"));
            //c.DiscoverDataOperations(From.Assemblies(a => a.FullName.Contains("BlogNote")));
            //c.DiscoverBootstrappers(From.Assemblies(a => a.FullName.Contains("BlogNote")));
        }

    }
}