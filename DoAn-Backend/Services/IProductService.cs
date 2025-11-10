using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IProductService
    {
        // ---------------- LẤY DỮ LIỆU ----------------
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();

        // ---------------- THÊM / CẬP NHẬT / XÓA ----------------
        Task CreateProductAsync(Product product);
        Task UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int id);
    }
}
