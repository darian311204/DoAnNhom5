using DoAn_Backend.Models;

namespace DoAn_Backend.Services
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<Product?> GetProductByIdAsync(int productId);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
    }
}
<<<<<<< HEAD


=======
>>>>>>> parent of d4e55c3 (1)
