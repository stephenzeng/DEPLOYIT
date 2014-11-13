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

        public ActionResult Test()
        {
            var config = new ProjectConfig
            {
                Name = "DMS",
                TfsProjectName = "DSC.DMS",
                Branch = "DMS-Main",
                VersionKeyName = "Version",
                SourceSubFolder = "",
                DestinationRootLocation = "",
                DetinationProjectFolder = "DMS_Main",
            };

            DocumentSession.Store(config);

            return View();
        }
    }
}