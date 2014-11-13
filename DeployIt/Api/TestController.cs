using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using DeployIt.Hubs;
using Microsoft.AspNet.SignalR;

namespace DeployIt.Api
{
    public class TestController : ApiController
    {
        readonly Lazy<IHubContext> _hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<MessageHub>()
            );

        protected IHubContext Hub {get { return _hub.Value; }}

        public HttpResponseMessage GetServerTime()
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();

            hub.Clients.All.send(DateTime.Now);

            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}