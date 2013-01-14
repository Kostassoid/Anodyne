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

namespace Kostassoid.Anodyne.Web
{
    using System.Web;
    using DataAccess;
    using Node.Configuration;

    public abstract class WebNode : Node.Node
    {
        private readonly HttpApplication _application;

        protected WebNode(HttpApplication application)
        {
            _application = application;

            Instance.OnContainerReady += () =>
                {
                    var cfg = ((IConfiguration) Instance);

                    cfg.UseHttpContext();

                    _application.EndRequest += (sender, args) => Instance.Container.Get<IDataAccessContext>().CloseCurrentSession();
                };
        }
    }
}