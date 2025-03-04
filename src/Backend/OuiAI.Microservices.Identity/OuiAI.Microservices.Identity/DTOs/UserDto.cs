using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Identity.DTOs
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public string Bio { get; set; }
        public string Website { get; set; }
        public string Location { get; set; }
        public string TwitterUsername { get; set; }
        public string GitHubUsername { get; set; }
        public DateTime CreatedAt { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int ProjectsCount { get; set; }
        public bool IsFollowedByCurrentUser { get; set; }
    }
}
