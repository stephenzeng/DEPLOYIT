using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DeployIt.Hubs
{
    [HubName("message")]
    public class MessageHub : Hub
    {
        public void Send(string message)
        {
            Clients.All.getMessage(message);
        }
    }
}