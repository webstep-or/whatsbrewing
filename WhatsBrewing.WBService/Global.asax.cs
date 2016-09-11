using System.Web.Http;
using Microsoft.Azure;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using WhatsBrewing.DAL;
using WhatsBrewing.Importer;

namespace WhatsBrewing.WBService
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

            // IOC logic
            var container = new Container();

            // Register unit of work
            container.RegisterWebApiRequest<IUnitOfwork>(CreateUoW, disposeWhenScopeEnds: true);
            container.RegisterWebApiRequest<IImporter, Importer.Importer>(disposeWhenScopeEnds: true);

            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            GlobalConfiguration.Configuration.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private Context CreateContext()
        {           
            return new Context(CloudConfigurationManager.GetSetting("SqlServerConnectionString"));
        }

        private IUnitOfwork CreateUoW()
        {
            return new UnitOfWork(CloudConfigurationManager.GetSetting("SqlServerConnectionString"));
        }
    }
}
