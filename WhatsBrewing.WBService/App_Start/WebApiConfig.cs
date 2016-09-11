using System;
using WebApi.OutputCache.V2;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApi.OutputCache.Core.Cache;

namespace WhatsBrewing.WBService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // New code
            config.EnableCors();


            // Web API configuration and services
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            // Web API routes
            config.MapHttpAttributeRoutes();

            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                //routeTemplate: "api/{controller}/{id}",
                routeTemplate: "api/{controller}/{action}"
                //defaults: new { id = RouteParameter.Optional }
            );

            //var cacheConfig = config.CacheOutputConfiguration();
            //cacheConfig.RegisterCacheOutputProvider(() => new MemoryCacheDefault());
        }
    }
}
