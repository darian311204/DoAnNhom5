using DoAn_Backend.Data;
using DoAn_Backend.DTOs;
using DoAn_Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Backend.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ApplicationDbContext context, ILogger<CategoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Products)
                    .Select(c => new CategoryDto
                    {
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        ProductCount = c.Products.Count
                    })
                    .OrderByDescending(c => c.CreatedAt)
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all categories");
                throw;
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .Where(c => c.IsActive)
                    .Include(c => c.Products)
                    .Select(c => new CategoryDto
                    {
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        ProductCount = c.Products.Count
                    })
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active categories");
                throw;
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .Where(c => c.CategoryID == id)
                    .Select(c => new CategoryDto
                    {
                        CategoryID = c.CategoryID,
                        CategoryName = c.CategoryName,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        CreatedAt = c.CreatedAt,
                        UpdatedAt = c.UpdatedAt,
                        ProductCount = c.Products.Count
                    })
                    .FirstOrDefaultAsync();

                return category;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting category by id: {CategoryId}", id);
                throw;
            }
        }

        public async Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto)
        {
            try
            {
                // Check if category name already exists
                var existingCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower());

                if (existingCategory != null)
                {
                    throw new InvalidOperationException("A category with this name already exists");
                }

                var category = new Category
                {
                    CategoryName = dto.CategoryName.Trim(),
                    Description = dto.Description?.Trim(),
                    IsActive = dto.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Created category: {CategoryName} with ID: {CategoryId}",
                    category.CategoryName, category.CategoryID);

                return new CategoryDto
                {
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    ProductCount = 0
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                throw;
            }
        }

        public async Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryID == id);

                if (category == null)
                {
                    throw new KeyNotFoundException($"Category with ID {id} not found");
                }

                // Check if another category has the same name
                var duplicateCategory = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryName.ToLower() == dto.CategoryName.ToLower()
                        && c.CategoryID != id);

                if (duplicateCategory != null)
                {
                    throw new InvalidOperationException("Another category with this name already exists");
                }

                category.CategoryName = dto.CategoryName.Trim();
                category.Description = dto.Description?.Trim();
                category.IsActive = dto.IsActive;
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Updated category: {CategoryId} - {CategoryName}",
                    category.CategoryID, category.CategoryName);

                return new CategoryDto
                {
                    CategoryID = category.CategoryID,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    IsActive = category.IsActive,
                    CreatedAt = category.CreatedAt,
                    UpdatedAt = category.UpdatedAt,
                    ProductCount = category.Products.Count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category: {CategoryId}", id);
                throw;
            }
        }
        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.CategoryID == categoryId);

                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", categoryId);
                    return false;
                }

                // Kiểm tra xem có sản phẩm không
                if (category.Products != null && category.Products.Any())
                {
                    throw new InvalidOperationException(
                        $"Cannot delete category '{category.CategoryName}' because it has {category.Products.Count} product(s). " +
                        "Please remove all products first or deactivate the category instead.");
                }

                // Xóa category
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Permanently deleted category: {CategoryId} - {CategoryName}",
                    categoryId, category.CategoryName);

                return true;
            }
            catch (InvalidOperationException)
            {
                // Re-throw để controller có thể handle
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category: {CategoryId}", categoryId);
                throw;
            }
        }

        public async Task<bool> ToggleCategoryStatusAsync(int categoryId)
        {
            try
            {
                var category = await _context.Categories
                    .FirstOrDefaultAsync(c => c.CategoryID == categoryId);

                if (category == null)
                {
                    _logger.LogWarning("Category not found: {CategoryId}", categoryId);
                    return false;
                }

                category.IsActive = !category.IsActive;
                category.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Toggled category status: {CategoryId} - IsActive: {IsActive}",
                    categoryId, category.IsActive);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling category status: {CategoryId}", categoryId);
                throw;
            }
        }
    }
}