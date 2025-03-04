using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using OuiAI.Common.DTOs;
using OuiAI.Common.Interfaces;
using OuiAI.Microservices.Identity.Data;
using OuiAI.Microservices.Identity.DTOs;
using OuiAI.Microservices.Identity.Models;

namespace OuiAI.Microservices.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IdentityDbContext _dbContext;
        private readonly ILogger<IdentityService> _logger;
        private readonly IServiceBusPublisher _serviceBusPublisher;

        public IdentityService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            IdentityDbContext dbContext,
            ILogger<IdentityService> logger,
            IServiceBusPublisher serviceBusPublisher)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _dbContext = dbContext;
            _logger = logger;
            _serviceBusPublisher = serviceBusPublisher;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterUserDto registerDto)
        {
            var response = new AuthResponseDto();

            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByNameAsync(registerDto.Username) 
                    ?? await _userManager.FindByEmailAsync(registerDto.Email);

                if (existingUser != null)
                {
                    response.Success = false;
                    response.Errors.Add("Username or email already exists");
                    return response;
                }

                // Create new user
                var newUser = new ApplicationUser
                {
                    UserName = registerDto.Username,
                    Email = registerDto.Email,
                    CreatedAt = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(newUser, registerDto.Password);

                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Errors.AddRange(result.Errors.Select(e => e.Description));
                    return response;
                }

                // Add to default role
                await _userManager.AddToRoleAsync(newUser, "User");

                // Create user profile
                var profile = new UserProfile
                {
                    UserId = newUser.Id,
                    DisplayName = registerDto.DisplayName,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _dbContext.UserProfiles.Add(profile);
                await _dbContext.SaveChangesAsync();

                // Generate token
                response.Success = true;
                response.Token = await GenerateJwtTokenAsync(newUser);
                response.Expiration = DateTime.UtcNow.AddDays(7);
                response.User = await ConvertToUserDtoAsync(newUser);

                // Publish user created event to Service Bus
                await _serviceBusPublisher.PublishMessageAsync("user-events", new
                {
                    EventType = "UserCreated",
                    UserId = newUser.Id,
                    Username = newUser.UserName,
                    Email = newUser.Email,
                    DisplayName = registerDto.DisplayName,
                    Timestamp = DateTime.UtcNow
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                response.Success = false;
                response.Errors.Add("An error occurred during registration. Please try again.");
                return response;
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
        {
            var response = new AuthResponseDto();

            try
            {
                // Find user by username or email
                var user = await _userManager.FindByNameAsync(loginDto.UsernameOrEmail)
                    ?? await _userManager.FindByEmailAsync(loginDto.UsernameOrEmail);

                if (user == null)
                {
                    response.Success = false;
                    response.Errors.Add("Invalid username/email or password");
                    return response;
                }

                // Verify password
                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

                if (!result.Succeeded)
                {
                    response.Success = false;
                    response.Errors.Add("Invalid username/email or password");
                    return response;
                }

                // Update last login timestamp
                user.LastLoginAt = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);

                // Generate token
                response.Success = true;
                response.Token = await GenerateJwtTokenAsync(user);
                response.Expiration = DateTime.UtcNow.AddDays(7);
                response.User = await ConvertToUserDtoAsync(user);

                // Publish user login event to Service Bus
                await _serviceBusPublisher.PublishMessageAsync("user-events", new
                {
                    EventType = "UserLoggedIn",
                    UserId = user.Id,
                    Username = user.UserName,
                    Timestamp = DateTime.UtcNow
                });

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                response.Success = false;
                response.Errors.Add("An error occurred during login. Please try again.");
                return response;
            }
        }

        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return null;

            return await ConvertToUserDtoAsync(user);
        }

        public async Task<UserDto> GetUserByUsernameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            
            if (user == null)
                return null;

            return await ConvertToUserDtoAsync(user);
        }

        public async Task<UserDto> UpdateProfileAsync(Guid userId, UpdateProfileDto profileDto)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            
            if (user == null)
                return null;

            // Get or create profile
            var profile = await _dbContext.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userId);
            
            if (profile == null)
            {
                profile = new UserProfile
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                _dbContext.UserProfiles.Add(profile);
            }

            // Update profile properties
            profile.DisplayName = profileDto.DisplayName ?? profile.DisplayName;
            profile.Bio = profileDto.Bio ?? profile.Bio;
            profile.ProfileImageUrl = profileDto.ProfileImageUrl ?? profile.ProfileImageUrl;
            profile.Website = profileDto.Website ?? profile.Website;
            profile.Location = profileDto.Location ?? profile.Location;
            profile.TwitterUsername = profileDto.TwitterUsername ?? profile.TwitterUsername;
            profile.GitHubUsername = profileDto.GitHubUsername ?? profile.GitHubUsername;
            profile.UpdatedAt = DateTime.UtcNow;

            await _dbContext.SaveChangesAsync();
            
            // Publish profile updated event to Service Bus
            await _serviceBusPublisher.PublishMessageAsync("user-events", new
            {
                EventType = "UserProfileUpdated",
                UserId = user.Id,
                Username = user.UserName,
                DisplayName = profile.DisplayName,
                Timestamp = DateTime.UtcNow
            });

            return await ConvertToUserDtoAsync(user);
        }

        public async Task<bool> FollowUserAsync(Guid currentUserId, Guid userToFollowId)
        {
            // Don't allow self-follow
            if (currentUserId == userToFollowId)
                return false;

            // Check if both users exist
            var currentUser = await _userManager.FindByIdAsync(currentUserId.ToString());
            var userToFollow = await _userManager.FindByIdAsync(userToFollowId.ToString());

            if (currentUser == null || userToFollow == null)
                return false;

            // Check if already following
            var existingFollow = await _dbContext.UserFollows
                .AnyAsync(f => f.FollowerId == currentUserId && f.FolloweeId == userToFollowId);

            if (existingFollow)
                return true; // Already following, so consider it successful

            // Create new follow relationship
            var follow = new UserFollow
            {
                FollowerId = currentUserId,
                FolloweeId = userToFollowId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.UserFollows.Add(follow);
            await _dbContext.SaveChangesAsync();

            // Publish follow event to Service Bus
            await _serviceBusPublisher.PublishMessageAsync("user-events", new
            {
                EventType = "UserFollowed",
                FollowerId = currentUserId,
                FolloweeId = userToFollowId,
                Timestamp = DateTime.UtcNow
            });

            return true;
        }

        public async Task<bool> UnfollowUserAsync(Guid currentUserId, Guid userToUnfollowId)
        {
            var follow = await _dbContext.UserFollows
                .FirstOrDefaultAsync(f => f.FollowerId == currentUserId && f.FolloweeId == userToUnfollowId);

            if (follow == null)
                return false;

            _dbContext.UserFollows.Remove(follow);
            await _dbContext.SaveChangesAsync();

            // Publish unfollow event to Service Bus
            await _serviceBusPublisher.PublishMessageAsync("user-events", new
            {
                EventType = "UserUnfollowed",
                FollowerId = currentUserId,
                FolloweeId = userToUnfollowId,
                Timestamp = DateTime.UtcNow
            });

            return true;
        }

        public async Task<PaginatedResult<UserDto>> GetFollowersAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserFollows
                .Where(f => f.FolloweeId == userId)
                .Include(f => f.Follower)
                .ThenInclude(u => u.Profile)
                .Select(f => f.Follower);

            return await PaginateUserResultsAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<UserDto>> GetFollowingAsync(Guid userId, int pageNumber, int pageSize)
        {
            var query = _dbContext.UserFollows
                .Where(f => f.FollowerId == userId)
                .Include(f => f.Followee)
                .ThenInclude(u => u.Profile)
                .Select(f => f.Followee);

            return await PaginateUserResultsAsync(query, pageNumber, pageSize);
        }

        public async Task<PaginatedResult<UserDto>> SearchUsersAsync(string searchTerm, int pageNumber, int pageSize)
        {
            var query = _userManager.Users
                .Include(u => u.Profile)
                .Where(u => u.UserName.Contains(searchTerm) || 
                            u.Email.Contains(searchTerm) || 
                            u.Profile.DisplayName.Contains(searchTerm));

            return await PaginateUserResultsAsync(query, pageNumber, pageSize);
        }

        private async Task<string> GenerateJwtTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Add roles to claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddDays(7);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserDto> ConvertToUserDtoAsync(ApplicationUser user)
        {
            await _dbContext.Entry(user)
                .Reference(u => u.Profile)
                .LoadAsync();

            var followersCount = await _dbContext.UserFollows.CountAsync(f => f.FolloweeId == user.Id);
            var followingCount = await _dbContext.UserFollows.CountAsync(f => f.FollowerId == user.Id);

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                DisplayName = user.Profile?.DisplayName ?? user.UserName,
                ProfileImageUrl = user.Profile?.ProfileImageUrl,
                Bio = user.Profile?.Bio,
                Website = user.Profile?.Website,
                Location = user.Profile?.Location,
                TwitterUsername = user.Profile?.TwitterUsername,
                GitHubUsername = user.Profile?.GitHubUsername,
                CreatedAt = user.CreatedAt,
                FollowersCount = followersCount,
                FollowingCount = followingCount
            };
        }

        private async Task<PaginatedResult<UserDto>> PaginateUserResultsAsync(IQueryable<ApplicationUser> query, int pageNumber, int pageSize)
        {
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var userDtos = new List<UserDto>();
            
            foreach (var user in users)
            {
                userDtos.Add(await ConvertToUserDtoAsync(user));
            }

            return new PaginatedResult<UserDto>
            {
                Items = userDtos,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = totalPages
            };
        }
    }
}
