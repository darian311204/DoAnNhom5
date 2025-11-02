using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class ProductApiService : BaseApiService, IProductApiService
    {
        public ProductApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        public async Task<List<Product>?> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("Products");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Product>>() : new List<Product>();
        }

        public async Task<Product?> GetProductAsync(int id)
        {
            var response = await _httpClient.GetAsync($"Products/{id}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Product>() : null;
        }

        public async Task<List<Product>?> SearchProductsAsync(string searchTerm)
        {
            var response = await _httpClient.GetAsync($"Products/search?searchTerm={searchTerm}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Product>>() : new List<Product>();
        }

        public async Task<List<Category>?> GetCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("Products/categories");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Category>>() : new List<Category>();
        }
    }
}

