using System.Web.Mvc;
using System.Web.Routing;

namespace Kostassoid.BlogNote.Web
{
    using System.Web;
    using Node;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static BlogNoteWebNode Node { get; private set; }

        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Node = new BlogNoteWebNode();
            Node.Start();
        }

        public override void Init()
        {
            base.Init();

            Node.AttachTo(this);
        }

        protected void Application_End()
        {
            if (Node != null) Node.Shutdown();
        }
    }
}