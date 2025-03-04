using System;

namespace OuiAI.Microservices.Social.Models
{
    public class UserFollowModel
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
}
