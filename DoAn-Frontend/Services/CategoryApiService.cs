using DoAn_Frontend.Models;
using System.Text;
using System.Text.Json;

namespace DoAn_Frontend.Services
{
    public class CategoryApiService : BaseApiService, ICategoryApiService
    {
        private readonly ILogger<CategoryApiService> _logger;

        public CategoryApiService(
            HttpClient httpClient,
            IConfiguration configuration,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CategoryApiService> logger)
            : base(httpClient, configuration, httpContextAccessor)
        {
            _logger = logger;
        }

        public async Task<List<Category>?> GetAllCategoriesAsync()
        {
            try
            {
                _logger.LogInformation("==== GetAllCategoriesAsync START ====");

                // DEBUG: Kiểm tra HttpContext
                var httpContext = _httpContextAccessor.HttpContext;
                _logger.LogInformation("HttpContext is null: {IsNull}", httpContext == null);

                if (httpContext != null)
                {
                    _logger.LogInformation("Session is null: {IsNull}", httpContext.Session == null);

                    if (httpContext.Session != null)
                    {
                        _logger.LogInformation("Session ID: {SessionId}", httpContext.Session.Id);

                        // Thử lấy tất cả keys trong session
                        var sessionKeys = httpContext.Session.Keys.ToList();
                        _logger.LogInformation("Session keys count: {Count}", sessionKeys.Count);
                        _logger.LogInformation("Session keys: [{Keys}]", string.Join(", ", sessionKeys));
                    }
                }

                var token = GetToken();
                _logger.LogInformation("Token from GetToken(): {HasToken}, Length: {Length}",
                    !string.IsNullOrEmpty(token), token?.Length ?? 0);

                // Thử lấy trực tiếp
                var directToken = _httpContextAccessor.HttpContext?.Session.GetString("Token");
                _logger.LogInformation("Token direct from Session: {HasToken}, Length: {Length}",
                    !string.IsNullOrEmpty(directToken), directToken?.Length ?? 0);

                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogError("NO TOKEN - User might not be logged in");
                    return new List<Category>();
                }

                var request = CreateRequest(HttpMethod.Get, "admin/categories");
                var response = await _httpClient.SendAsync(request);

                _logger.LogInformation("Response Status: {StatusCode}", response.StatusCode);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<Category>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                    _logger.LogInformation("Successfully fetched {Count} categories", categories?.Count ?? 0);
                    return categories ?? new List<Category>();
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to fetch categories. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                return new List<Category>();
            }
        }

        public async Task<List<Category>?> GetActiveCategoriesAsync()
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get, "admin/categories/active");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<List<Category>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                        ?? new List<Category>();
                }

                return new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching active categories");
                return new List<Category>();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Get, $"admin/categories/{id}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Category>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching category {CategoryId}", id);
                return null;
            }
        }

        public async Task<bool> CreateCategoryAsync(Category category)
        {
            try
            {
                var dto = new
                {
                    categoryName = category.CategoryName,
                    description = category.Description,
                    isActive = category.IsActive
                };

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = CreateRequest(HttpMethod.Post, "admin/categories", content);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully created category: {CategoryName}", category.CategoryName);
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to create category. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating category");
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            try
            {
                var dto = new
                {
                    categoryName = category.CategoryName,
                    description = category.Description,
                    isActive = category.IsActive
                };

                var json = JsonSerializer.Serialize(dto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = CreateRequest(HttpMethod.Put, $"admin/categories/{category.CategoryID}", content);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully updated category {CategoryId}", category.CategoryID);
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to update category. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating category {CategoryId}", category.CategoryID);
                return false;
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Delete, $"admin/categories/{id}");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully deleted category {CategoryId}", id);
                    return true;
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Failed to delete category. Status: {StatusCode}, Error: {Error}",
                    response.StatusCode, errorContent);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting category {CategoryId}", id);
                return false;
            }
        }

        public async Task<bool> ToggleCategoryStatusAsync(int id)
        {
            try
            {
                var request = CreateRequest(HttpMethod.Patch, $"admin/categories/{id}/toggle-status");
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Successfully toggled status for category {CategoryId}", id);
                    return true;
                }

                _logger.LogWarning("Failed to toggle category status. Status: {StatusCode}", response.StatusCode);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for category {CategoryId}", id);
                return false;
            }
        }
    }
}