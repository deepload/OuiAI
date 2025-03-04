using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OuiAI.Common.DTOs;
using OuiAI.Microservices.Projects.DTOs;
using OuiAI.Microservices.Projects.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetCommentsByProject(Guid projectId)
        {
            var comments = await _commentService.GetCommentsByProjectIdAsync(projectId);

            var response = new ApiResponse<IEnumerable<CommentDto>>
            {
                Success = true,
                Data = comments
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CommentDto>>> GetComment(Guid id)
        {
            var comment = await _commentService.GetCommentByIdAsync(id);

            if (comment == null)
            {
                return NotFound(new ApiResponse<CommentDto>
                {
                    Success = false,
                    Message = "Comment not found"
                });
            }

            var response = new ApiResponse<CommentDto>
            {
                Success = true,
                Data = comment
            };

            return Ok(response);
        }

        [HttpGet("{id}/replies")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CommentDto>>>> GetCommentReplies(Guid id)
        {
            var replies = await _commentService.GetRepliesByCommentIdAsync(id);

            var response = new ApiResponse<IEnumerable<CommentDto>>
            {
                Success = true,
                Data = replies
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CommentDto>>> CreateComment([FromBody] CreateCommentDto commentDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var profileImageUrl = User.FindFirstValue("ProfileImageUrl") ?? string.Empty;

            var createdComment = await _commentService.CreateCommentAsync(commentDto, userId, userName, profileImageUrl);

            var response = new ApiResponse<CommentDto>
            {
                Success = true,
                Message = "Comment created successfully",
                Data = createdComment
            };

            return CreatedAtAction(nameof(GetComment), new { id = createdComment.Id }, response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CommentDto>>> UpdateComment(Guid id, [FromBody] UpdateCommentDto commentDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            try
            {
                var updatedComment = await _commentService.UpdateCommentAsync(id, commentDto, userId);

                var response = new ApiResponse<CommentDto>
                {
                    Success = true,
                    Message = "Comment updated successfully",
                    Data = updatedComment
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CommentDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteComment(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            bool isAdmin = User.IsInRole("Admin");
            bool isModerator = User.IsInRole("Moderator");

            try
            {
                await _commentService.DeleteCommentAsync(id, userId);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Comment deleted successfully",
                    Data = true
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = ex.Message,
                    Data = false
                });
            }
        }
    }
}
