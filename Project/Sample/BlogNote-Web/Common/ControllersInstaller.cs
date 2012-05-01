namespace Kostassoid.BlogNote.Web.Common
{
    using System.Web.Mvc;
    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    public class ControllersInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                AllTypes.FromThisAssembly().BasedOn<IController>()
                    //.If(Component.IsInSameNamespaceAs<HomeController>())
                    .If(t => t.Name.EndsWith("Controller")).LifestyleTransient());
        }
    }
}