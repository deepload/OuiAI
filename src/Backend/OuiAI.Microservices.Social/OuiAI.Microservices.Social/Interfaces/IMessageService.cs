using OuiAI.Microservices.Social.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Interfaces
{
    public interface IMessageService
    {
        Task<MessageDto> CreateMessageAsync(Guid senderId, CreateMessageDto messageDto);
        Task<MessageDto> GetMessageByIdAsync(Guid messageId);
        Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50);
        Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId);
        Task<bool> DeleteMessageAsync(Guid messageId, Guid userId);
    }
}
