using System;
using System.Web.Http;
using DeployIt.Hubs;
using DeployIt.Models;
using Microsoft.AspNet.SignalR;

namespace DeployIt.Api
{
    public class DeployController : ApiController
    {
        public void Post(DeployBuildModel model)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            hub.Clients.All.addNewMessageToPage("A test message from deploy controller");
        }

        public DateTime GetServerTime()
        {
            return DateTime.Now;
        }
    }
}
