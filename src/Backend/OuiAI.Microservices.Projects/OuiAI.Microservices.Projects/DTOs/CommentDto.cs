using System;
using System.Collections.Generic;

namespace OuiAI.Microservices.Projects.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string UserProfileImageUrl { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<CommentDto> Replies { get; set; }
    }

    public class CreateCommentDto
    {
        public Guid ProjectId { get; set; }
        public string Content { get; set; }
        public Guid? ParentCommentId { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Content { get; set; }
    }
}
