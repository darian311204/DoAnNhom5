using DoAn_Frontend.Models;
using Microsoft.Extensions.Logging;

namespace DoAn_Frontend.Services
{
    public class AdminApiService : BaseApiService, IAdminApiService
    {
        private readonly ILogger<AdminApiService> _logger;

        public AdminApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AdminApiService> logger)
            : base(httpClient, configuration, httpContextAccessor)
        {
            _logger = logger;
        }

        // Orders
        public async Task<List<Order>?> GetAdminOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "admin/Orders");
            var response = await _httpClient.SendAsync(request);
            _logger.LogInformation("GetAdminOrdersAsync - Status {StatusCode}", response.StatusCode);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string status, string? cancelReason)
        {
            try
            {
                var content = JsonContent.Create(new { Status = status, CancelReason = cancelReason });
                var request = CreateRequest(HttpMethod.Put, $"admin/Orders/{orderId}/status", content);
                var response = await _httpClient.SendAsync(request);
                _logger.LogInformation("UpdateOrderStatusAsync({OrderId}, {Status}) - Status {StatusCode}", orderId, status, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateOrderStatusAsync({OrderId}) failed", orderId);
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
                _logger.LogInformation("CreateProductAsync({Name}) - Status {StatusCode}", product.ProductName, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateProductAsync({Name}) failed", product.ProductName);
                return false;
            }
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get, $"Products/{id}");
                var response = await _httpClient.SendAsync(request);

                _logger.LogInformation("GetProductByIdAsync({Id}) - Status {StatusCode}", id, response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var product = await response.Content.ReadFromJsonAsync<Product>();
                    _logger.LogInformation("Product retrieved: {Name}", product?.ProductName);
                    return product;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("GetProductByIdAsync({Id}) failed: {StatusCode} - {Error}", id, response.StatusCode, errorContent);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProductByIdAsync({Id}) error", id);
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
                _logger.LogInformation("UpdateProductAsync({Id}) - Status {StatusCode}", product.ProductID, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateProductAsync({Id}) failed", product.ProductID);
                return false;
            }
        }

        public async Task<bool> DeleteProductAsync(int productId)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Delete, $"admin/Products/{productId}");
                var response = await _httpClient.SendAsync(request);
                _logger.LogInformation("DeleteProductAsync({Id}) - Status {StatusCode}", productId, response.StatusCode);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteProductAsync({Id}) failed", productId);
                return false;
            }
        }

        public async Task<string?> UploadImageAsync(IFormFile file)
        {
            try
            {
                using var content = new MultipartFormDataContent();
                var streamContent = new StreamContent(file.OpenReadStream());
                content.Add(streamContent, "file", file.FileName);

                var request = CreateRequest(HttpMethod.Post, "admin/Products/upload-image", content);
                var response = await _httpClient.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("UploadImageAsync failed: {StatusCode} - {Error}", response.StatusCode, error);
                    return null;
                }

                var result = await response.Content.ReadFromJsonAsync<UploadImageResponse>();
                _logger.LogInformation("UploadImageAsync succeeded, url={Url}", result?.ImageUrl);
                return result?.ImageUrl;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadImageAsync error");
                return null;
            }
        }

        private class UploadImageResponse
        {
            public string ImageUrl { get; set; } = string.Empty;
        }
    }
}

