using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OuiAI.Common.DTOs;
using OuiAI.Microservices.Projects.DTOs;
using OuiAI.Microservices.Projects.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OuiAI.Microservices.Projects.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetAllTags()
        {
            var tags = await _tagService.GetAllTagsAsync();

            var response = new ApiResponse<IEnumerable<TagDto>>
            {
                Success = true,
                Data = tags
            };

            return Ok(response);
        }

        [HttpGet("popular")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetPopularTags([FromQuery] int count = 10)
        {
            var tags = await _tagService.GetPopularTagsAsync(count);

            var response = new ApiResponse<IEnumerable<TagDto>>
            {
                Success = true,
                Data = tags
            };

            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> SearchTags([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest(new ApiResponse<IEnumerable<TagDto>>
                {
                    Success = false,
                    Message = "Search term is required"
                });
            }

            var tags = await _tagService.SearchTagsAsync(term);

            var response = new ApiResponse<IEnumerable<TagDto>>
            {
                Success = true,
                Data = tags
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TagDto>>> GetTag(Guid id)
        {
            var tag = await _tagService.GetTagByIdAsync(id);

            if (tag == null)
            {
                return NotFound(new ApiResponse<TagDto>
                {
                    Success = false,
                    Message = "Tag not found"
                });
            }

            var response = new ApiResponse<TagDto>
            {
                Success = true,
                Data = tag
            };

            return Ok(response);
        }

        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<TagDto>>>> GetTagsByProjectId(Guid projectId)
        {
            var tags = await _tagService.GetTagsByProjectIdAsync(projectId);

            var response = new ApiResponse<IEnumerable<TagDto>>
            {
                Success = true,
                Data = tags
            };

            return Ok(response);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TagDto>>> CreateTag([FromBody] CreateTagDto tagDto)
        {
            var createdTag = await _tagService.CreateTagAsync(tagDto);

            var response = new ApiResponse<TagDto>
            {
                Success = true,
                Message = "Tag created successfully",
                Data = createdTag
            };

            return CreatedAtAction(nameof(GetTag), new { id = createdTag.Id }, response);
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<TagDto>>> UpdateTag(Guid id, [FromBody] UpdateTagDto tagDto)
        {
            try
            {
                var updatedTag = await _tagService.UpdateTagAsync(id, tagDto);

                var response = new ApiResponse<TagDto>
                {
                    Success = true,
                    Message = "Tag updated successfully",
                    Data = updatedTag
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<TagDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin,Moderator")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTag(Guid id)
        {
            try
            {
                await _tagService.DeleteTagAsync(id);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Tag deleted successfully",
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
