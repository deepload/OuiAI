using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Social.DTOs
{
    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public MessageDto LastMessage { get; set; }
        public int UnreadCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
    
    public class ConversationDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<ParticipantDto> Participants { get; set; }
        public List<MessageDto> Messages { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
    }
    
    public class ParticipantDto
    {
        public Guid UserId { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime JoinedAt { get; set; }
        public DateTime? LastReadAt { get; set; }
        public bool IsOnline { get; set; }
    }
    
    public class CreateConversationDto
    {
        public string Title { get; set; }
        public List<Guid> ParticipantIds { get; set; }
        public string InitialMessage { get; set; }
    }
    
    public class AddParticipantDto
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
    }
    
    public class RemoveParticipantDto
    {
        public Guid ConversationId { get; set; }
        public Guid UserId { get; set; }
    }
    
    public class MarkConversationReadDto
    {
        public Guid ConversationId { get; set; }
        public DateTime ReadTimestamp { get; set; } = DateTime.UtcNow;
    }
}
