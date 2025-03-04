using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OuiAI.Microservices.Social.DTOs;
using OuiAI.Microservices.Social.Interfaces;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IConversationService _conversationService;
        private readonly IMessageService _messageService;

        public ConversationsController(IConversationService conversationService, IMessageService messageService)
        {
            _conversationService = conversationService;
            _messageService = messageService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserConversations([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversations = await _conversationService.GetUserConversationsAsync(userId, page, pageSize);
            var unreadCount = await _conversationService.GetUnreadConversationsCountAsync(userId);
            return Ok(new { conversations, unreadCount });
        }

        [HttpGet("{conversationId}")]
        public async Task<IActionResult> GetConversationById(Guid conversationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversation = await _conversationService.GetConversationByIdAsync(conversationId, userId);
            
            if (conversation == null)
                return NotFound();
                
            return Ok(conversation);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> GetConversationMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            var messages = await _messageService.GetConversationMessagesAsync(conversationId, page, pageSize);
            return Ok(messages);
        }

        [HttpPost]
        public async Task<IActionResult> CreateConversation(CreateConversationDto conversationDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversation = await _conversationService.CreateConversationAsync(userId, conversationDto);
            return CreatedAtAction(nameof(GetConversationById), new { conversationId = conversation.Id }, conversation);
        }

        [HttpPost("{conversationId}/messages")]
        public async Task<IActionResult> CreateMessage(Guid conversationId, [FromBody] string content)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var messageDto = new CreateMessageDto
            {
                ConversationId = conversationId,
                Content = content
            };
            
            var message = await _messageService.CreateMessageAsync(userId, messageDto);
            return Ok(message);
        }

        [HttpPost("{conversationId}/participants")]
        public async Task<IActionResult> AddParticipant(Guid conversationId, [FromBody] Guid participantId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var addParticipantDto = new AddParticipantDto
            {
                ConversationId = conversationId,
                UserId = participantId
            };
            
            var participant = await _conversationService.AddParticipantAsync(userId, addParticipantDto);
            return Ok(participant);
        }

        [HttpDelete("{conversationId}/participants/{participantId}")]
        public async Task<IActionResult> RemoveParticipant(Guid conversationId, Guid participantId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var removeParticipantDto = new RemoveParticipantDto
            {
                ConversationId = conversationId,
                UserId = participantId
            };
            
            var result = await _conversationService.RemoveParticipantAsync(userId, removeParticipantDto);
            return Ok(new { success = result });
        }

        [HttpPost("{conversationId}/read")]
        public async Task<IActionResult> MarkConversationAsRead(Guid conversationId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var markReadDto = new MarkConversationReadDto
            {
                ConversationId = conversationId,
                ReadTimestamp = DateTime.UtcNow
            };
            
            var result = await _conversationService.MarkConversationAsReadAsync(userId, markReadDto);
            return Ok(new { success = result });
        }

        [HttpGet("unread/count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _conversationService.GetUnreadConversationsCountAsync(userId);
            return Ok(new { count });
        }
    }
}
