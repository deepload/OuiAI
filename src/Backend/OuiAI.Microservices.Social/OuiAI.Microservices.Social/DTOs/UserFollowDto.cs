using System;

namespace OuiAI.Microservices.Social.DTOs
{
    public class UserFollowDto
    {
        public Guid FollowerId { get; set; }
        public string FollowerUsername { get; set; }
        public string FollowerDisplayName { get; set; }
        public string FollowerProfileImageUrl { get; set; }
        public Guid FolloweeId { get; set; }
        public string FolloweeUsername { get; set; }
        public string FolloweeDisplayName { get; set; }
        public string FolloweeProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class FollowerDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime FollowedAt { get; set; }
    }
    
    public class FolloweeDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime FollowedAt { get; set; }
    }
    
    public class FollowRequestDto
    {
        public Guid FolloweeId { get; set; }
    }
}
