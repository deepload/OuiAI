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
    public class FollowsController : ControllerBase
    {
        private readonly IFollowService _followService;

        public FollowsController(IFollowService followService)
        {
            _followService = followService;
        }

        [HttpPost]
        public async Task<IActionResult> FollowUser(FollowRequestDto followRequest)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _followService.FollowUserAsync(userId, followRequest);
            return Ok(result);
        }

        [HttpDelete("{followeeId}")]
        public async Task<IActionResult> UnfollowUser(Guid followeeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _followService.UnfollowUserAsync(userId, followeeId);
            return Ok(result);
        }

        [HttpGet("check/{followeeId}")]
        public async Task<IActionResult> CheckFollowStatus(Guid followeeId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _followService.IsFollowingAsync(userId, followeeId);
            return Ok(new { isFollowing = result });
        }

        [HttpGet("followers/{userId}")]
        public async Task<IActionResult> GetFollowers(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var followers = await _followService.GetFollowersAsync(userId, page, pageSize);
            var count = await _followService.GetFollowersCountAsync(userId);
            return Ok(new { followers, count });
        }

        [HttpGet("following/{userId}")]
        public async Task<IActionResult> GetFollowing(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var following = await _followService.GetFollowingAsync(userId, page, pageSize);
            var count = await _followService.GetFollowingCountAsync(userId);
            return Ok(new { following, count });
        }

        [HttpGet("suggestions")]
        public async Task<IActionResult> GetSuggestions([FromQuery] int count = 10)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var suggestions = await _followService.GetSuggestedUsersToFollowAsync(userId, count);
            return Ok(suggestions);
        }
    }
}
