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

namespace Kostassoid.BlogNote.Host
{
    using Topshelf;
    using System.IO;
    using log4net;
    using log4net.Config;

    class Program
    {
        static void Main(string[] args)
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo("log4net.config"));

            var logger = LogManager.GetLogger(typeof(Program));

            const string serviceName = Const.ProjectName + "-Host";

            var h = HostFactory.New(x =>
            {
                x.UseLog4Net();

                x.BeforeStartingServices(s => logger.InfoFormat("Staring service {0}...", serviceName));
                x.AfterStartingServices(s => logger.InfoFormat("Service {0} started.", serviceName));
                x.AfterStoppingServices(s => logger.InfoFormat("Service {0} stopped.", serviceName));

                x.Service<BlogNoteSystem>(s =>
                {
                    s.SetServiceName(serviceName);
                    s.ConstructUsing(name => new BlogNoteSystem());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Shutdown());
                });
                x.RunAsNetworkService();

                x.SetDescription(Const.ProjectName + " Host Service");
                x.SetDisplayName(serviceName);
                x.SetServiceName(serviceName);
            });

            h.Run();

        }
    }
}
