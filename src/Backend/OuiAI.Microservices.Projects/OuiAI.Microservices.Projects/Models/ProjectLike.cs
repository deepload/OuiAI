using System;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectLike
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual Project Project { get; set; }
    }
}
