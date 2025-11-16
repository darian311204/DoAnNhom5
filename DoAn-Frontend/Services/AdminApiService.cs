using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class AdminApiService : BaseApiService, IAdminApiService
    {
        public AdminApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        // Orders
        public async Task<List<Order>?> GetAdminOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "admin/Orders");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? cancelReason)
        {
            try
            {
                var content = JsonContent.Create(new { Status = status, CancelReason = cancelReason });
                var request = CreateRequest(HttpMethod.Put, $"admin/Orders/{orderId}/status", content);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update order status error: {ex.Message}");
                return false;
            }
        }

        // Products
        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                var content = JsonContent.Create(product);
                // BackendUrl already ends with "/api/", so we just append the admin route segment.
                var request = CreateRequest(HttpMethod.Post, "admin/Products", content);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create product error: {ex.Message}");
                return false;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get, $"Products/{id}");
                var response = await _httpClient.SendAsync(request);

                Console.WriteLine($"Get product {id} - Status: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<Product>();
                    Console.WriteLine($"Product retrieved: {product?.ProductName}");
                    return product;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Get product failed: {response.StatusCode} - {errorContent}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Get product error: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> UpdateProductAsync(Product product)
        {
            try
            {
                var content = JsonContent.Create(product);
                var request = CreateRequest(HttpMethod.Put, $"admin/Products/{product.ProductID}", content);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update product error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Delete, $"admin/Products/{productId}");
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete product error: {ex.Message}");
                return false;
            }
        }
    }
}

