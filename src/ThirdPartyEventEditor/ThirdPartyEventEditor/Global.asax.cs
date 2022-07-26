using Autofac;
using Autofac.Integration.Mvc;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ThirdPartyEventEditor.Services.Implementations;
using ThirdPartyEventEditor.Services.Interfaces;

namespace ThirdPartyEventEditor
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            this.RegisterAutoFac();

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        private void RegisterAutoFac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterControllers(typeof(Global).Assembly);

            builder.RegisterType<FileEventStorage>().As<IEventStorage>().SingleInstance();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}