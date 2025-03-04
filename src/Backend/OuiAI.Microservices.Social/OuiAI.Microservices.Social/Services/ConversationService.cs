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
    public class ConversationService : IConversationService
    {
        private readonly SocialDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<ConversationHub> _conversationHub;
        private readonly IMessageService _messageService;
        private readonly ILogger<ConversationService> _logger;

        public ConversationService(
            SocialDbContext context,
            IMapper mapper,
            IHubContext<ConversationHub> conversationHub,
            IMessageService messageService,
            ILogger<ConversationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _conversationHub = conversationHub;
            _messageService = messageService;
            _logger = logger;
        }

        public async Task<ConversationDto> CreateConversationAsync(Guid creatorId, CreateConversationDto conversationDto)
        {
            // Check if all participants exist
            if (!conversationDto.ParticipantIds.Contains(creatorId))
            {
                // Always include the creator as a participant
                conversationDto.ParticipantIds.Add(creatorId);
            }

            // Create the conversation
            var conversation = new ConversationModel
            {
                Id = Guid.NewGuid(),
                Title = conversationDto.Title,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add participants
            foreach (var participantId in conversationDto.ParticipantIds)
            {
                conversation.Participants.Add(new ConversationParticipantModel
                {
                    ConversationId = conversation.Id,
                    UserId = participantId,
                    JoinedAt = DateTime.UtcNow
                });
            }

            _context.Conversations.Add(conversation);
            await _context.SaveChangesAsync();

            // If there's an initial message, create it
            if (!string.IsNullOrEmpty(conversationDto.InitialMessage))
            {
                var messageDto = new CreateMessageDto
                {
                    ConversationId = conversation.Id,
                    Content = conversationDto.InitialMessage
                };

                await _messageService.CreateMessageAsync(creatorId, messageDto);
            }

            // Notify all participants about the new conversation
            try
            {
                foreach (var participantId in conversationDto.ParticipantIds)
                {
                    await _conversationHub.Clients.Group($"User_{participantId}")
                        .SendAsync("NewConversation", conversation.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending new conversation notification via SignalR");
            }

            return await GetConversationDtoAsync(conversation.Id, creatorId);
        }

        public async Task<ConversationDetailDto> GetConversationByIdAsync(Guid conversationId, Guid userId)
        {
            // Check if the user is a participant in the conversation
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == conversationId && cp.UserId == userId);

            if (!isParticipant)
            {
                return null;
            }

            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(50))
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                return null;
            }

            var result = new ConversationDetailDto
            {
                Id = conversation.Id,
                Title = conversation.Title,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                LastMessageAt = conversation.LastMessageAt,
                Participants = _mapper.Map<List<ParticipantDto>>(conversation.Participants),
                Messages = _mapper.Map<List<MessageDto>>(conversation.Messages.OrderBy(m => m.CreatedAt).ToList())
            };

            return result;
        }

        public async Task<IEnumerable<ConversationDto>> GetUserConversationsAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var conversationIds = await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => cp.ConversationId)
                .ToListAsync();

            var conversations = await _context.Conversations
                .Include(c => c.Participants)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .Where(c => conversationIds.Contains(c.Id))
                .OrderByDescending(c => c.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<ConversationDto>();
            foreach (var conversation in conversations)
            {
                var lastMessage = conversation.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
                var userParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
                
                var unreadCount = 0;
                if (userParticipant?.LastReadAt != null && lastMessage != null)
                {
                    unreadCount = await _context.Messages
                        .CountAsync(m => m.ConversationId == conversation.Id 
                                 && m.CreatedAt > userParticipant.LastReadAt
                                 && m.SenderId != userId);
                }

                result.Add(new ConversationDto
                {
                    Id = conversation.Id,
                    Title = conversation.Title,
                    Participants = _mapper.Map<List<ParticipantDto>>(conversation.Participants),
                    LastMessage = _mapper.Map<MessageDto>(lastMessage),
                    UnreadCount = unreadCount,
                    CreatedAt = conversation.CreatedAt,
                    UpdatedAt = conversation.UpdatedAt,
                    LastMessageAt = conversation.LastMessageAt
                });
            }

            return result;
        }

        public async Task<ParticipantDto> AddParticipantAsync(Guid currentUserId, AddParticipantDto addParticipantDto)
        {
            // Verify the current user is a participant
            var isParticipant = await _context.ConversationParticipants
                .AnyAsync(cp => cp.ConversationId == addParticipantDto.ConversationId && cp.UserId == currentUserId);

            if (!isParticipant)
            {
                throw new UnauthorizedAccessException("You are not a participant in this conversation");
            }

            // Check if the user is already a participant
            var existingParticipant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == addParticipantDto.ConversationId && cp.UserId == addParticipantDto.UserId);

            if (existingParticipant != null)
            {
                return _mapper.Map<ParticipantDto>(existingParticipant);
            }

            // Add the new participant
            var participant = new ConversationParticipantModel
            {
                ConversationId = addParticipantDto.ConversationId,
                UserId = addParticipantDto.UserId,
                JoinedAt = DateTime.UtcNow
            };

            _context.ConversationParticipants.Add(participant);
            await _context.SaveChangesAsync();

            // Notify all participants about the new participant
            try
            {
                var conversation = await _context.Conversations
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == addParticipantDto.ConversationId);

                if (conversation != null)
                {
                    var participantDto = _mapper.Map<ParticipantDto>(participant);
                    
                    foreach (var existingParticipantId in conversation.Participants.Select(p => p.UserId))
                    {
                        await _conversationHub.Clients.Group($"User_{existingParticipantId}")
                            .SendAsync("ParticipantAdded", addParticipantDto.ConversationId, participantDto);
                    }
                    
                    // Also notify the new participant to join the conversation
                    await _conversationHub.Clients.Group($"User_{addParticipantDto.UserId}")
                        .SendAsync("NewConversation", addParticipantDto.ConversationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending participant added notification via SignalR");
            }

            return _mapper.Map<ParticipantDto>(participant);
        }

        public async Task<bool> RemoveParticipantAsync(Guid currentUserId, RemoveParticipantDto removeParticipantDto)
        {
            // Only allow removing if the current user is the one being removed, or is already a participant
            bool isCurrentUserOrParticipant = currentUserId == removeParticipantDto.UserId ||
                await _context.ConversationParticipants
                    .AnyAsync(cp => cp.ConversationId == removeParticipantDto.ConversationId && cp.UserId == currentUserId);

            if (!isCurrentUserOrParticipant)
            {
                throw new UnauthorizedAccessException("You are not authorized to remove participants from this conversation");
            }

            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == removeParticipantDto.ConversationId && cp.UserId == removeParticipantDto.UserId);

            if (participant == null)
            {
                return false;
            }

            _context.ConversationParticipants.Remove(participant);
            await _context.SaveChangesAsync();

            // Notify all participants about the removed participant
            try
            {
                var conversation = await _context.Conversations
                    .Include(c => c.Participants)
                    .FirstOrDefaultAsync(c => c.Id == removeParticipantDto.ConversationId);

                if (conversation != null)
                {
                    foreach (var remainingParticipantId in conversation.Participants.Select(p => p.UserId))
                    {
                        await _conversationHub.Clients.Group($"User_{remainingParticipantId}")
                            .SendAsync("ParticipantRemoved", removeParticipantDto.ConversationId, removeParticipantDto.UserId);
                    }
                    
                    // Notify the removed participant
                    await _conversationHub.Clients.Group($"User_{removeParticipantDto.UserId}")
                        .SendAsync("RemovedFromConversation", removeParticipantDto.ConversationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending participant removed notification via SignalR");
            }

            return true;
        }

        public async Task<bool> MarkConversationAsReadAsync(Guid userId, MarkConversationReadDto markReadDto)
        {
            var participant = await _context.ConversationParticipants
                .FirstOrDefaultAsync(cp => cp.ConversationId == markReadDto.ConversationId && cp.UserId == userId);

            if (participant == null)
            {
                return false;
            }

            participant.LastReadAt = markReadDto.ReadTimestamp;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetUnreadConversationsCountAsync(Guid userId)
        {
            var conversationIds = await _context.ConversationParticipants
                .Where(cp => cp.UserId == userId)
                .Select(cp => new { cp.ConversationId, cp.LastReadAt })
                .ToListAsync();

            var count = 0;
            foreach (var conversation in conversationIds)
            {
                var lastMessageTime = await _context.Messages
                    .Where(m => m.ConversationId == conversation.ConversationId && m.SenderId != userId)
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefaultAsync();

                if (lastMessageTime != default && (conversation.LastReadAt == null || lastMessageTime > conversation.LastReadAt))
                {
                    count++;
                }
            }

            return count;
        }

        private async Task<ConversationDto> GetConversationDtoAsync(Guid conversationId, Guid userId)
        {
            var conversation = await _context.Conversations
                .Include(c => c.Participants)
                .Include(c => c.Messages.OrderByDescending(m => m.CreatedAt).Take(1))
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                return null;
            }

            var lastMessage = conversation.Messages.OrderByDescending(m => m.CreatedAt).FirstOrDefault();
            var userParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == userId);
            
            var unreadCount = 0;
            if (userParticipant?.LastReadAt != null && lastMessage != null)
            {
                unreadCount = await _context.Messages
                    .CountAsync(m => m.ConversationId == conversation.Id 
                             && m.CreatedAt > userParticipant.LastReadAt
                             && m.SenderId != userId);
            }

            return new ConversationDto
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Participants = _mapper.Map<List<ParticipantDto>>(conversation.Participants),
                LastMessage = _mapper.Map<MessageDto>(lastMessage),
                UnreadCount = unreadCount,
                CreatedAt = conversation.CreatedAt,
                UpdatedAt = conversation.UpdatedAt,
                LastMessageAt = conversation.LastMessageAt
            };
        }
    }
}
