using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectCategory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string IconUrl { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        // Navigation property
        public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
