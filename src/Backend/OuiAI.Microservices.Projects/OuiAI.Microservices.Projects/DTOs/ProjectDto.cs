using OuiAI.Microservices.Projects.Models;
using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.DTOs
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public CategoryDto Category { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public string ThumbnailUrl { get; set; }
        public string RepositoryUrl { get; set; }
        public string DemoUrl { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public List<TagDto> Tags { get; set; }
        public List<ProjectMediaDto> Media { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    public class ProjectSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string CategoryName { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ViewsCount { get; set; }
        public int LikesCount { get; set; }
        public List<string> TagNames { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsFeatured { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    public class CreateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Guid CategoryId { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public string ThumbnailUrl { get; set; }
        public string RepositoryUrl { get; set; }
        public string DemoUrl { get; set; }
        public List<Guid> TagIds { get; set; }
    }

    public class UpdateProjectDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public Guid CategoryId { get; set; }
        public ProjectVisibility Visibility { get; set; }
        public string ThumbnailUrl { get; set; }
        public string RepositoryUrl { get; set; }
        public string DemoUrl { get; set; }
        public List<Guid> TagIds { get; set; }
    }
}
