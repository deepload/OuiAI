using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Hubs
{
    [Authorize]
    public class ConversationHub : Hub
    {
        private readonly ILogger<ConversationHub> _logger;

        public ConversationHub(ILogger<ConversationHub> logger)
        {
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = Context.User.FindFirst("sub")?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation($"User {userId} connected to ConversationHub");
            }
            
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.User.FindFirst("sub")?.Value;
            
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId}");
                _logger.LogInformation($"User {userId} disconnected from ConversationHub");
            }
            
            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinConversation(string conversationId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            _logger.LogInformation($"Client {Context.ConnectionId} joined conversation {conversationId}");
        }

        public async Task LeaveConversation(string conversationId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Conversation_{conversationId}");
            _logger.LogInformation($"Client {Context.ConnectionId} left conversation {conversationId}");
        }

        public async Task SendMessage(string conversationId, string message)
        {
            var userId = Context.User.FindFirst("sub")?.Value;
            var username = Context.User.FindFirst("name")?.Value ?? "Unknown";
            
            await Clients.Group($"Conversation_{conversationId}").SendAsync("ReceiveMessage", userId, username, message, DateTime.UtcNow);
            _logger.LogInformation($"Message sent to conversation {conversationId} by user {userId}");
        }

        public async Task SendTypingNotification(string conversationId)
        {
            var userId = Context.User.FindFirst("sub")?.Value;
            var username = Context.User.FindFirst("name")?.Value ?? "Unknown";
            
            await Clients.GroupExcept($"Conversation_{conversationId}", Context.ConnectionId)
                          .SendAsync("UserTyping", userId, username);
        }
    }
}
