using OuiAI.Microservices.Social.Models;
using System;

namespace OuiAI.Microservices.Social.DTOs
{
    public class ActivityDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string UserDisplayName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public ActivityType Type { get; set; }
        public string Message { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string RelatedEntityTitle { get; set; }
        public string RelatedEntityUrl { get; set; }
        public string RelatedEntityThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class CreateActivityDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string UserDisplayName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public ActivityType Type { get; set; }
        public string Message { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public string RelatedEntityTitle { get; set; }
        public string RelatedEntityUrl { get; set; }
        public string RelatedEntityThumbnailUrl { get; set; }
    }
}
