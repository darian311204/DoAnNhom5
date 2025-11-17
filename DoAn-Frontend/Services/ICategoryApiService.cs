using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface ICategoryApiService
    {
        Task<List<Category>?> GetAllCategoriesAsync();
        Task<List<Category>?> GetActiveCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<bool> CreateCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<bool> ToggleCategoryStatusAsync(int categoryId);
    }
}