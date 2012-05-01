namespace Kostassoid.BlogNote.Web.Installers
{
    using System.Configuration;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using MongoDB.Driver;

    public class PersistanceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var databaseServer = ConfigurationManager.AppSettings["DatabaseServer"];
            var databaseName = ConfigurationManager.AppSettings["DatabaseName"];
            container.Register(
                Component.For<MongoDatabase>()
                .UsingFactoryMethod(t => MongoServer.Create(BuildConnectionString(databaseServer))
                    .GetDatabase(databaseName)).LifeStyle.Singleton);
        }

        private string BuildConnectionString(string databaseServer)
        {
            return "mongodb://" + databaseServer;
        }
    }
}