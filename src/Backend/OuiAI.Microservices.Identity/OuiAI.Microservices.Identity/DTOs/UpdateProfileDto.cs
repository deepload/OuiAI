using System.ComponentModel.DataAnnotations;

namespace OuiAI.Microservices.Identity.DTOs
{
    public class UpdateProfileDto
    {
        [StringLength(100)]
        public string DisplayName { get; set; }

        [StringLength(500)]
        public string Bio { get; set; }

        public string ProfileImageUrl { get; set; }

        [Url]
        public string Website { get; set; }

        [StringLength(100)]
        public string Location { get; set; }

        [StringLength(50)]
        public string TwitterUsername { get; set; }

        [StringLength(50)]
        public string GitHubUsername { get; set; }
    }
}
