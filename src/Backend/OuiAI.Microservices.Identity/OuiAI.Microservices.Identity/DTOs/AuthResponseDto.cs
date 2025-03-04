using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Identity.DTOs
{
    public class AuthResponseDto
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserDto User { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }
}
