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
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDto>>>> GetAllCategories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            var response = new ApiResponse<IEnumerable<CategoryDto>>
            {
                Success = true,
                Data = categories
            };

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> GetCategory(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
            {
                return NotFound(new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = "Category not found"
                });
            }

            var response = new ApiResponse<CategoryDto>
            {
                Success = true,
                Data = category
            };

            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> CreateCategory([FromBody] CreateCategoryDto categoryDto)
        {
            var createdCategory = await _categoryService.CreateCategoryAsync(categoryDto);

            var response = new ApiResponse<CategoryDto>
            {
                Success = true,
                Message = "Category created successfully",
                Data = createdCategory
            };

            return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDto>>> UpdateCategory(Guid id, [FromBody] UpdateCategoryDto categoryDto)
        {
            try
            {
                var updatedCategory = await _categoryService.UpdateCategoryAsync(id, categoryDto);

                var response = new ApiResponse<CategoryDto>
                {
                    Success = true,
                    Message = "Category updated successfully",
                    Data = updatedCategory
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<CategoryDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCategory(Guid id)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(id);

                var response = new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Category deleted successfully",
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
