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
    using Common.ExecutionContext;
    using Node.Configuration;

    public static class ConfigurationEx
    {
        public static void UseHttpContext(this INodeConfigurator nodeConfigurator)
        {
            Context.SetProvider(new HttpContextProvider());

            //TODO: close open DataAccessContext session on request end
            //HttpContext.Current.ApplicationInstance.EndRequest += () => 
        }

        public static void UseOperationContext(this INodeConfigurator nodeConfigurator)
        {
            Context.SetProvider(new OperationContextProvider());
        }

    }

}