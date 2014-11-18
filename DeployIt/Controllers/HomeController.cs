using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Mvc;
using DeployIt.Common;
using DeployIt.Models;
using Newtonsoft.Json;

namespace DeployIt.Controllers
{
    public class HomeController : BaseController
    {
        public async Task<ActionResult> Index(int? id)
        {
            var request = new DeployRequest();

            try
            {
                var projects = DocumentSession.Query<ProjectConfig>().OrderBy(c => c.Name);
                ViewBag.ProjectList = projects;

                if (id.HasValue && id != 0)
                {
                    var projectConfig = DocumentSession.Load<ProjectConfig>(id);
                    var buildList = await GetBuildList(projectConfig.TfsProjectName, projectConfig.Branch);
                    ViewBag.LastBuildList = buildList;

                    var webConfig = Path.Combine(projectConfig.DestinationRootLocation,
                        projectConfig.DetinationProjectFolder, "Web.config");
                    //var currentVersion = Helper.ReadVersionNumber(webConfig, projectConfig.VersionKeyName);
                    var currentVersion = "1.0.0";
                    var nextVersion = CalculateNextVersionNumber(currentVersion);

                    request.DestinationRootLocation = projectConfig.DestinationRootLocation;
                    request.DestinationProjectFolder = projectConfig.DetinationProjectFolder;
                    request.VersionKeyName = projectConfig.VersionKeyName;
                    request.CurrentVersion = currentVersion;
                    request.NextVersion = nextVersion;
                    request.ProjectName = projectConfig.Name;
                    request.ProjectId = projectConfig.Id;
                    
                    if (buildList.Any())
                    {
                        var build = buildList.First();
                        request.BuildDropLocation = build.DropLocation;
                        request.PublishedWebsiteFolder = projectConfig.SourceSubFolder;
                    }

                    ViewBag.LastDeploymentList = DocumentSession.Query<DeployRequest>()
                        .Where(d => d.ProjectName == projectConfig.Name)
                        .OrderByDescending(d => d.RequestAt)
                        .Take(5);

                    ViewBag.QueuedBuildList = await GetQueuedBuildList(projectConfig.TfsProjectName);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }

            return View(request);
        }

        private async Task<IEnumerable<BuildInfoModel>> GetBuildList(string tfsProjectName, string branch)
        {
            var url = Url.HttpRouteUrl("DefaultApi",
                new
                {
                    controller = "Tfs",
                    tfsProjectName = tfsProjectName,
                    branch = branch
                });

            return await GetWebApiAsync<BuildInfoModel>(url);
        }

        private async Task<IEnumerable<QueuedBuildInfoModel>> GetQueuedBuildList(string tfsProjectName)
        {
            var url = Url.HttpRouteUrl("DefaultApi",
                new
                {
                    controller = "Tfs",
                    tfsProjectName = tfsProjectName,
                });

            return await GetWebApiAsync<QueuedBuildInfoModel>(url);
        }

        private static string CalculateNextVersionNumber(string versionNumber)
        {
            var numbers = versionNumber.Split('.').Select(int.Parse).ToArray();
            numbers[numbers.Count() - 1] += 1;

            return string.Join(".", numbers);
        }

        private async Task<IEnumerable<T>> GetWebApiAsync<T>(string url)
        {
            return await Task.FromResult(Enumerable.Empty<T>());

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            using(var handler= new HttpClientHandler{UseDefaultCredentials = true })
            using (var httpClient = new HttpClient(handler){BaseAddress = new Uri(baseUrl)})
            {
                var response = await httpClient.GetStringAsync(baseUrl + url);
                return JsonConvert.DeserializeObject<IEnumerable<T>>(response);
            }
        }
    }
}
