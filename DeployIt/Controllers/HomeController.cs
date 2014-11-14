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
            var request = new DeployRequest();

            try
            {
                var projects = DocumentSession.Query<ProjectConfig>().OrderBy(c => c.Name);

                ViewBag.ProjectList = projects;

                if (id.HasValue && id != 0)
                {
                    var projectConfig = DocumentSession.Load<ProjectConfig>(id);
                    var buildList = Helper.GetLastBuildList(projectConfig.TfsProjectName, projectConfig.Branch, 5);

                    var webConfig = Path.Combine(projectConfig.DestinationRootLocation,
                        projectConfig.DetinationProjectFolder, "Web.config");
                    var currentVersion = Helper.ReadVersionNumber(webConfig, projectConfig.VersionKeyName);
                    var nextVersion = CalculateNextVersionNumber(currentVersion);

                    request.DestinationRootLocation = projectConfig.DestinationRootLocation;
                    request.DestinationProjectFolder = projectConfig.DetinationProjectFolder;
                    request.VersionKeyName = projectConfig.VersionKeyName;
                    request.CurrentVersion = currentVersion;
                    request.NextVersion = nextVersion;
                    request.ProjectName = projectConfig.Name;
                    
                    if (buildList.Any())
                    {
                        var build = buildList.First();
                        request.BuildDropLocation = build.DropLocation;
                        request.PublishedWebsiteFolder = projectConfig.SourceSubFolder;
                    }

                    ViewBag.LastBuildList = buildList;

                    ViewBag.LastDeploymentList = DocumentSession.Query<DeployRequest>()
                        .OrderByDescending(d => d.RequestAt)
                        .Take(5);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

            return View(request);
        }

        private static string CalculateNextVersionNumber(string versionNumber)
        {
            var numbers = versionNumber.Split('.').Select(int.Parse).ToArray();
            numbers[numbers.Count() - 1] += 1;

            return string.Join(".", numbers);
        }
    }
}
