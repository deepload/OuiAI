using System.ComponentModel.DataAnnotations;

namespace OuiAI.Microservices.Identity.DTOs
{
    public class LoginDto
    {
        [Required]
        public string UsernameOrEmail { get; set; }

        [Required]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
