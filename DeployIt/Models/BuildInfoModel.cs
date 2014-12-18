using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

    public class QueuedBuildInfoModel
    {
        public string TeamProject { get; set; }
        public string BuildNumber { get; set; }
        public string RequestedFor { get; set; }
        public QueueStatus Status { get; set; }
        public DateTime QueueTime { get; set; }
    }

    public class DeployRequest
    {
        public int Id { get; set; }

        [DisplayName("Last build drop location")]
        [Required]
        public string BuildDropLocation { get; set; }

        [DisplayName("Website build drop folder")]
        [Required]
        public string PublishedWebsiteFolder { get; set; }

        [DisplayName("Destination location")]
        [Required]
        public string DestinationRootLocation { get; set; }

        [DisplayName("Destination project folder")]
        [Required]
        public string DestinationProjectFolder { get; set; }

        [DisplayName("Current version")]
        public string CurrentVersion { get; set; }

        [DisplayName("Next version")]
        [Required]
        public string NextVersion { get; set; }

        public string VersionKeyName { get; set; }
        public int ProjectId { get; set; }
        public DateTime RequestAt { get; set; }
        public bool DeploySuccess { get; set; }
        public string ProjectName { get; set; }
    }
}