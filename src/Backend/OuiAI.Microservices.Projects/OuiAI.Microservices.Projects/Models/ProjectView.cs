using System;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectView
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public DateTime ViewedAt { get; set; }
        
        // Navigation property
        public virtual Project Project { get; set; }
    }
}
