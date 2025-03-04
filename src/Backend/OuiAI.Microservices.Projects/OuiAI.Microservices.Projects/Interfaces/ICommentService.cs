using OuiAI.Microservices.Projects.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Interfaces
{
    public interface ICommentService
    {
        Task<IEnumerable<CommentDto>> GetCommentsByProjectIdAsync(Guid projectId);
        Task<CommentDto> GetCommentByIdAsync(Guid id);
        Task<CommentDto> CreateCommentAsync(CreateCommentDto commentDto, Guid userId, string userName, string userProfileImageUrl);
        Task<CommentDto> UpdateCommentAsync(Guid id, UpdateCommentDto commentDto, Guid userId);
        Task DeleteCommentAsync(Guid id, Guid userId);
        Task<IEnumerable<CommentDto>> GetRepliesByCommentIdAsync(Guid commentId);
    }
}
