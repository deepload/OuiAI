using OuiAI.Microservices.Social.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(CreateNotificationDto notification);
        Task<IEnumerable<NotificationDto>> GetUserNotificationsAsync(Guid userId, bool unreadOnly = false, int page = 1, int pageSize = 20);
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
        Task<bool> MarkNotificationAsReadAsync(Guid notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        Task ProcessNotificationFromServiceBusAsync(string notificationJson);
    }
}
