using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class CartApiService : BaseApiService, ICartApiService
    {
        public CartApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

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
    }
}

