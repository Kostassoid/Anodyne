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

namespace Kostassoid.BlogNote.Host.Startup
{
    using Anodyne.Common.Reflection;
    using Anodyne.MongoDb;
    using Anodyne.System;
    using Anodyne.System.Configuration;
    using Domain;

    public class DataAccessConfiguration : IStartupAction
    {
        public void OnStartup(IConfigurationSettings configuration)
        {
            configuration.DataAccess
                .OnNative(db =>
                {
                    db.MapAllClassesBasedOn<BasePostContent>(From.ThisAssembly);
                    db.EnsureUniqueIndexFor<User>(u => u.Name);
                });
        }
    }
}