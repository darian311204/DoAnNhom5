using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IAdminApiService
    {
        // Orders
        Task<List<Order>?> GetAdminOrdersAsync();

        // Products
        Task<bool> CreateProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int id);
        Task<bool> UpdateProductAsync(Product product);
        Task<bool> DeleteProductAsync(int productId);
    }
}

