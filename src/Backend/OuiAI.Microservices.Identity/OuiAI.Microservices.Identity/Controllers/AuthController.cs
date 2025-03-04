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
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IIdentityService identityService, ILogger<AuthController> logger)
        {
            _identityService = identityService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterUserDto registerDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid input", 
                    new List<string> { "Please check your input and try again." }));

            try
            {
                var result = await _identityService.RegisterAsync(registerDto);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Registration failed", result.Errors));
                }

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user registration");
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during registration"));
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Invalid input"));

            try
            {
                var result = await _identityService.LoginAsync(loginDto);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse("Login failed", result.Errors));
                }

                return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in user login");
                return StatusCode(500, ApiResponse<AuthResponseDto>.ErrorResponse("An error occurred during login"));
            }
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Unauthorized(ApiResponse<UserDto>.ErrorResponse("User is not authenticated"));
                }

                var user = await _identityService.GetUserByIdAsync(userGuid);
                
                if (user == null)
                {
                    return NotFound(ApiResponse<UserDto>.ErrorResponse("User not found"));
                }

                return Ok(ApiResponse<UserDto>.SuccessResponse(user));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting current user");
                return StatusCode(500, ApiResponse<UserDto>.ErrorResponse("An error occurred while retrieving user data"));
            }
        }
    }
}
