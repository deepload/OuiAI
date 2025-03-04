using System;

namespace OuiAI.Microservices.Identity.Models
{
    public class UserFollow
    {
        public Guid FollowerId { get; set; }  // The user who is following
        public Guid FolloweeId { get; set; }  // The user being followed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties
        public virtual ApplicationUser Follower { get; set; }
        public virtual ApplicationUser Followee { get; set; }
    }
}
