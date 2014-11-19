using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using DeployIt.Hubs;
using DeployIt.Models;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

namespace DeployIt.Controllers
{
    public class TfsController : ApiHubController<NotificationHub>
    {
        private readonly IBuildServer _buildServer;

        public TfsController()
        {
            var tfsUri = new Uri(ConfigurationManager.AppSettings["TfsUrl"]);
            var tfs = new TfsTeamProjectCollection(tfsUri);
            _buildServer = tfs.GetService<IBuildServer>();
        }

        public IEnumerable<BuildInfoModel> GetBuildList(string tfsProjectName, string branch, int? numberOfResuslts = 5)
        {
            var lastBuildList = _buildServer.QueryBuilds(tfsProjectName)
                .Where(b => b.BuildDefinition.Name == branch)
                .OrderByDescending(b => b.FinishTime)
                .Take(numberOfResuslts.Value)
                .Select(b => new BuildInfoModel()
                {
                    Project = b.TeamProject,
                    Branch = b.BuildDefinition.Name,
                    BuildNumber = b.BuildNumber,
                    Status = b.Status,
                    FinishTime = b.FinishTime,
                    RequestedBy = b.RequestedFor,
                    Changeset = b.SourceGetVersion,
                    DropLocation = b.DropLocation,
                });

            return lastBuildList;
        }

        public IEnumerable<QueuedBuildInfoModel> GetQueuedBuildList(string tfsProjectName)
        {
            var spec = _buildServer.CreateBuildQueueSpec(tfsProjectName);
            var list = _buildServer.QueryQueuedBuilds(spec)
                .QueuedBuilds
                .OrderBy(b => b.QueuePosition)
                .Select(b => new QueuedBuildInfoModel
                {
                    TeamProject = b.TeamProject,
                    BuildNumber = b.Build.BuildNumber,
                    RequestedFor = b.Build.RequestedFor,
                    Status = b.Status,
                    QueueTime = b.QueueTime,

                });

            return list;
        }
    }
}