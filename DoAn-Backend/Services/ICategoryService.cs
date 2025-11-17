using DoAn_Backend.DTOs;

namespace DoAn_Backend.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
        Task<CategoryDto?> GetCategoryByIdAsync(int id);
        Task<CategoryDto> CreateCategoryAsync(CreateCategoryDto dto);
        Task<CategoryDto> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<bool> ToggleCategoryStatusAsync(int categoryId);
        Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    }
}