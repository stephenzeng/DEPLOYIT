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

    public class DeployRequest
    {
        public int Id { get; set; }

        [DisplayName("Last build drop location")]
        public string BuildDropLocation { get; set; }

        [DisplayName("Website build drop folder")]
        public string PublishedWebsiteFolder { get; set; }

        [DisplayName("Destination location")]
        public string DestinationRootLocation { get; set; }

        [DisplayName("Destination project folder")]
        public string DestinationProjectFolder { get; set; }

        [DisplayName("Current version")]
        public string CurrentVersion { get; set; }

        [DisplayName("Next version")]
        public string NextVersion { get; set; }

        public string VersionKeyName { get; set; }
        public int ProjectId { get; set; }
        public DateTime RequestAt { get; set; }
        public bool DeploySuccess { get; set; }
        public string ProjectName { get; set; }
    }
}