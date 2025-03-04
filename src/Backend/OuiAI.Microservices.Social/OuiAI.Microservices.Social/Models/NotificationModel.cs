using System;

namespace OuiAI.Microservices.Social.Models
{
    public class NotificationModel
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

    public enum NotificationType
    {
        NewFollower,
        ProjectLike,
        ProjectComment,
        CommentReply,
        ProjectFeature,
        SystemAnnouncement
    }
}
