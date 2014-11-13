using System;

namespace DeployIt.Models
{
    public class DeploymentLog
    {
        public int Id { get; set; }
        public DateTime LogTime { get; set; }
        public string Description { get; set; }
        public string User { get; set; }
    }
}