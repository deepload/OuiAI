using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OuiAI.Common.DTOs;
using OuiAI.Microservices.Identity.DTOs;
using OuiAI.Microservices.Identity.Services;

namespace OuiAI.Microservices.Identity.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IIdentityService identityService, ILogger<UsersController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserByUsername(string username)
        {
            try
            {
                var user = await _identityService.GetUserByUsernameAsync(username);
                
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by username: {username}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving user data"));
            }
        }

        [HttpGet("id/{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(Guid id)
        {
            try
            {
                var user = await _identityService.GetUserByIdAsync(id);
                
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting user by id: {id}");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving user data"));
            }
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<ActionResult<ApiResponse<UserDto>>> UpdateProfile([FromBody] UpdateProfileDto profileDto)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResponse("User is not authenticated"));
                }

                var updatedUser = await _identityService.UpdateProfileAsync(userGuid, profileDto);
                
                if (updatedUser == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(updatedUser, "Profile updated successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while updating profile"));
            }
        }

        [Authorize]
        [HttpPost("follow/{userToFollowId}")]
        public async Task<ActionResult<ApiResponse<bool>>> FollowUser(Guid userToFollowId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated"));
                }

                if (userGuid == userToFollowId)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("You cannot follow yourself"));
                }

                var result = await _identityService.FollowUserAsync(userGuid, userToFollowId);
                
                if (!result)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to follow user"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User followed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error following user: {userToFollowId}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while following user"));
            }
        }

        [Authorize]
        [HttpPost("unfollow/{userToUnfollowId}")]
        public async Task<ActionResult<ApiResponse<bool>>> UnfollowUser(Guid userToUnfollowId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(ApiResponse<bool>.ErrorResponse("User is not authenticated"));
                }

                var result = await _identityService.UnfollowUserAsync(userGuid, userToUnfollowId);
                
                if (!result)
                {
                    return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to unfollow user"));
                }

                return Ok(ApiResponse<bool>.SuccessResponse(true, "User unfollowed successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error unfollowing user: {userToUnfollowId}");
                return StatusCode(500, ApiResponse<bool>.ErrorResponse("An error occurred while unfollowing user"));
            }
        }

        [HttpGet("{userId}/followers")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<UserDto>>>> GetFollowers(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var followers = await _identityService.GetFollowersAsync(userId, page, pageSize);
                return Ok(ApiResponse<PaginatedResult<UserDto>>.SuccessResponse(followers));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting followers for user: {userId}");
                return StatusCode(500, ApiResponse<PaginatedResult<UserDto>>.ErrorResponse("An error occurred while retrieving followers"));
            }
        }

        [HttpGet("{userId}/following")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<UserDto>>>> GetFollowing(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var following = await _identityService.GetFollowingAsync(userId, page, pageSize);
                return Ok(ApiResponse<PaginatedResult<UserDto>>.SuccessResponse(following));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting following for user: {userId}");
                return StatusCode(500, ApiResponse<PaginatedResult<UserDto>>.ErrorResponse("An error occurred while retrieving following"));
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<PaginatedResult<UserDto>>>> SearchUsers([FromQuery] string q, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return BadRequest(ApiResponse<PaginatedResult<UserDto>>.ErrorResponse("Search query cannot be empty"));
                }

                var results = await _identityService.SearchUsersAsync(q, page, pageSize);
                return Ok(ApiResponse<PaginatedResult<UserDto>>.SuccessResponse(results));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error searching users with query: {q}");
                return StatusCode(500, ApiResponse<PaginatedResult<UserDto>>.ErrorResponse("An error occurred while searching users"));
            }
        }
    }
}
