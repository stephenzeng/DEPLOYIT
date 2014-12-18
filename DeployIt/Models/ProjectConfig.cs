using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DeployIt.Models
{
    public class ProjectConfig
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DisplayName("TFS Project Name")]
        public string TfsProjectName { get; set; }

        [Required]
        public string Branch { get; set; }

        [Required]
        [DisplayName("Version Key Name")]
        public string VersionKeyName { get; set; }

        [Required]
        [DisplayName("Source Subfolder")]
        public string SourceSubFolder { get; set; }

        [Required]
        [DisplayName("Destination Root Location")]
        public string DestinationRootLocation { get; set; }

        [Required]
        [DisplayName("Destination Project Location")]
        public string DetinationProjectFolder { get; set; }

        public DateTime CreateAt { get; set; }
    }
}