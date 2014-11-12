using System;
using System.ComponentModel;
using Microsoft.TeamFoundation.Build.Client;

namespace DeployIt.Models
{
    public class BuildInfoModel
    {
        public string Project { get; set; }
        public string Branch { get; set; }
        public string BuildNumber { get; set; }
        public BuildStatus Status { get; set; }
        public DateTime FinishTime { get; set; }
        public string RequestedBy { get; set; }
        public string Changeset { get; set; }
        public string DropLocation { get; set; }
    }

    public class DeployBuildModel
    {
        [DisplayName("Source location")]
        public string SourceLocation { get; set; }

        [DisplayName("Destination location")]
        public string DestinationLocation { get; set; }

        [DisplayName("Current version")]
        public string CurrentVersion { get; set; }

        [DisplayName("Next version")]
        public string NextVersion { get; set; }

        public DateTime LastDeployedAt { get; set; }
    }
}