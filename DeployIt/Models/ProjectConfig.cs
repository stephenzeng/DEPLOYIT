using System;

namespace DeployIt.Models
{
    public class ProjectConfig
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string TfsProjectName { get; set; }
        public string Branch { get; set; }
        public string VersionKeyName { get; set; }
        public string SourceSubFolder { get; set; }
        public string DestinationRootLocation { get; set; }
        public string DetinationProjectFolder { get; set; }
        public DateTime LastDeployedAt { get; set; }
    }
}