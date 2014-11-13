using System;
using System.IO;
using System.Web.Http;
using DeployIt.Common;
using DeployIt.Hubs;
using DeployIt.Models;
using Microsoft.AspNet.SignalR;
using Microsoft.VisualBasic.FileIO;

namespace DeployIt.Controllers
{
    public class DeployController : ApiController
    {
        public void Post(DeployBuildModel model)
        {
            var hub = GlobalHost.ConnectionManager.GetHubContext<MessageHub>();
            hub.Clients.All.addNewMessageToPage("A test message from deploy controller");
        }
    }
}
