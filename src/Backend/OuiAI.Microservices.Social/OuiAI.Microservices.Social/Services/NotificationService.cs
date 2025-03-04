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
using System.Text.Json;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Services
{
    public class NotificationService : INotificationService
    {
        private readonly SocialDbContext _context;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHub;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            SocialDbContext context,
            IMapper mapper,
            IHubContext<NotificationHub> notificationHub,
            ILogger<NotificationService> logger)
        {
            _context = context;
            _mapper = mapper;
            _notificationHub = notificationHub;
            _logger = logger;
        }

        public async Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto notificationDto)
        {
            var notification = _mapper.Map<NotificationModel>(notificationDto);
            notification.Id = Guid.NewGuid();
            notification.IsRead = false;
            notification.CreatedAt = DateTime.UtcNow;

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            var result = _mapper.Map<NotificationDto>(notification);

            // Send real-time notification via SignalR
            try
            {
                await _notificationHub.Clients.Group($"User_{notification.UserId}")
                    .SendAsync("ReceiveNotification", result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification via SignalR");
            }

            return result;
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, int page = 1, int pageSize = 20)
        {
            var query = _context.Notifications
                .Where(n => n.UserId == userId);

            if (unreadOnly)
            {
                query = query.Where(n => !n.IsRead);
            }

            var notifications = await query
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<NotificationDto>>(notifications);
        }

        public async Task<int> GetUnreadNotificationCountAsync(Guid userId)
        {
            return await _context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        public async Task<bool> MarkNotificationAsReadAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            // Send update via SignalR
            try
            {
                await _notificationHub.Clients.Group($"User_{notification.UserId}")
                    .SendAsync("NotificationRead", notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification read status via SignalR");
            }

            return true;
        }

        public async Task<bool> MarkAllNotificationsAsReadAsync(Guid userId)
        {
            var unreadNotifications = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (!unreadNotifications.Any())
            {
                return true;
            }

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();

            // Send update via SignalR
            try
            {
                await _notificationHub.Clients.Group($"User_{userId}")
                    .SendAsync("AllNotificationsRead");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending all notifications read status via SignalR");
            }

            return true;
        }

        public async Task<bool> DeleteNotificationAsync(Guid notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification == null)
            {
                return false;
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            // Send update via SignalR
            try
            {
                await _notificationHub.Clients.Group($"User_{notification.UserId}")
                    .SendAsync("NotificationDeleted", notificationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification deleted status via SignalR");
            }

            return true;
        }

        public async Task ProcessNotificationFromServiceBusAsync(string notificationJson)
        {
            try
            {
                var notification = JsonSerializer.Deserialize<CreateNotificationDto>(notificationJson);
                if (notification != null)
                {
                    await CreateNotificationAsync(notification);
                    _logger.LogInformation($"Processed notification from service bus for user {notification.UserId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing notification from service bus");
            }
        }
    }
}
