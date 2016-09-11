
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(WhatsBrewing.WBService.SignalR.Startup))]

namespace WhatsBrewing.WBService.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new IdProvider());

            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}