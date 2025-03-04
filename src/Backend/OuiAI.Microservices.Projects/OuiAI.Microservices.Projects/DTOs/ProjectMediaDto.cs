using OuiAI.Microservices.Projects.Models;
using System;

namespace OuiAI.Microservices.Projects.DTOs
{
    public class ProjectMediaDto
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
    }

    public class CreateMediaDto
    {
        public Guid ProjectId { get; set; }
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public ProjectMediaType Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }

    public class UpdateMediaDto
    {
        public string Url { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
