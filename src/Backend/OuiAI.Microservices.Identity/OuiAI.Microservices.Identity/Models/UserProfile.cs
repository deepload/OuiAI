using System;

namespace OuiAI.Microservices.Identity.Models
{
    public class UserProfile
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; }
        public string Bio { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string TwitterUsername { get; set; }
        public string GitHubUsername { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation property
        public virtual ApplicationUser User { get; set; }
    }
}
