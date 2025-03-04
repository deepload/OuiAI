using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.Models
{
    public class Project
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public Guid CategoryId { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public string ThumbnailUrl { get; set; }
        public string RepositoryUrl { get; set; }
        public string DemoUrl { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsFeatured { get; set; }
        
        // Navigation properties
        public virtual ProjectCategory Category { get; set; }
        public virtual ICollection<ProjectTagRelation> TagRelations { get; set; } = new List<ProjectTagRelation>();
        public virtual ICollection<ProjectMedia> Media { get; set; } = new List<ProjectMedia>();
        public virtual ICollection<ProjectLike> Likes { get; set; } = new List<ProjectLike>();
        public virtual ICollection<ProjectComment> Comments { get; set; } = new List<ProjectComment>();
        public virtual ICollection<ProjectView> Views { get; set; } = new List<ProjectView>();
    }

    public enum ProjectVisibility
    {
        Public,
        Private,
        Unlisted
    }
}
