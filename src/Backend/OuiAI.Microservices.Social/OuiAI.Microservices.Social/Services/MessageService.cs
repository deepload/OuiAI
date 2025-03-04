using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OuiAI.Microservices.Social.Data;
using OuiAI.Microservices.Social.DTOs;
using OuiAI.Microservices.Social.Hubs;
using OuiAI.Microservices.Social.Interfaces;
using OuiAI.Microservices.Social.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Services
{
    public class MessageService : IMessageService
    {
        private readonly SocialDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ConversationHub> _conversationHub;
        private readonly ILogger<MessageService> _logger;

        public MessageService(
            SocialDbContext context,
            IMapper mapper,
            IHubContext<ConversationHub> conversationHub,
            ILogger<MessageService> logger)
        {
            _context = context;
            _mapper = mapper;
            _conversationHub = conversationHub;
            _logger = logger;
        }

        public async Task<MessageDto> CreateMessageAsync(Guid senderId, CreateMessageDto messageDto)
        {
            // Verify the sender is a participant in the conversation
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == messageDto.ConversationId && cp.UserId == senderId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            // Get sender information
            var sender = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == messageDto.ConversationId && cp.UserId == senderId);

            // Create the message
            var message = new MessageModel
            {
                Id = Guid.NewGuid(),
                ConversationId = messageDto.ConversationId,
                SenderId = senderId,
                SenderUsername = sender?.Username,
                SenderDisplayName = sender?.DisplayName,
                SenderProfileImageUrl = sender?.ProfileImageUrl,
                Content = messageDto.Content,
                AttachmentUrls = messageDto.AttachmentUrls,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);

            // Update the conversation's last message timestamp
            var conversation = await _context.Conversations.FindAsync(messageDto.ConversationId);
            if (conversation != null)
            {
                conversation.LastMessageAt = message.CreatedAt;
                conversation.UpdatedAt = message.CreatedAt;
            }

            await _context.SaveChangesAsync();

            // Mark as read for the sender
            var senderParticipant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == messageDto.ConversationId && cp.UserId == senderId);

            if (senderParticipant != null)
            {
                senderParticipant.LastReadAt = message.CreatedAt;
                await _context.SaveChangesAsync();
            }

            var messageDto = _mapper.Map<MessageDto>(message);

            // Send real-time message notification via SignalR
            try
            {
                await _conversationHub.Clients.Group($"Conversation_{message.ConversationId}")
                    .SendAsync("ReceiveMessage", messageDto);

                // Also send notifications to all participants who might not be in the conversation group
                var participantIds = await _context.ConversationParticipants
                    .Where(cp => cp.ConversationId == message.ConversationId && cp.UserId != senderId)
                    .Select(cp => cp.UserId)
                    .ToListAsync();

                foreach (var participantId in participantIds)
                {
                    await _conversationHub.Clients.Group($"User_{participantId}")
                        .SendAsync("NewMessage", message.ConversationId, messageDto);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message via SignalR");
            }

            return messageDto;
        }

        public async Task<MessageDto> GetMessageByIdAsync(Guid messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            return _mapper.Map<MessageDto>(message);
        }

        public async Task<IEnumerable<MessageDto>> GetConversationMessagesAsync(Guid conversationId, int page = 1, int pageSize = 50)
        {
            var messages = await _context.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<MessageDto>>(messages.OrderBy(m => m.CreatedAt));
        }

        public async Task<bool> MarkMessageAsReadAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return false;
            }

            // Verify the user is a participant in the conversation
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == message.ConversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            // If it's the sender's message, it's already read
            if (message.SenderId == userId)
            {
                return true;
            }

            // Update the participant's last read timestamp
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == message.ConversationId && cp.UserId == userId);

            if (participant != null && (participant.LastReadAt == null || participant.LastReadAt < message.CreatedAt))
            {
                participant.LastReadAt = message.CreatedAt;
                await _context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
            {
                return false;
            }

            // Only the sender can delete their messages
            if (message.SenderId != userId)
            {
                throw new UnauthorizedAccessException("You can only delete your own messages");
            }

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();

            // Notify all participants about the deleted message
            try
            {
                await _conversationHub.Clients.Group($"Conversation_{message.ConversationId}")
                    .SendAsync("MessageDeleted", messageId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message deleted notification via SignalR");
            }

            return true;
        }
    }
}
