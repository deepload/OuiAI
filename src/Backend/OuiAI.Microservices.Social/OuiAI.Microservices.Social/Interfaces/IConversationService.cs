using OuiAI.Microservices.Social.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Interfaces
{
    public interface IConversationService
    {
        Task<ConversationDto> CreateConversationAsync(Guid creatorId, CreateConversationDto conversationDto);
        Task<ConversationDetailDto> GetConversationByIdAsync(Guid conversationId, Guid userId);
        Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<ParticipantDto> AddParticipantAsync(Guid currentUserId, AddParticipantDto addParticipantDto);
        Task<bool> RemoveParticipantAsync(Guid currentUserId, RemoveParticipantDto removeParticipantDto);
        Task<bool> MarkConversationAsReadAsync(Guid userId, MarkConversationReadDto markReadDto);
        Task<int> GetUnreadConversationsCountAsync(Guid userId);
    }
}
