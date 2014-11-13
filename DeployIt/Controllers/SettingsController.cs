using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using DeployIt.Models;
using Raven.Client.Linq;

namespace DeployIt.Controllers
{
    public class SettingsController : BaseController
    {
        public ActionResult Index()
        {
            var list = DocumentSession.Query<ProjectConfig>()
                .OrderByDescending(c => c.Id);

            return View(list);
        }

        [HttpGet]
        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(ProjectConfig config)
        {
            try
            {
                DocumentSession.Store(config);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
                return View();
            }
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var config = DocumentSession.Load<ProjectConfig>(id);
            return View(config);
        }

        [HttpPost]
        public ActionResult Edit(ProjectConfig config)
        {
            try
            {
                DocumentSession.Store(config);
                ShowInfoMessage("Settings saved successfully");
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

            return View(config);
        }

        [HttpPost]
        public HttpResponseMessage Delete(int id)
        {
            try
            {
                var config = DocumentSession.Load<ProjectConfig>(id);
                DocumentSession.Delete(config);

                return new HttpResponseMessage(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                var response = new HttpResponseMessage(HttpStatusCode.ExpectationFailed)
                {
                    Content = new StringContent(ex.Message)
                };
                return response;
            }
        }
    }
}