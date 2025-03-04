using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OuiAI.Common.DTOs;
using OuiAI.Microservices.Projects.DTOs;
using OuiAI.Microservices.Projects.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _projectService;

        public ProjectsController(IProjectService projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetProjects(
            [FromQuery] string searchTerm = null,
            [FromQuery] Guid? categoryId = null,
            [FromQuery] List<Guid> tagIds = null,
            [FromQuery] ProjectSortBy sortBy = ProjectSortBy.Latest,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var filter = new ProjectFilter
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                TagIds = tagIds,
                SortBy = sortBy,
                IncludePrivate = false
            };

            var projects = await _projectService.GetProjectsAsync(filter, page, pageSize, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [HttpGet("featured")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetFeaturedProjects([FromQuery] int count = 5)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var projects = await _projectService.GetFeaturedProjectsAsync(count, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [HttpGet("trending")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetTrendingProjects([FromQuery] int count = 5)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var projects = await _projectService.GetTrendingProjectsAsync(count, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [HttpGet("recent")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetRecentProjects([FromQuery] int count = 5)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var projects = await _projectService.GetRecentProjectsAsync(count, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetUserProjects(
            Guid userId, 
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 10)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var projects = await _projectService.GetUserProjectsAsync(userId, page, pageSize, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> GetProject(Guid id)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var project = await _projectService.GetProjectByIdAsync(id, currentUserId);

            if (project == null)
            {
                return NotFound(new ApiResponse<ProjectDto>
                {
                    Success = false,
                    Message = "Project not found"
                });
            }

            var response = new ApiResponse<ProjectDto>
            {
                Success = true,
                Data = project
            };

            return Ok(response);
        }

        [HttpGet("{id}/related")]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProjectSummaryDto>>>> GetRelatedProjects(Guid id, [FromQuery] int count = 5)
        {
            Guid? currentUserId = User.Identity.IsAuthenticated 
                ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)) 
                : null;

            var projects = await _projectService.GetRelatedProjectsAsync(id, count, currentUserId);

            var response = new ApiResponse<IEnumerable<ProjectSummaryDto>>
            {
                Success = true,
                Data = projects
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> CreateProject([FromBody] CreateProjectDto projectDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var createdProject = await _projectService.CreateProjectAsync(projectDto, userId);

            var response = new ApiResponse<ProjectDto>
            {
                Success = true,
                Message = "Project created successfully",
                Data = createdProject
            };

            return CreatedAtAction(nameof(GetProject), new { id = createdProject.Id }, response);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<ProjectDto>>> UpdateProject(Guid id, [FromBody] UpdateProjectDto projectDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                var updatedProject = await _projectService.UpdateProjectAsync(id, projectDto, userId);

                var response = new ApiResponse<ProjectDto>
                {
                    Success = true,
                    Message = "Project updated successfully",
                    Data = updatedProject
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ProjectDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteProject(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            try
            {
                await _projectService.DeleteProjectAsync(id, userId);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Project deleted successfully",
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

        [Authorize]
        [HttpPost("{id}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> LikeProject(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var userName = User.FindFirstValue(ClaimTypes.Name);
            
            var result = await _projectService.LikeProjectAsync(id, userId, userName);

            var response = new ApiResponse<bool>
            {
                Success = true,
                Message = "Project liked successfully",
                Data = result
            };

            return Ok(response);
        }

        [Authorize]
        [HttpDelete("{id}/like")]
        public async Task<ActionResult<ApiResponse<bool>>> UnlikeProject(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var result = await _projectService.UnlikeProjectAsync(id, userId);

            var response = new ApiResponse<bool>
            {
                Success = true,
                Message = "Project unliked successfully",
                Data = result
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost("{id}/view")]
        public async Task<ActionResult<ApiResponse<bool>>> RecordProjectView(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
            var result = await _projectService.RecordProjectViewAsync(id, userId);

            var response = new ApiResponse<bool>
            {
                Success = true,
                Message = "Project view recorded successfully",
                Data = result
            };

            return Ok(response);
        }
    }
}
