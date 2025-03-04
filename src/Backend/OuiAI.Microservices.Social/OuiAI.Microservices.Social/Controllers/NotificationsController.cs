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
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications([FromQuery] bool unreadOnly = false, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly, page, pageSize);
            var unreadCount = await _notificationService.GetUnreadNotificationCountAsync(userId);
            return Ok(new { notifications, unreadCount });
        }

        [HttpGet("count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var count = await _notificationService.GetUnreadNotificationCountAsync(userId);
            return Ok(new { count });
        }

        [HttpPost("read/{notificationId}")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            var result = await _notificationService.MarkNotificationAsReadAsync(notificationId);
            return Ok(new { success = result });
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _notificationService.MarkAllNotificationsAsReadAsync(userId);
            return Ok(new { success = result });
        }

        [HttpDelete("{notificationId}")]
        public async Task<IActionResult> DeleteNotification(Guid notificationId)
        {
            var result = await _notificationService.DeleteNotificationAsync(notificationId);
            return Ok(new { success = result });
        }

        // Admin-only endpoint to create notifications manually
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto notification)
        {
            var result = await _notificationService.CreateNotificationAsync(notification);
            return Ok(result);
        }
    }
}
