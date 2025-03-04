using OuiAI.Microservices.Social.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Interfaces
{
    public interface IActivityService
    {
        Task<ActivityDto> CreateActivityAsync(CreateActivityDto activity);
        Task<IEnumerable<ActivityDto>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<ActivityDto>> GetFollowingActivitiesAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<ActivityDto>> GetGlobalActivitiesAsync(int page = 1, int pageSize = 20);
        Task<bool> DeleteActivityAsync(Guid activityId);
        Task ProcessActivityFromServiceBusAsync(string activityJson);
        Task<ActivityOwnershipDto> CheckActivityOwnershipAsync(Guid activityId, Guid userId);
    }
}
