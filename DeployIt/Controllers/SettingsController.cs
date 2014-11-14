using System;
using System.Net;
using System.Net.Http;
using System.Web.Mvc;
using AutoMapper;
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
            var config = DocumentSession.Load<ProjectConfig>(id);
            DocumentSession.Delete(config);

            throw new InvalidCastException();

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public HttpResponseMessage Copy(int id)
        {
            Mapper.CreateMap<ProjectConfig, ProjectConfig>()
                .ForMember(s => s.Name, o => o.MapFrom(c => c.Name + " - copy"))
                .ForMember(s => s.Id, o => o.Ignore())
                .ForMember(s => s.LastDeployedAt, o => o.Ignore());

            var config = DocumentSession.Load<ProjectConfig>(id);
            var copy = Mapper.Map<ProjectConfig>(config);

            DocumentSession.Store(copy);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
}