using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IAdminService
    {
        // Products
        Task<Product> CreateProductAsync(Product product);
        Task<Product> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);

        // Categories
        Task<Category> CreateCategoryAsync(Category category);
        Task<Category> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        // Users
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(int userId);
        Task<User> UpdateUserAsync(User user);
        Task<bool> ToggleUserStatusAsync(int userId);

        // Orders
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<bool> UpdateOrderStatusAsync(int orderId, string status);

        // Reviews
        Task<IEnumerable<Review>> GetAllReviewsAsync();
        Task<bool> ToggleReviewStatusAsync(int reviewId);

        // Statistics
        Task<decimal> GetTotalRevenueAsync(DateTime? startDate, DateTime? endDate);
        Task<IEnumerable<object>> GetOrdersByDateAsync(DateTime startDate, DateTime endDate);
    }
}
