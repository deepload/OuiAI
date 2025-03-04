using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OuiAI.Microservices.Social.Data;
using OuiAI.Microservices.Social.DTOs;
using OuiAI.Microservices.Social.Interfaces;
using OuiAI.Microservices.Social.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Services
{
    public class ActivityService : IActivityService
    {
        private readonly SocialDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<ActivityService> _logger;

        public ActivityService(
            SocialDbContext context,
            IMapper mapper,
            ILogger<ActivityService> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ActivityDto> CreateActivityAsync(CreateActivityDto activityDto)
        {
            var activity = _mapper.Map<ActivityModel>(activityDto);
            activity.Id = Guid.NewGuid();
            activity.CreatedAt = DateTime.UtcNow;

            _context.Activities.Add(activity);
            await _context.SaveChangesAsync();

            return _mapper.Map<ActivityDto>(activity);
        }

        public async Task<IEnumerable<ActivityDto>> GetUserActivitiesAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            var activities = await _context.Activities
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<IEnumerable<ActivityDto>> GetFollowingActivitiesAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            // Get IDs of users that the current user follows
            var followingIds = await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();

            // Get activities from followed users
            var activities = await _context.Activities
                .Where(a => followingIds.Contains(a.UserId))
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<IEnumerable<ActivityDto>> GetGlobalActivitiesAsync(int page = 1, int pageSize = 20)
        {
            var activities = await _context.Activities
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ActivityDto>>(activities);
        }

        public async Task<bool> DeleteActivityAsync(Guid activityId)
        {
            var activity = await _context.Activities.FindAsync(activityId);
            if (activity == null)
            {
                return false;
            }

            _context.Activities.Remove(activity);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ActivityOwnershipDto> CheckActivityOwnershipAsync(Guid activityId, Guid userId)
        {
            var result = new ActivityOwnershipDto
            {
                Exists = false,
                IsOwner = false,
                ActivityId = activityId,
                OwnerId = null
            };

            var activity = await _context.Activities.FindAsync(activityId);
            
            if (activity == null)
            {
                return result;
            }
            
            result.Exists = true;
            result.OwnerId = activity.UserId;
            result.IsOwner = activity.UserId == userId;
            
            return result;
        }

        public async Task ProcessActivityFromServiceBusAsync(string activityJson)
        {
            try
            {
                var activity = JsonSerializer.Deserialize<CreateActivityDto>(activityJson);
                if (activity != null)
                {
                    await CreateActivityAsync(activity);
                    _logger.LogInformation($"Processed activity from service bus for user {activity.UserId}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing activity from service bus");
            }
        }
    }
}
