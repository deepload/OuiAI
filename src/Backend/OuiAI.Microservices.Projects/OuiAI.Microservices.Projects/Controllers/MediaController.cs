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
    public class MediaController : ControllerBase
    {
        private readonly IMediaService _mediaService;
        private readonly IProjectService _projectService;

        public MediaController(IMediaService mediaService, IProjectService projectService)
        {
            _mediaService = mediaService;
            _projectService = projectService;
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectMediaDto>>>> GetMediaByProject(Guid projectId)
        {
            var media = await _mediaService.GetMediaByProjectIdAsync(projectId);

            var response = new ApiResponse<IEnumerable<ProjectMediaDto>>
            {
                Success = true,
                Data = media
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectMediaDto>>> GetMedia(Guid id)
        {
            var media = await _mediaService.GetMediaByIdAsync(id);

            if (media == null)
            {
                return NotFound(new ApiResponse<ProjectMediaDto>
                {
                    Success = false,
                    Message = "Media not found"
                });
            }

            var response = new ApiResponse<ProjectMediaDto>
            {
                Success = true,
                Data = media
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProjectMediaDto>>> CreateMedia([FromBody] CreateMediaDto mediaDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            // Verify that the user owns the project
            var project = await _projectService.GetProjectByIdAsync(mediaDto.ProjectId);
            if (project == null)
            {
                return NotFound(new ApiResponse<ProjectMediaDto>
                {
                    Success = false,
                    Message = "Project not found"
                });
            }

            if (project.UserId != userId && !User.IsInRole("Admin"))
            {
                return Forbid();
            }

            var createdMedia = await _mediaService.CreateMediaAsync(mediaDto);

            var response = new ApiResponse<ProjectMediaDto>
            {
                Success = true,
                Message = "Media created successfully",
                Data = createdMedia
            };

            return CreatedAtAction(nameof(GetMedia), new { id = createdMedia.Id }, response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectMediaDto>>> UpdateMedia(Guid id, [FromBody] UpdateMediaDto mediaDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var media = await _mediaService.GetMediaByIdAsync(id);
                if (media == null)
                {
                    return NotFound(new ApiResponse<ProjectMediaDto>
                    {
                        Success = false,
                        Message = "Media not found"
                    });
                }

                // Verify that the user owns the project
                var project = await _projectService.GetProjectByIdAsync(media.ProjectId);
                if (project.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                var updatedMedia = await _mediaService.UpdateMediaAsync(id, mediaDto);

                var response = new ApiResponse<ProjectMediaDto>
                {
                    Success = true,
                    Message = "Media updated successfully",
                    Data = updatedMedia
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectMediaDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteMedia(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var media = await _mediaService.GetMediaByIdAsync(id);
                if (media == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Media not found",
                        Data = false
                    });
                }

                // Verify that the user owns the project
                var project = await _projectService.GetProjectByIdAsync(media.ProjectId);
                if (project.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _mediaService.DeleteMediaAsync(id);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Media deleted successfully",
                    Data = true
                };

                return Ok(response);
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

        [Authorize]
        [HttpPost("project/{projectId}/reorder")]
        public async Task<ActionResult<ApiResponse<bool>>> ReorderMedia(Guid projectId, [FromBody] List<Guid> mediaIdsInOrder)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                // Verify that the user owns the project
                var project = await _projectService.GetProjectByIdAsync(projectId);
                if (project == null)
                {
                    return NotFound(new ApiResponse<bool>
                    {
                        Success = false,
                        Message = "Project not found",
                        Data = false
                    });
                }

                if (project.UserId != userId && !User.IsInRole("Admin"))
                {
                    return Forbid();
                }

                await _mediaService.ReorderMediaAsync(projectId, mediaIdsInOrder);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Media reordered successfully",
                    Data = true
                };

                return Ok(response);
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
