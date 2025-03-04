using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectTag
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public virtual ICollection<ProjectTagRelation> ProjectRelations { get; set; } = new List<ProjectTagRelation>();
    }

    public class ProjectTagRelation
    {
        public Guid ProjectId { get; set; }
        public Guid TagId { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual ProjectTag Tag { get; set; }
    }
}
