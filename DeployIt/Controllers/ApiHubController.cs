using System;
using System.Web.Http;
using Microsoft.AspNet.SignalR;

namespace DeployIt.Controllers
{
    public abstract class ApiHubController<T> : ApiController where T : Hub
    {
        private readonly Lazy<IHubContext> _lazy = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<T>());

        public IHubContext HubContext
        {
            get { return _lazy.Value; }
        }
    }
}