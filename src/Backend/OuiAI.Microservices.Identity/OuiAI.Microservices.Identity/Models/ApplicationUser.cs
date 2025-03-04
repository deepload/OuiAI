using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace OuiAI.Microservices.Identity.Models
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; } = true;
        
        // Navigation property for user profile
        public virtual UserProfile Profile { get; set; }
        
        // Navigation properties for followers/following
        public virtual ICollection<UserFollow> Followers { get; set; } = new List<UserFollow>();
        public virtual ICollection<UserFollow> Following { get; set; } = new List<UserFollow>();
    }
}
