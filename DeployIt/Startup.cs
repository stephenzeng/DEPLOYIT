using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(DeployIt.Startup))]

namespace DeployIt
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}