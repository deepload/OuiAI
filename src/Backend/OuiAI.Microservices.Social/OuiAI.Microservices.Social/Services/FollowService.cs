using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OuiAI.Common.Interfaces;
using OuiAI.Microservices.Social.Data;
using OuiAI.Microservices.Social.DTOs;
using OuiAI.Microservices.Social.Interfaces;
using OuiAI.Microservices.Social.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Social.Services
{
    public class FollowService : IFollowService
    {
        private readonly SocialDbContext _context;
        private readonly IMapper _mapper;
        private readonly IServiceBusPublisher _serviceBusPublisher;
        private readonly ILogger<FollowService> _logger;

        public FollowService(
            SocialDbContext context,
            IMapper mapper,
            IServiceBusPublisher serviceBusPublisher,
            ILogger<FollowService> logger)
        {
            _context = context;
            _mapper = mapper;
            _serviceBusPublisher = serviceBusPublisher;
            _logger = logger;
        }

        public async Task<UserFollowDto> FollowUserAsync(Guid followerId, FollowRequestDto followRequest)
        {
            // Check if already following
            var existingFollow = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followRequest.FolloweeId);

            if (existingFollow != null)
            {
                return _mapper.Map<UserFollowDto>(existingFollow);
            }

            // Prevent self-following
            if (followerId == followRequest.FolloweeId)
            {
                throw new InvalidOperationException("Users cannot follow themselves");
            }

            // Create new follow relationship
            var userFollow = new UserFollowModel
            {
                FollowerId = followerId,
                FolloweeId = followRequest.FolloweeId,
                CreatedAt = DateTime.UtcNow
            };

            _context.UserFollows.Add(userFollow);
            await _context.SaveChangesAsync();

            // Publish follow event to service bus for notifications
            try
            {
                await _serviceBusPublisher.PublishMessageAsync("user-followed", new 
                { 
                    FollowerId = followerId,
                    FolloweeId = followRequest.FolloweeId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing follow event to service bus");
            }

            return _mapper.Map<UserFollowDto>(userFollow);
        }

        public async Task<bool> UnfollowUserAsync(Guid followerId, Guid followeeId)
        {
            var followRelationship = await _context.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

            if (followRelationship == null)
            {
                return false;
            }

            _context.UserFollows.Remove(followRelationship);
            await _context.SaveChangesAsync();

            // Publish unfollow event to service bus
            try
            {
                await _serviceBusPublisher.PublishMessageAsync("user-unfollowed", new 
                { 
                    FollowerId = followerId,
                    FolloweeId = followeeId,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing unfollow event to service bus");
            }

            return true;
        }

        public async Task<bool> IsFollowingAsync(Guid followerId, Guid followeeId)
        {
            return await _context.UserFollows
                .AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);
        }

        public async Task<IEnumerable<FollowerDto>> GetFollowersAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            return await _context.UserFollows
                .Where(f => f.FolloweeId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FollowerDto
                {
                    UserId = f.FollowerId,
                    Username = f.FollowerUsername,
                    DisplayName = f.FollowerDisplayName,
                    ProfileImageUrl = f.FollowerProfileImageUrl,
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<FolloweeDto>> GetFollowingAsync(Guid userId, int page = 1, int pageSize = 20)
        {
            return await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .OrderByDescending(f => f.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(f => new FolloweeDto
                {
                    UserId = f.FolloweeId,
                    Username = f.FolloweeUsername,
                    DisplayName = f.FolloweeDisplayName,
                    ProfileImageUrl = f.FolloweeProfileImageUrl,
                    FollowedAt = f.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<int> GetFollowersCountAsync(Guid userId)
        {
            return await _context.UserFollows.CountAsync(f => f.FolloweeId == userId);
        }

        public async Task<int> GetFollowingCountAsync(Guid userId)
        {
            return await _context.UserFollows.CountAsync(f => f.FollowerId == userId);
        }

        public async Task<IEnumerable<FolloweeDto>> GetSuggestedUsersToFollowAsync(Guid userId, int count = 10)
        {
            // Get users that the current user's followees follow (but the current user doesn't follow yet)
            var following = await _context.UserFollows
                .Where(f => f.FollowerId == userId)
                .Select(f => f.FolloweeId)
                .ToListAsync();
            
            following.Add(userId); // Add current user to exclude from suggestions
            
            var suggestedUserIds = await _context.UserFollows
                .Where(f => following.Contains(f.FollowerId) && !following.Contains(f.FolloweeId))
                .Select(f => f.FolloweeId)
                .Distinct()
                .Take(count)
                .ToListAsync();
            
            if (suggestedUserIds.Count < count)
            {
                // If we don't have enough recommendations, get some popular users
                var additionalUserIds = await _context.UserFollows
                    .GroupBy(f => f.FolloweeId)
                    .OrderByDescending(g => g.Count())
                    .Where(g => !following.Contains(g.Key))
                    .Select(g => g.Key)
                    .Take(count - suggestedUserIds.Count)
                    .ToListAsync();
                
                suggestedUserIds.AddRange(additionalUserIds);
            }
            
            // Get user details for suggested user IDs
            var suggestedUsers = await _context.UserFollows
                .Where(f => suggestedUserIds.Contains(f.FolloweeId))
                .Select(f => new FolloweeDto
                {
                    UserId = f.FolloweeId,
                    Username = f.FolloweeUsername,
                    DisplayName = f.FolloweeDisplayName,
                    ProfileImageUrl = f.FolloweeProfileImageUrl,
                    FollowedAt = DateTime.UtcNow
                })
                .Take(count)
                .ToListAsync();
            
            return suggestedUsers;
        }
    }
}
