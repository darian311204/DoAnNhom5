using DoAn_Frontend.Models;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace DoAn_Frontend.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _httpClient.BaseAddress = new Uri(_configuration["BackendUrl"]!);
        }

        private string? GetToken() => _httpContextAccessor.HttpContext?.Session.GetString("Token");
        
        private HttpRequestMessage CreateRequest(HttpMethod method, string url, HttpContent? content = null)
        {
            var request = new HttpRequestMessage(method, url);
            
            // Add authorization header if token exists
            var token = GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            
            if (content != null)
            {
                request.Content = content;
            }
            
            return request;
        }
        
        public bool IsTokenExpired()
        {
            try
            {
                var token = GetToken();
                if (string.IsNullOrEmpty(token)) return true;
                
                // Parse JWT to check expiration
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                
                return jwt.ValidTo < DateTime.UtcNow;
            }
            catch
            {
                return true;
            }
        }

        // Auth
        public async Task<AuthResponse?> LoginAsync(LoginDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/login", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    if (result != null)
                    {
                        // Store JWT token in session
                        _httpContextAccessor.HttpContext?.Session.SetString("Token", result.Token);
                        _httpContextAccessor.HttpContext?.Session.SetString("User", JsonSerializer.Serialize(result.User));
                    }
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Login failed: {response.StatusCode} - {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
            }
            return null;
        }

        public async Task<AuthResponse?> RegisterAsync(RegisterDto dto)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("Auth/register", dto);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponse>();
                    // Don't store session after registration - let user log in manually
                    return result;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Registration failed: {response.StatusCode} - {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Registration error: {ex.Message}");
                throw; // Re-throw to be caught by controller
            }
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext?.Session.Remove("Token");
            _httpContextAccessor.HttpContext?.Session.Remove("User");
        }

        public bool IsAuthenticated()
        {
            var token = GetToken();
            if (string.IsNullOrEmpty(token)) return false;
            
            // Check if token is expired
            if (IsTokenExpired())
            {
                // Clear expired token
                Logout();
                return false;
            }
            
            return true;
        }
        
        public bool IsAdmin() => GetCurrentUser()?.Role == "Admin";

        public User? GetCurrentUser()
        {
            var userJson = _httpContextAccessor.HttpContext?.Session.GetString("User");
            return string.IsNullOrEmpty(userJson) ? null : JsonSerializer.Deserialize<User>(userJson);
        }

        // Products
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

        // Cart
        public async Task<List<Cart>?> GetCartAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Cart");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Cart>>() : new List<Cart>();
        }

        public async Task<bool> AddToCartAsync(int productId, int quantity)
        {
            try
            {
                // Send as PascalCase to match backend
                var content = JsonContent.Create(new { ProductId = productId, Quantity = quantity });
                Console.WriteLine($"Sending to cart: ProductId={productId}, Quantity={quantity}");
                var request = CreateRequest(HttpMethod.Post, "Cart/add", content);
                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Add to cart failed: {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Add to cart error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> UpdateCartQuantityAsync(int cartId, int quantity)
        {
            try
            {
                var content = new StringContent(quantity.ToString(), System.Text.Encoding.UTF8, "application/json");
                var request = CreateRequest(HttpMethod.Put, $"Cart/{cartId}", content);
                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Update cart error: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> RemoveFromCartAsync(int cartId)
        {
            var request = CreateRequest(HttpMethod.Delete, $"Cart/{cartId}");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetCartItemCountAsync()
        {
            try
            {
                var cart = await GetCartAsync();
                return cart?.Sum(item => item.Quantity) ?? 0;
            }
            catch
            {
                return 0;
            }
        }

        // Orders
        public async Task<Order?> CreateOrderAsync(string shippingAddress, string phone, string recipientName)
        {
            var content = JsonContent.Create(new { shippingAddress, phone, recipientName });
            var request = CreateRequest(HttpMethod.Post, "Orders/checkout", content);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<Order>() : null;
        }

        public async Task<List<Order>?> GetUserOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Orders/history");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }

        // Reviews
        public async Task<List<Review>?> GetProductReviewsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"Reviews/product/{productId}");
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Review>>() : new List<Review>();
        }

        public async Task<bool> AddReviewAsync(int productId, int rating, string comment)
        {
            var content = JsonContent.Create(new { productId, rating, comment });
            var request = CreateRequest(HttpMethod.Post, "Reviews", content);
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode;
        }

        // Admin
        public async Task<List<Order>?> GetAdminOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "admin/Orders");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }

        // Admin Products
        public async Task<bool> CreateProductAsync(Product product)
        {
            try
            {
                var content = JsonContent.Create(product);
                var request = CreateRequest(HttpMethod.Post, "api/admin/Products", content);
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
                var request = CreateRequest(HttpMethod.Put, $"api/admin/Products/{product.ProductID}", content);
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
                var request = CreateRequest(HttpMethod.Delete, $"api/admin/Products/{productId}");
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
