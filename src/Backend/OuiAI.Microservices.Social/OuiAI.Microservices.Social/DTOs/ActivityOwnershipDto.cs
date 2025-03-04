using System;

namespace OuiAI.Microservices.Social.DTOs
{
    public class ActivityOwnershipDto
    {
        public bool Exists { get; set; }
        public bool IsOwner { get; set; }
        public Guid? ActivityId { get; set; }
        public Guid? OwnerId { get; set; }
    }
}
