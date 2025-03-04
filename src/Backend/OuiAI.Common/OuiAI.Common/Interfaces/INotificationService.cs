using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OuiAI.Common.DTOs;
using OuiAI.Common.Models;

namespace OuiAI.Common.Interfaces
{
    public interface INotificationService
    {
        Task<Notification> GetNotificationByIdAsync(Guid notificationId);
        Task<PaginatedResult<Notification>> GetUserNotificationsAsync(Guid userId, int pageNumber, int pageSize, bool includeRead = false);
        Task<Notification> CreateNotificationAsync(Notification notification);
        Task<bool> MarkNotificationAsReadAsync(Guid notificationId);
        Task<bool> MarkAllNotificationsAsReadAsync(Guid userId);
        Task<bool> DeleteNotificationAsync(Guid notificationId);
        Task<int> GetUnreadNotificationCountAsync(Guid userId);
    }
}
