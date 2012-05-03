namespace Kostassoid.BlogNote.Web.Installers
{
    using System.Configuration;
    using System.ServiceModel;
    using Castle.Facilities.WcfIntegration;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;
    using Contracts;

    public class WcfClientsInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var userServiceUrl = ConfigurationManager.AppSettings["UserServiceUrl"];

            container.AddFacility<WcfFacility>();
            container.Register(Component.For<IUserService>().AsWcfClient(WcfEndpoint.BoundTo(new BasicHttpBinding()).At(userServiceUrl)));
        }
    }
}