using System.Web.Mvc;

namespace DeployIt.Controllers
{
    public class TestController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}