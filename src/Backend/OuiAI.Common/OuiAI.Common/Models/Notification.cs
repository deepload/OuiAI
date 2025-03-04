using System;

namespace OuiAI.Common.Models
{
    public class Notification
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public string RelatedEntityId { get; set; }
        public string RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
    }

    public enum NotificationType
    {
        NewComment,
        NewLike,
        NewFollower,
        ProjectFeatured,
        MentionInComment,
        SystemAnnouncement
    }
}
