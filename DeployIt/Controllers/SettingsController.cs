using System.Web.Mvc;
using DeployIt.Models;

namespace DeployIt.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            var list = DocumentSession.Query<ProjectConfig>();

            return View(list);
        }
    }
}