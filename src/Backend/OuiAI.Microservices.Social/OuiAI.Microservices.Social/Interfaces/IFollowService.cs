using OuiAI.Microservices.Social.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Interfaces
{
    public interface IFollowService
    {
        Task<UserFollowDto> FollowUserAsync(Guid followerId, FollowRequestDto followRequest);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<bool> IsFollowingAsync(Guid followerId, Guid followeeId);
        Task<IEnumerable<FollowerDto>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<IEnumerable<FolloweeDto>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 20);
        Task<int> GetFollowersCountAsync(Guid userId);
        Task<int> GetFollowingCountAsync(Guid userId);
        Task<IEnumerable<FolloweeDto>> GetSuggestedUsersToFollowAsync(Guid userId, int count = 10);
    }
}
