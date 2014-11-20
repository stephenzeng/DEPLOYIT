using System;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.AspNet.SignalR;
using Raven.Client;

namespace DeployIt.Controllers
{
    public abstract class ApiHubController<T> : ApiController where T : Hub
    {
        private readonly Lazy<IHubContext> _lazy = new Lazy<IHubContext>(() => GlobalHost.ConnectionManager.GetHubContext<T>());

        protected IDocumentSession DocumentSession { get; set; }

        protected IHubContext HubContext
        {
            get { return _lazy.Value; }
        }
        
        protected void Broadcast(string name, string message)
        {
            HubContext.Clients.All.broadcast(name, message);
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            if (DocumentSession == null) DocumentSession = MvcApplication.DocumentStore.OpenSession();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            using (DocumentSession)
            {
                if (DocumentSession != null)
                    DocumentSession.SaveChanges();
            }
        }
    }
}