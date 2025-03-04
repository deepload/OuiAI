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
    public class ActivitiesController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserActivities(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var activities = await _activityService.GetUserActivitiesAsync(userId, page, pageSize);
            return Ok(activities);
        }

        [HttpGet("following")]
        public async Task<IActionResult> GetFollowingActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var activities = await _activityService.GetFollowingActivitiesAsync(userId, page, pageSize);
            return Ok(activities);
        }

        [HttpGet("global")]
        public async Task<IActionResult> GetGlobalActivities([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var activities = await _activityService.GetGlobalActivitiesAsync(page, pageSize);
            return Ok(activities);
        }

        [HttpDelete("{activityId}")]
        public async Task<IActionResult> DeleteActivity(Guid activityId)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Check if the activity exists and belongs to the user
            var activityOwnership = await _activityService.CheckActivityOwnershipAsync(activityId, userId);
            
            if (!activityOwnership.Exists)
            {
                return NotFound(new { message = "Activity not found" });
            }
            
            if (!activityOwnership.IsOwner && !User.IsInRole("Admin"))
            {
                return Forbid();
            }
            
            var result = await _activityService.DeleteActivityAsync(activityId);
            
            if (!result)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the activity" });
            }
            
            return Ok(new { success = true, message = "Activity deleted successfully" });
        }

        // Admin-only endpoint to create activities manually
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateActivity(CreateActivityDto activity)
        {
            var result = await _activityService.CreateActivityAsync(activity);
            return Ok(result);
        }
    }
}
