using DoAn_Backend.DTOs;
using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IAdminService
    {
        // Products
        Task<Product> CreateProductAsync(CreateProductDto dto);
        Task<Product> UpdateProductAsync(int id, UpdateProductDto dto);
        Task<bool> DeleteProductAsync(int productId);

        // Categories
        Task<Category> CreateCategoryAsync(CreateCategoryDto dto);
        Task<Category> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        // Users
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<User> UpdateUserAsync(User user);
        Task<bool> ToggleUserStatusAsync(int userId);

        // Orders
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? cancelReason = null);

        // Reviews
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<bool> ToggleReviewStatusAsync(int reviewId);

        // Statistics
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<object>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate);
    }
}
