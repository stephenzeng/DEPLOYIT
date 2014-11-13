using System;
using System.Net;
using System.Net.Http;
using DeployIt.Hubs;
using DeployIt.Models;

namespace DeployIt.Controllers
{
    public class DeployController : ApiHubController<NotificationHub>
    {
        public HttpResponseMessage Get()
        {
            HubContext.Clients.All.broadcast(string.Format("Get: The server time is {0}", DateTime.Now));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        public HttpResponseMessage Post(DeployBuildModel model)
        {
            HubContext.Clients.All.broadcast(string.Format("Post: The server time is {0}", DateTime.Now));

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}
