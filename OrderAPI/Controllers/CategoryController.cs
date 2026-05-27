using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Data;
using OrderAPI.DTOs.Auth.Login;
using OrderAPI.DTOs.Category;
using OrderAPI.DTOs.Common;
using OrderAPI.Exceptions;
using OrderAPI.Services;
using OrderAPI.Services.Interfaces;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace OrderAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        public CategoryController(ICategoryService categoryServicecontext)
        {
            _categoryService = categoryServicecontext;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCategory(string? keyword, int page = 1, int pageSize = 5)
        {
            var categories = await _categoryService.GetAllCategory(keyword, page, pageSize);

            return Ok(ApiResponse<PagedList<CategoryResponse>>.Ok(
                "Get categories successfully",
                categories));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            var categories = await _categoryService.GetCategoryById(id);
            return Ok(ApiResponse<CategoryResponse>.Ok($"Get category {id} successfully", categories));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed", errors.ToList()));
            }
            var createdCategory = await _categoryService.CreateCategory(request);

            return CreatedAtAction(
                nameof(GetCategoryById),
                new { id = createdCategory.Id },
                ApiResponse<CategoryResponse>.Ok(
                    "Create category successfully",
                    createdCategory));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors).Select(x=>x.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.Fail("Validation failed",errors.ToList()));
            }
            var updatedCategory = await _categoryService.UpdateCategory(id, request);

            return Ok(ApiResponse<CategoryResponse>.Ok(
                $"Update category {id} successfully",
                updatedCategory));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategory(id);

            return NoContent();
        }
    }
}
