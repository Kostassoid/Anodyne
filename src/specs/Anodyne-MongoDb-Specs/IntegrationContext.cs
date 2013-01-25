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

using Kostassoid.Anodyne.Node.Configuration;
using Kostassoid.Anodyne.Windsor;

namespace Kostassoid.Anodyne.MongoDb.Specs
{
    using Abstractions.DataAccess;
    using Anodyne.Domain.DataAccess;

    public static class IntegrationContext
    {
        public static Node.Node System;
        public static IDataAccessContext DataContext { get; set; }

        class TestSystem : Node.Node
        {
            public override void OnConfigure(INodeConfigurator c)
            {
                c.UseWindsorContainer();
                c.ForDataAccess()
                    .Use(MongoProvider.Instance("localhost:27001", "Anodyne-Testing"))
                    .AsDomainStorage();
                c.ForDataAccess("ReadModel")
                    .Use(MongoProvider.Instance("localhost:27001", "Anodyne-Testing-Read"))
                    .AsInjectedContext();

                c.OnStartupPerform(i =>
                    {
                        i.DefaultDataAccess.OnNative(d => d.Drop());
                        i.GetDataAccessProvider("ReadModel").OnNative(d => d.Drop());
                    });
            }
        }

        public static void Init()
        {
            System = new TestSystem();
            System.Start();

            DataContext = System.Configuration.Container.Get<IDataAccessContext>();
        }

    }
}