using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using DeployIt.Models;
using Microsoft.TeamFoundation.Build.Client;
using Microsoft.TeamFoundation.Client;

namespace DeployIt.Common
{
    public static class Helper
    {
        static readonly Uri TfsUri = new Uri("http://wptfs08:8080/");

        public static IEnumerable<BuildInfoModel> GetLastBuildList(string tfsProjectName, string branch)
        {
            var tfs = new TfsTeamProjectCollection(TfsUri);

            var buildServer = tfs.GetService<IBuildServer>();

            var lastBuildList = buildServer.QueryBuilds(tfsProjectName)
                .Where(b => b.BuildDefinition.Name == branch)
                .OrderByDescending(b => b.FinishTime)
                .Take(10)
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

        public static string ReadVersionNumber(string configFile, string versionKeyName)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);

            var node = xmlDoc.SelectSingleNode(string.Format("//add[@key='{0}']", versionKeyName));
            return node != null ? node.Attributes["value"].Value : string.Empty;
        }

        public static void SetVersionNumber(string configFile, string versionKeyName, string value)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(configFile);

            var node = xmlDoc.SelectSingleNode(string.Format("//add[@key='{0}']", versionKeyName));
            if (node != null) node.Attributes["value"].Value = value;

            xmlDoc.Save(configFile);
        }
    }
}