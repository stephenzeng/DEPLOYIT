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

                    model.DestinationRootLocation = projectConfig.DestinationRootLocation;
                    model.DestinationProjectFolder = projectConfig.DetinationProjectFolder;
                    model.VersionKeyName = projectConfig.VersionKeyName;
                    model.CurrentVersion = currentVersion;
                    model.NextVersion = nextVersion;
                    model.LastDeployedAt = projectConfig.LastDeployedAt;
                    
                    if (buildList.Any())
                    {
                        var build = buildList.First();
                        model.BuildDropLocation = build.DropLocation;
                        model.PublishedWebsiteFolder = projectConfig.SourceSubFolder;
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
            //Backup project folder
            var backupFolder = Path.Combine(model.DestinationRootLocation, "_Backup",
                string.Format("{0}_{1}", model.DestinationProjectFolder, DateTime.Now.ToString("yyyyMMdd_hhmmss")));
            var projectPath = Path.Combine(model.DestinationRootLocation, model.DestinationProjectFolder);
            FileSystem.CopyDirectory(projectPath, backupFolder);

            //Copy files to project folder
            var source = Path.Combine(model.BuildDropLocation, model.PublishedWebsiteFolder);
            FileSystem.CopyDirectory(source, projectPath, true);

            //copy web.config
            var sourceConfig = Path.Combine(backupFolder, "Web.config");
            var destConfig = Path.Combine(projectPath, "Web.config");
            FileSystem.CopyFile(sourceConfig, destConfig, true);

            //update version number
            Helper.SetVersionNumber(destConfig, model.VersionKeyName, model.NextVersion);

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
