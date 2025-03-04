using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Social.Models
{
    public class MessageModel
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderDisplayName { get; set; }
        public string SenderProfileImageUrl { get; set; }
        public string Content { get; set; }
        public ICollection<string> AttachmentUrls { get; set; } = new List<string>();
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual ConversationModel Conversation { get; set; }
    }
}
