using System;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectMedia
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public ProjectMediaType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual Project Project { get; set; }
    }

    public enum ProjectMediaType
    {
        Image,
        Video,
        Document,
        Code
    }
}
