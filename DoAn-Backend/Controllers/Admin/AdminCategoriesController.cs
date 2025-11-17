using DoAn_Backend.DTOs;
using DoAn_Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAn_Backend.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public CategoriesController(ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                _logger.LogInformation("GetAllCategories called");
                var categories = await _categoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                return StatusCode(500, new { message = "An error occurred while retrieving categories" });
            }
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveCategories()
        {
            try
            {
                _logger.LogInformation("GetActiveCategories called");
                var categories = await _categoryService.GetActiveCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active categories");
                return StatusCode(500, new { message = "An error occurred while retrieving active categories" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                _logger.LogInformation("GetCategoryById called with id: {Id}", id);
                var category = await _categoryService.GetCategoryByIdAsync(id);

                if (category == null)
                {
                    _logger.LogWarning("Category not found with id: {Id}", id);
                    return NotFound(new { message = "Category not found" });
                }

                return Ok(category);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving the category" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("CreateCategory called with invalid model state");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("CreateCategory called with name: {Name}", dto.CategoryName);
                var result = await _categoryService.CreateCategoryAsync(dto);

                _logger.LogInformation("Category created successfully with id: {Id}", result.CategoryID);
                return CreatedAtAction(nameof(GetCategoryById), new { id = result.CategoryID }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while creating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return StatusCode(500, new { message = "An error occurred while creating the category" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("UpdateCategory called with invalid model state");
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("UpdateCategory called for id: {Id}", id);
                var result = await _categoryService.UpdateCategoryAsync(id, dto);

                _logger.LogInformation("Category updated successfully with id: {Id}", id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Category not found: {Id}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation while updating category");
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category with id: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating the category" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                _logger.LogInformation("DeleteCategory called for id: {Id}", id);
                var result = await _categoryService.DeleteCategoryAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Category not found with id: {Id}", id);
                    return NotFound(new { message = "Category not found" });
                }

                _logger.LogInformation("Category deleted successfully with id: {Id}", id);
                return Ok(new { message = "Category deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Cannot delete category: {Id}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category with id: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while deleting the category" });
            }
        }

        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> ToggleCategoryStatus(int id)
        {
            try
            {
                _logger.LogInformation("ToggleCategoryStatus called for id: {Id}", id);
                var result = await _categoryService.ToggleCategoryStatusAsync(id);

                if (!result)
                {
                    _logger.LogWarning("Category not found with id: {Id}", id);
                    return NotFound(new { message = "Category not found" });
                }

                _logger.LogInformation("Category status toggled successfully with id: {Id}", id);
                return Ok(new { message = "Category status updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status with id: {Id}", id);
                return StatusCode(500, new { message = "An error occurred while updating category status" });
            }
        }
        [HttpGet("test")]
        [AllowAnonymous] // Tạm thời bỏ authentication để test
        public async Task<IActionResult> TestGetCategories()
        {
            try
            {
                _logger.LogInformation("TestGetCategories called");
                var categories = await _categoryService.GetAllCategoriesAsync();
                _logger.LogInformation("Categories count: {Count}", categories?.Count() ?? 0);
                return Ok(new
                {
                    success = true,
                    count = categories?.Count() ?? 0,
                    data = categories
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TestGetCategories");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}