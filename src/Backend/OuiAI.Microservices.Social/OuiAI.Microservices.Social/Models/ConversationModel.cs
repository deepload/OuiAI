using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Social.Models
{
    public class ConversationModel
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        
        // Navigation properties
        public virtual ICollection<MessageModel> Messages { get; set; } = new List<MessageModel>();
        public virtual ICollection<ConversationParticipantModel> Participants { get; set; } = new List<ConversationParticipantModel>();
    }
    
    public class ConversationParticipantModel
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        
        // Navigation property
        public virtual ConversationModel Conversation { get; set; }
    }
}
