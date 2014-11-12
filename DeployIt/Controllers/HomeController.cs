using System;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Xml;
using DeployIt.Common;
using DeployIt.Models;
using Microsoft.VisualBasic.FileIO;

namespace DeployIt.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index(int? id)
        {
            var model = new DeployBuildModel();

            try
            {
                var projects = DocumentSession.Query<ProjectConfig>().OrderBy(c => c.Name);

                ViewBag.ProjectList = projects;

                if (id.HasValue && id != 0)
                {
                    var projectConfig = DocumentSession.Load<ProjectConfig>(id);
                    var buildList = Helper.GetLastBuildList(projectConfig.TfsProjectName, projectConfig.Branch);

                    var webConfig = Path.Combine(projectConfig.DestinationRootLocation,
                        projectConfig.DetinationProjectFolder, "Web.config");
                    var currentVersion = Helper.ReadVersionNumber(webConfig, projectConfig.VersionKeyName);
                    var nextVersion = CalculateNextVersionNumber(currentVersion);

                    model.DestinationLocation = Path.Combine(projectConfig.DestinationRootLocation,
                        projectConfig.DetinationProjectFolder);
                    model.CurrentVersion = currentVersion;
                    model.NextVersion = nextVersion;
                    model.LastDeployedAt = projectConfig.LastDeployedAt;

                    if (buildList.Any())
                    {
                        model.SourceLocation = buildList.First().DropLocation;
                    }

                    ViewBag.LastBuildList = buildList;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Deploy(DeployBuildModel model)
        {
            ////Backup folder
            //var backupFolder = string.Format("{0}_{1}", model.DestinationLocation, DateTime.Now.ToString("ddMMyyyy"));
            //FileSystem.CopyDirectory(model.DestinationLocation, backupFolder);

            ////Copy files to dest
            //var source = Path.Combine(model.SourceLocation, _sourceSubFolder);
            //FileSystem.CopyDirectory(source, model.DestinationLocation, true);

            ////copy web.config
            //FileSystem.CopyFile(Path.Combine(backupFolder, "Web.config"), Path.Combine(model.DestinationLocation, "Web.config"), true);

            ////update version number
            //var configFile = Path.Combine(model.DestinationLocation, "Web.config");
            //SetVersionNumber(configFile, model.NextVersion);

            return View("Index");
        }

        private static string CalculateNextVersionNumber(string versionNumber)
        {
            var numbers = versionNumber.Split('.').Select(int.Parse).ToArray();
            numbers[numbers.Count() - 1] += 1;

            return string.Join(".", numbers);
        }
    }
}
