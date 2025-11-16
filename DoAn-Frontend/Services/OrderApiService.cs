using DoAn_Frontend.Models;

namespace DoAn_Frontend.Services
{
    public class OrderApiService : BaseApiService, IOrderApiService
    {
        public OrderApiService(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
            : base(httpClient, configuration, httpContextAccessor)
        {
        }

        public async Task<Order?> CreateOrderAsync(string shippingAddress, string phone, string recipientName)
        {
            // Use PascalCase property names to match backend CheckoutDto
            var content = JsonContent.Create(new
            {
                ShippingAddress = shippingAddress,
                Phone = phone,
                RecipientName = recipientName
            });
            var request = CreateRequest(HttpMethod.Post, "Orders/checkout", content);
            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>();
            }

            // Read error content for debugging / propagation
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"CreateOrderAsync failed: {response.StatusCode} - {error}");
            throw new Exception(string.IsNullOrWhiteSpace(error) ? "Create order failed" : error);
        }

        public async Task<List<Order>?> GetUserOrdersAsync()
        {
            var request = CreateRequest(HttpMethod.Get, "Orders/history");
            var response = await _httpClient.SendAsync(request);
            return response.IsSuccessStatusCode ? await response.Content.ReadFromJsonAsync<List<Order>>() : new List<Order>();
        }
    }
}

