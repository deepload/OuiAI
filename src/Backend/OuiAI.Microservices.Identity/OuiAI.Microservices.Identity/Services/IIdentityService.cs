using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OuiAI.Common.DTOs;
using OuiAI.Microservices.Identity.DTOs;

namespace OuiAI.Microservices.Identity.Services
{
    public interface IIdentityService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
        Task<UserDto> GetUserByIdAsync(Guid userId);
        Task<UserDto> GetUserByUsernameAsync(string username);
        Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto);
        Task<bool> FollowUserAsync(Guid currentUserId, Guid userToFollowId);
        Task<bool> UnfollowUserAsync(Guid currentUserId, Guid userToUnfollowId);
        Task<PaginatedResult<UserDto>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize);
        Task<PaginatedResult<UserDto>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize);
        Task<PaginatedResult<UserDto>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize);
    }
}
