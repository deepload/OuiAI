using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OuiAI.Common.DTOs;
using OuiAI.Common.Models;

namespace OuiAI.Common.Interfaces
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByUsernameAsync(string username);
        Task<User> GetUserByEmailAsync(string email);
        Task<User> CreateUserAsync(User user, string password);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> FollowUserAsync(Guid followerId, Guid followeeId);
        Task<bool> UnfollowUserAsync(Guid followerId, Guid followeeId);
        Task<PaginatedResult<User>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize);
        Task<PaginatedResult<User>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize);
        Task<bool> VerifyUserCredentialsAsync(string usernameOrEmail, string password);
    }
}
