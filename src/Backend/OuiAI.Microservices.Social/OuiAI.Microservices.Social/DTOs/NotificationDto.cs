using OuiAI.Microservices.Social.Models;
using System;

namespace OuiAI.Microservices.Social.DTOs
{
    public class NotificationDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string ActionUrl { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? OriginatorId { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorProfileImageUrl { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    public class CreateNotificationDto
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public string ImageUrl { get; set; }
        public string ActionUrl { get; set; }
        public Guid? RelatedEntityId { get; set; }
        public Guid? OriginatorId { get; set; }
        public string OriginatorName { get; set; }
        public string OriginatorProfileImageUrl { get; set; }
    }
    
    public class NotificationMarkReadDto
    {
        public Guid NotificationId { get; set; }
    }
    
    public class NotificationMarkAllReadDto
    {
        public Guid UserId { get; set; }
    }
}
