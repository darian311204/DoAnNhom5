using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public interface IProductApiService
    {
        Task<List<Product>?> GetProductsAsync();
        Task<Product?> GetProductAsync(int id);
        Task<List<Product>?> SearchProductsAsync(string searchTerm);
        Task<List<Category>?> GetCategoriesAsync();
    }
}

