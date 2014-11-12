using System.Web.Mvc;
using Raven.Client;

namespace DeployIt.Controllers
{
    public abstract class BaseController : Controller
    {
        public static IDocumentStore DocumentStore { get; set; }

        public IDocumentSession DocumentSession { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            DocumentSession = (IDocumentSession)HttpContext.Items["CurrentRequestRavenSession"];
        }

        protected HttpStatusCodeResult HttpNotModified()
        {
            return new HttpStatusCodeResult(304);
        }

        protected void ShowInfoMessage(string message)
        {
            ViewBag.InfoMessage = message;
        }

        protected void ShowWarningMessage(string message)
        {
            ViewBag.WarningMessage = message;
        }

        protected void ShowErrorMessage(string message)
        {
            ViewBag.ErrorMessage = message;
        }
    }
}