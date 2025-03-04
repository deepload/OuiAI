using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.Models
{
    public class ProjectComment
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Navigation properties
        public virtual Project Project { get; set; }
        public virtual ProjectComment ParentComment { get; set; }
        public virtual ICollection<ProjectComment> Replies { get; set; } = new List<ProjectComment>();
    }
}
